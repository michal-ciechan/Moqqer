using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MoqqerNamespace.MoqqerQueryable
{
    public class EnumerableMoqqerQuery<T> : EnumerableMoqqerQuery, IOrderedQueryable<T>, IQueryProvider
    {
        readonly Expression _expression;
        IEnumerable<T> _enumerable;

        IQueryProvider IQueryable.Provider => this;

        // Must remain public for Silverlight
        public EnumerableMoqqerQuery(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
            _expression = Expression.Constant(this);
        }

        // Must remain public for Silverlight
        public EnumerableMoqqerQuery(Expression expression)
        {
            _expression = expression;
        }

        internal override Expression Expression => _expression;

        internal override IEnumerable Enumerable => _enumerable;

        Expression IQueryable.Expression => _expression;

        Type IQueryable.ElementType => typeof(T);

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var iqType = TypeHelper.FindGenericType(typeof(IQueryable<>), expression.Type);

            if (iqType == null)
                throw new ArgumentOutOfRangeException(nameof(expression));

            var res = MoqqerExpressionRewriter.RewriteLinqCall(expression);

            return Create(iqType.GetGenericArguments()[0], res);
        }

        IQueryable<TS> IQueryProvider.CreateQuery<TS>(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!typeof(IQueryable<TS>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }

            var res = MoqqerExpressionRewriter.RewriteLinqCall(expression);

            return new EnumerableMoqqerQuery<TS>(res);
        }

        // Baselining as Safe for Mix demo so that interface can be transparent. Marking this
        // critical (which was the original annotation when porting to silverlight) would violate
        // fxcop security rules if the interface isn't also critical. However, transparent code
        // can't access this anyway for Mix since we're not exposing AsQueryable().
        // [....]: the above assertion no longer holds. Now making AsQueryable() public again
        // the security fallout of which will need to be re-examined.
        object IQueryProvider.Execute(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return EnumerableExecutor.Create(expression).ExecuteBoxed();
        }

        // see above
        TS IQueryProvider.Execute<TS>(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (!typeof(TS).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException(nameof(expression));
            return new EnumerableExecutor<TS>(expression).Execute();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> GetEnumerator()
        {
            if (_enumerable == null)
            {
                var rewriter = new EnumerableRewriter();
                var body = rewriter.Visit(_expression);

                var f = Expression.Lambda<Func<IEnumerable<T>>>(body, (IEnumerable<ParameterExpression>)null);

                _enumerable = f.Compile()();
            }
            return _enumerable.GetEnumerator();
        }

        public override string ToString()
        {
            var c = _expression as ConstantExpression;
            if (c != null && c.Value == this)
            {
                if (_enumerable != null)
                    return _enumerable.ToString();
                return "null";
            }
            return _expression.ToString();
        }
    }

    public abstract class EnumerableMoqqerQuery
    {
        internal abstract Expression Expression { get; }
        internal abstract IEnumerable Enumerable { get; }
        internal static IQueryable Create(Type elementType, IEnumerable sequence)
        {
            var seqType = typeof(EnumerableMoqqerQuery<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(seqType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { sequence }, null);
        }

        internal static IQueryable Create(Type elementType, Expression expression)
        {
            var seqType = typeof(EnumerableMoqqerQuery<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(seqType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { expression }, null);
        }
    }
}