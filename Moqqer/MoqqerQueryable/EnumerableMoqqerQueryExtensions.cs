using System.Collections.Generic;
using System.Linq;

namespace MoqqerNamespace.MoqqerQueryable
{
    public static class EnumerableMoqqerQueryExtensions
    {
        public static IQueryable<T> AsMoqqerQueryable<T>(this IEnumerable<T> source)
        {
            return new EnumerableMoqqerQuery<T>(source);
        }
    }
}