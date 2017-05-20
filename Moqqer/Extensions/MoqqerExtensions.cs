using System;
using System.Collections;
using System.Collections.Generic;
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

        public static void Add<T>(this Moqqer moq, params T[] items)
        {
            foreach (var item in items)
                moq.Add(item);
        }

        public static void AddItems<T>(this Moqqer moq, IEnumerable<T> items)
        {
            foreach (var item in items)
                moq.Add(item);
        }
    }
}
