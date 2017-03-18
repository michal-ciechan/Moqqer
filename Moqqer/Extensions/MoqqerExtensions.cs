using System;
using System.Linq.Expressions;

namespace MoqqerNamespace.Extensions
{
    public static class MoqqerExtensions
    {
        public static MoqFluentVerifyBuilder<T>.IHasVerifyAction Verify<T>
            (this Moqqer moq, Expression<Action<T>> action) 
            where T : class
        {
            return new MoqFluentVerifyBuilder<T>(moq.Of<T>(), action);
        }

        public static void Add<T>(this Moqqer moq, T item)
        {
            moq.List<T>().Add(item);
        }
    }
}
