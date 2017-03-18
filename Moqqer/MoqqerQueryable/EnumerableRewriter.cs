using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MoqqerNamespace.MoqqerQueryable
{
    internal class EnumerableRewriter : OldExpressionVisitor
    {
        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            var obj = Visit(m.Object);
            var args = VisitExpressionList(m.Arguments);

            // check for args changed
            if (obj != m.Object || args != m.Arguments)
            {
                var typeArgs = (m.Method.IsGenericMethod) ? m.Method.GetGenericArguments() : null;

                Debug.Assert(m.Method.DeclaringType != null, "m.Method.DeclaringType != null");
                if ((m.Method.IsStatic || m.Method.DeclaringType.IsAssignableFrom(obj.Type))
                    && ArgsMatch(m.Method, args, typeArgs))
                {
                    // current method is still valid
                    return Expression.Call(obj, m.Method, args);
                }
                else if (m.Method.DeclaringType == typeof(Queryable))
                {
                    // convert Queryable method to Enumerable method
                    var seqMethod = FindEnumerableMethod(m.Method.Name, args, typeArgs);
                    args = FixupQuotedArgs(seqMethod, args);
                    return Expression.Call(obj, seqMethod, args);
                }
                else
                {
                    // rebind to new method
                    var flags = BindingFlags.Static | (m.Method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);
                    var method = FindMethod(m.Method.DeclaringType, m.Method.Name, args, typeArgs, flags);
                    args = FixupQuotedArgs(method, args);
                    return Expression.Call(obj, method, args);
                }
            }
            return m;
        }

        private ReadOnlyCollection<Expression> FixupQuotedArgs(MethodInfo mi, ReadOnlyCollection<Expression> argList)
        {
            var pis = mi.GetParameters();
            if (pis.Length > 0)
            {
                List<Expression> newArgs = null;
                for (int i = 0, n = pis.Length; i < n; i++)
                {
                    var arg = argList[i];
                    var pi = pis[i];
                    arg = FixupQuotedExpression(pi.ParameterType, arg);
                    if (newArgs == null && arg != argList[i])
                    {
                        newArgs = new List<Expression>(argList.Count);
                        for (var j = 0; j < i; j++)
                        {
                            newArgs.Add(argList[j]);
                        }
                    }
                    newArgs?.Add(arg);
                }
                if (newArgs != null)
                    argList = newArgs.ToReadOnlyCollection();
            }
            return argList;
        }

        private Expression FixupQuotedExpression(Type type, Expression expression)
        {
            var expr = expression;
            while (true)
            {
                if (type.IsAssignableFrom(expr.Type))
                    return expr;
                if (expr.NodeType != ExpressionType.Quote)
                    break;
                expr = ((UnaryExpression)expr).Operand;
            }
            if (!type.IsAssignableFrom(expr.Type) && type.IsArray && expr.NodeType == ExpressionType.NewArrayInit)
            {
                var strippedType = StripExpression(expr.Type);
                if (type.IsAssignableFrom(strippedType))
                {
                    var elementType = type.GetElementType();
                    var na = (NewArrayExpression)expr;
                    var exprs = new List<Expression>(na.Expressions.Count);
                    for (int i = 0, n = na.Expressions.Count; i < n; i++)
                    {
                        exprs.Add(FixupQuotedExpression(elementType, na.Expressions[i]));
                    }
                    expression = Expression.NewArrayInit(elementType, exprs);
                }
            }
            return expression;
        }

        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            return lambda;
        }

        private static Type GetPublicType(Type t)
        {
            // If we create a constant explicitly typed to be a private nested type,
            // such as Lookup<,>.Grouping or a compiler-generated iterator class, then
            // we cannot use the expression tree in a context which has only execution
            // permissions.  We should endeavour to translate constants into 
            // new constants which have public types.

            
            if (t.IsGenericType && t.GetGenericTypeDefinition().Name == "Grouping")
                return typeof(IGrouping<,>).MakeGenericType(t.GetGenericArguments());
            if (!t.IsNestedPrivate)
                return t;
            foreach (var iType in t.GetInterfaces())
            {
                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return iType;
            }
            if (typeof(IEnumerable).IsAssignableFrom(t))
                return typeof(IEnumerable);
            return t;
        }

        internal override Expression VisitConstant(ConstantExpression c)
        {
            var sq = c.Value as EnumerableMoqqerQuery;
            if (sq != null)
            {
                if (sq.Enumerable != null)
                {
                    var t = GetPublicType(sq.Enumerable.GetType());
                    return Expression.Constant(sq.Enumerable, t);
                }
                return Visit(sq.Expression);
            }
            return c;
        }

        internal override Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        private static volatile ILookup<string, MethodInfo> _seqMethods;
        static MethodInfo FindEnumerableMethod(string name, ReadOnlyCollection<Expression> args, params Type[] typeArgs)
        {
            if (_seqMethods == null)
            {
                _seqMethods = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).ToLookup(m => m.Name);
            }
            var mi = _seqMethods[name].FirstOrDefault(m => ArgsMatch(m, args, typeArgs));
            if (mi == null)
                throw new Exception($"Error.NoMethodOnTypeMatchingArguments({name}, {typeof(Enumerable)})");
            if (typeArgs != null)
                return mi.MakeGenericMethod(typeArgs);
            return mi;
        }

        internal static MethodInfo FindMethod(Type type, string name, ReadOnlyCollection<Expression> args, Type[] typeArgs, BindingFlags flags)
        {
            var methods = type.GetMethods(flags).Where(m => m.Name == name).ToArray();
            if (methods.Length == 0)
                throw new Exception($"Error.NoMethodOnType({name}, {type});");
            var mi = methods.FirstOrDefault(m => ArgsMatch(m, args, typeArgs));
            if (mi == null)
                throw new Exception($"Error.NoMethodOnTypeMatchingArguments({name}, {type});");
            if (typeArgs != null)
                return mi.MakeGenericMethod(typeArgs);
            return mi;
        }

        private static bool ArgsMatch(MethodInfo m, ReadOnlyCollection<Expression> args, Type[] typeArgs)
        {
            var mParams = m.GetParameters();
            if (mParams.Length != args.Count)
                return false;
            if (!m.IsGenericMethod && typeArgs != null && typeArgs.Length > 0)
            {
                return false;
            }
            if (!m.IsGenericMethodDefinition && m.IsGenericMethod && m.ContainsGenericParameters)
            {
                m = m.GetGenericMethodDefinition();
            }
            if (m.IsGenericMethodDefinition)
            {
                if (typeArgs == null || typeArgs.Length == 0)
                    return false;
                if (m.GetGenericArguments().Length != typeArgs.Length)
                    return false;
                m = m.MakeGenericMethod(typeArgs);
                mParams = m.GetParameters();
            }
            for (int i = 0, n = args.Count; i < n; i++)
            {
                var parameterType = mParams[i].ParameterType;
                if (parameterType.IsByRef)
                    parameterType = parameterType.GetElementType();
                var arg = args[i];
                if (!parameterType.IsAssignableFrom(arg.Type))
                {
                    if (arg.NodeType == ExpressionType.Quote)
                    {
                        arg = ((UnaryExpression)arg).Operand;
                    }
                    if (!parameterType.IsAssignableFrom(arg.Type) &&
                        !parameterType.IsAssignableFrom(StripExpression(arg.Type)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static Type StripExpression(Type type)
        {
            var isArray = type.IsArray;
            var tmp = isArray ? type.GetElementType() : type;
            var eType = TypeHelper.FindGenericType(typeof(Expression<>), tmp);
            if (eType != null)
                tmp = eType.GetGenericArguments()[0];
            if (isArray)
            {
                var rank = type.GetArrayRank();
                return (rank == 1) ? tmp.MakeArrayType() : tmp.MakeArrayType(rank);
            }
            return type;
        }
    }
}