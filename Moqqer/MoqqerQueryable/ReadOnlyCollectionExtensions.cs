using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MoqqerNamespace.MoqqerQueryable
{
    internal static class ReadOnlyCollectionExtensions
    {
        internal static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
                return DefaultReadOnlyCollection<T>.Empty;
            var col = sequence as ReadOnlyCollection<T>;
            if (col != null)
                return col;
            return new ReadOnlyCollection<T>(sequence.ToArray());
        }
        private static class DefaultReadOnlyCollection<T>
        {
            private static volatile ReadOnlyCollection<T> _defaultCollection;
            internal static ReadOnlyCollection<T> Empty => _defaultCollection ?? (_defaultCollection = new ReadOnlyCollection<T>(new T[] { }));
        }
    }
}