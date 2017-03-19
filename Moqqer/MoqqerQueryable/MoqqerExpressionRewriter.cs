using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MoqqerNamespace.MoqqerQueryable
{
    public static class MoqqerExpressionRewriter
    {
        public static Expression RewriteLinqCall(Expression expression)
        {
            ProcessMemberCallExpression(ref expression);

            return expression;
        }

        private static void ProcessMemberCallExpression(ref Expression expression)
        {
            var meth = expression as MethodCallExpression;

            if (meth?.Arguments.Count != 2) return;

            var arg = meth.Arguments[1] as UnaryExpression;

            var operand = arg?.Operand as LambdaExpression;

            if (operand == null) return;

            var newOperand = RewriteLambdaExpression(operand);

            var newMeth = Expression.Call(meth.Method, meth.Arguments[0], newOperand);

            expression = newMeth;
        }

        public static Expression RewriteLambdaExpression(LambdaExpression original)
        {
            if (original.NodeType != ExpressionType.Lambda)
                throw new ArgumentOutOfRangeException(nameof(original), "Expression must be of type Lambda");

            var body = original.Body;

            var newBody = RewriteExpression(body);

            var lambda = Expression.Lambda(newBody, original.Parameters);

            return lambda;
        }

        private static Expression RewriteExpression(Expression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return RewriteMemberExpression(body as MemberExpression);
                case ExpressionType.Call:
                    return RewriteMethodCallExpression(body as MethodCallExpression);

                // Projections - Select(x => ?)
                case ExpressionType.New: // new { x }
                    return RewriteNewExpression(body as NewExpression);
                case ExpressionType.MemberInit:
                    return RewriteMemberInitExpression(body as MemberInitExpression);
                case ExpressionType.ListInit:
                    return RewriteListInitExpression(body as ListInitExpression);
                case ExpressionType.NewArrayInit:
                    return RewriteArrayExpression(body as NewArrayExpression);

                case ExpressionType.Convert: // (int?) integer
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Increment:
                case ExpressionType.Decrement:
                case ExpressionType.UnaryPlus:
                    return RewriteUnaryExpression(body as UnaryExpression);


                // Comparison types - Where(x => ?)
                case ExpressionType.Equal: // a == b
                case ExpressionType.NotEqual: // a != b
                case ExpressionType.GreaterThan: // a > b
                case ExpressionType.GreaterThanOrEqual: // a >= b
                case ExpressionType.LessThan: // a < b
                case ExpressionType.LessThanOrEqual: // a <= b
                    return RewriteComparisonBinaryExpression(body as BinaryExpression);

                // Conditional operators - Where(x => ?)
                case ExpressionType.AndAlso: // a && b
                case ExpressionType.OrElse: // a || b
                    return RewriteConditionalBinaryExpression(body as BinaryExpression);
                case ExpressionType.Conditional: // a ? b : c
                    return RewriteConditionalExpression(body as ConditionalExpression);
                case ExpressionType.IsTrue
                : // TODO: http://stackoverflow.com/questions/42875376/c-sharp-how-to-create-expressiontype-istrue-isfalse
                case ExpressionType.IsFalse
                : // TODO: http://stackoverflow.com/questions/42875376/c-sharp-how-to-create-expressiontype-istrue-isfalse
                    break;
                case ExpressionType.Not:
                    return RewriteConditionalUnaryExpression(body as UnaryExpression);

                // Numeric Calculation Types
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.Coalesce:
                    return RewriteMethodBinaryExpression(body as BinaryExpression);

                // Static
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                case ExpressionType.Default:
                    break;

                // Assignment
                case ExpressionType.Assign:
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;

                // TBD
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.Unbox:
                    break;
                
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return body;
        }

        private static Expression RewriteArrayExpression(NewArrayExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteMemberInitExpression)} as it is null");

            var newExpressions = expression.Expressions.Select(RewriteExpression).ToList();

            return Expression.NewArrayInit(expression.Type.GetElementType(), newExpressions);
        }

        private static Expression RewriteListInitExpression(ListInitExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteMemberInitExpression)} as it is null");

            var newInitialisers = expression.Initializers.Select(RewriteListInitElementInitialisers).ToList();

            var newExpression = RewriteExpression(expression.NewExpression) as NewExpression ?? expression.NewExpression;

            return Expression.ListInit(newExpression, newInitialisers);
        }

        private static ElementInit RewriteListInitElementInitialisers(ElementInit init)
        {
            var newArgs = init.Arguments.Select(RewriteExpression).ToList();

            return Expression.ElementInit(init.AddMethod, newArgs);
        }

        private static Expression RewriteMemberInitExpression(MemberInitExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteMemberInitExpression)} as it is null");

            var bindings = expression.Bindings.Select(RewriteMemberBinding).ToList();

            var newExpression = Expression.MemberInit(expression.NewExpression, bindings);

            return newExpression;
        }

        private static MemberBinding RewriteMemberBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return RewriteAssignmentMemberBinding(binding as MemberAssignment);
                case MemberBindingType.MemberBinding:
                    break;
                case MemberBindingType.ListBinding:
                    break;
            }

            return binding;
        }

        private static MemberBinding RewriteAssignmentMemberBinding(MemberAssignment binding)
        {
            return Expression.Bind(binding.Member, RewriteExpression(binding.Expression));
        }

        private static Expression RewriteMethodCallExpression(MethodCallExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteMethodCallExpression)} as it is null");

            var check = GetMethodCallMemberAccessCheckExpression(expression);

            if (check == null)
                return expression;

            return Expression.Condition(check, expression, GetDefaultConstantExpression(expression));
        }

        /// <summary>
        /// Should this throw when trying to select a value type from a reference type. 
        /// E.g. x => x.NavigationProperty.Integer
        /// Should throw an exception if SomeNullableType is Null? 
        /// Because returning default SomeValueType is incorrect
        /// Correct expression should be x => (int?) x.NavigationProperty.Integer 
        /// so that null can be returned when NavigationProperty is null
        /// </summary>
        public static bool ThrowOnNonNullableReferenceTypeSelection { get; set; } = true;

        private static Expression RewriteUnaryExpression(UnaryExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteUnaryExpression)} as it is null");
            
            var operand = GetMemberAccessCheckExpression(expression.Operand);

            if (operand == null)
                return expression;

            if (expression.IsLiftedToNull)
                return Expression.Condition(operand, expression, GetDefaultConstantExpression(expression));

            if (ThrowOnNonNullableReferenceTypeSelection)
                throw new MoqqerException($"{expression}'s return type is a non-nullable type, but has nullable type access. " +
                                          $"Either set {nameof(Moqqer)}.{nameof(ThrowOnNonNullableReferenceTypeSelection)} to False," +
                                          "and make sure the navigations properties are not null. " +
                                          "Or make sure you cast Value types accessing navigation properties to nullable types. " +
                                          "E.g. x => (int?) x.NavigationProperty.Integer");

            return expression;
        }

        private static Expression RewriteMethodBinaryExpression(BinaryExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteMethodBinaryExpression)} as it is null");

            var left = RewriteExpression(expression.Left);
            var right = RewriteExpression(expression.Right);

            return Expression.MakeBinary(expression.NodeType, left, right, expression.IsLiftedToNull, expression.Method, expression.Conversion);
        }

        private static Expression RewriteConditionalUnaryExpression(UnaryExpression expression)
        {
            if(expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteConditionalUnaryExpression)} as it is null");

            var check = GetMemberAccessCheckExpression(expression.Operand);
            
            return check == null
                ? (Expression) expression
                : Expression.MakeBinary(ExpressionType.AndAlso, check, expression);
        }

        private static Expression RewriteConditionalBinaryExpression(BinaryExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteConditionalBinaryExpression)} as it is null");

            var leftAccessCheckExpression = GetMemberAccessCheckExpression(expression.Left);
            var rightAccessCheckExpression = GetMemberAccessCheckExpression(expression.Right);
            
            if (rightAccessCheckExpression != null)
                expression = Expression.MakeBinary(ExpressionType.AndAlso, rightAccessCheckExpression, expression);

            if (leftAccessCheckExpression != null)
                expression = Expression.MakeBinary(ExpressionType.AndAlso, leftAccessCheckExpression, expression);

            return expression;
        }

        private static Expression RewriteConditionalExpression(ConditionalExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteConditionalExpression)} as it is null");

            var test = RewriteExpression(expression.Test);
            var ifTrue = RewriteExpression(expression.IfTrue);
            var ifFalse = RewriteExpression(expression.IfFalse);
            
            return Expression.Condition(test, ifTrue, ifFalse, expression.Type);
        }

        /// <summary>
        /// This is to rewrite "x.L1.Name" -> "x != null && x.L1 != null"
        /// </summary>
        private static Expression GetMemberAccessCheckExpression(Expression expression)
        {
            var memberAccess = expression as MemberExpression;

            if (memberAccess != null)
                return GetBinaryMemberAccessCheckExpression(memberAccess);

            var binaryExpr = expression as BinaryExpression;

            if (binaryExpr != null)
            {
                var left = GetMemberAccessCheckExpression(binaryExpr.Left);
                var right = GetMemberAccessCheckExpression(binaryExpr.Right);

                if (left == null && right == null)
                    return null;
                if (left == null)
                    return right;
                if (right == null)
                    return left;

                return Expression.MakeBinary(ExpressionType.AndAlso, left, right);
            }

            var unaryExpression = expression as UnaryExpression;

            if (unaryExpression != null)
                return GetMemberAccessCheckExpression(unaryExpression.Operand);

            var methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression != null)
                return GetMethodCallMemberAccessCheckExpression(methodCallExpression);


            return null;
        }

        private static Expression GetMethodCallMemberAccessCheckExpression(MethodCallExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(GetMethodCallMemberAccessCheckExpression)} as it is null");

            var objectCheckList = GetListOfMemberAccesExpressions(expression.Object as MemberExpression);
            var argumentChecklist = expression.Arguments.Select(GetMemberAccessCheckExpression)
                .Where(x => x != null)
                .ToList();
            
            Expression expr = null;

            if (objectCheckList != null && objectCheckList.Count > 0)
                foreach (var parent in objectCheckList)
                {
                    if (parent.Type.IsValueType)
                        continue;

                    var isNullExpression = Expression.MakeBinary(ExpressionType.NotEqual, parent, Expression.Constant(null, parent.Type));

                    expr = expr == null
                        ? isNullExpression
                        : Expression.MakeBinary(ExpressionType.AndAlso, isNullExpression, expr);
                }

            if(argumentChecklist.Count > 0)
                foreach (var parent in argumentChecklist)
                {

                    expr = expr == null
                        ? parent
                        : Expression.MakeBinary(ExpressionType.AndAlso, parent, expr);
                }

            return expr;
        }


        /// <summary>
        /// This is to rewrite "x.L1.Name" -> "x != null && x.L1 != null"
        /// </summary>
        private static Expression GetBinaryMemberAccessCheckExpression(MemberExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(GetBinaryMemberAccessCheckExpression)} as it is null");

            var memberCheckList = GetListOfMemberAccesExpressions(expression);

            Expression expr = null;

            foreach (var parent in memberCheckList.Skip(1))
            {
                if(parent.Type.IsValueType)
                    continue;
                
                var isNullExpression = Expression.MakeBinary(ExpressionType.NotEqual, parent, Expression.Constant(null, parent.Type));

                expr = expr == null 
                    ? isNullExpression 
                    : Expression.MakeBinary(ExpressionType.AndAlso, isNullExpression, expr);
            }

            return expr;
        }

        private static List<Expression> GetListOfMemberAccesExpressions(MemberExpression expression)
        {
            var memberExpressions = GetListOfMemberExpressions(expression);

            if (memberExpressions == null || memberExpressions.Count == 0)
                return null;
            
            // Get all members that need to be checked
            var memberCheckList = memberExpressions.Cast<Expression>().ToList();

            // Ensure Last expression is a paramater
            var parameter = memberExpressions.Last()?.Expression as ParameterExpression;

            if(parameter!= null) 
                memberCheckList.Add(parameter);
                    
            return memberCheckList;
        }

        private static Expression RewriteComparisonBinaryExpression(BinaryExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteComparisonBinaryExpression)} as it is null");

            Expression leftAccessCheckExpression = null;
            Expression rightAccessCheckExpression = null;

            if (expression.Left.NodeType == ExpressionType.MemberAccess)
                leftAccessCheckExpression = GetBinaryMemberAccessCheckExpression(expression.Left as MemberExpression);

            if(expression.Right.NodeType == ExpressionType.MemberAccess)
                rightAccessCheckExpression = GetBinaryMemberAccessCheckExpression(expression.Right as MemberExpression);

            if (rightAccessCheckExpression != null)
                expression = Expression.MakeBinary(ExpressionType.AndAlso, rightAccessCheckExpression, expression,
                    expression.IsLiftedToNull, expression.Method);

            if(leftAccessCheckExpression != null)
                expression = Expression.MakeBinary(ExpressionType.AndAlso, leftAccessCheckExpression, expression);

            return expression;
        }

        private static Expression RewriteNewExpression(NewExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteNewExpression)} as it is null");

            var newArgs = expression.Arguments.Select(RewriteExpression).ToList();

            var newExpression =  Expression.New(expression.Constructor, newArgs);

            return newExpression;
        }

        private static Expression RewriteMemberExpression(MemberExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), $"Cannot {nameof(RewriteMemberExpression)} as it is null");

            var memberCheckList = GetListOfMemberAccesExpressions(expression);

            // Crate IIF Expressions starting in midle
            var value = memberCheckList.First();
            foreach (var parent in memberCheckList.Skip(1).ToList())
            {
                var isNullExpression = Expression.MakeBinary(ExpressionType.Equal, parent, Expression.Constant(null));

                value = Expression.Condition(isNullExpression, GetDefaultConstantExpression(value), value);
            }

            return value;
        }

        private static ConstantExpression GetDefaultConstantExpression(Expression value)
        {
            return Expression.Constant(GetDefault(value.Type), value.Type);
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        private static List<MemberExpression> GetListOfMemberExpressions(MemberExpression expression)
        {
            var memberExpressions = new List<MemberExpression>();

            while (expression != null && expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpressions.Add(expression);

                expression = expression.Expression as MemberExpression;
            }
            return memberExpressions;
        }

        public static Expression<Func<TIn, TOut>> RebuildExpressStronglyTypes<TIn, TOut>(Expression<Func<TIn, TOut>> original)
        {
            return RewriteLambdaExpression(original) as Expression<Func<TIn, TOut>>;
        }
    }
}
