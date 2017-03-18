using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MoqqerNamespace.MoqqerQueryable
{
    public abstract class EnumerableExecutor
    {
        internal abstract object ExecuteBoxed();

        internal static EnumerableExecutor Create(Expression expression)
        {
            var execType = typeof(EnumerableExecutor<>).MakeGenericType(expression.Type);
            return (EnumerableExecutor)Activator.CreateInstance(execType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { expression }, null);
        }
    }
    public class EnumerableExecutor<T> : EnumerableExecutor
    {
        readonly Expression _expression;
        Func<T> _func;

        // Must remain public for Silverlight
        public EnumerableExecutor(Expression expression)
        {
            _expression = expression;
        }

        internal override object ExecuteBoxed()
        {
            return Execute();
        }

        internal T Execute()
        {
            if (_func != null)
                return _func();

            var rewriter = new EnumerableRewriter();
            var body = rewriter.Visit(_expression);
            var f = Expression.Lambda<Func<T>>(body, (IEnumerable<ParameterExpression>)null);
            _func = f.Compile();

            return _func();
        }
    }
}