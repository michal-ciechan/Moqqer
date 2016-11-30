using System.Collections.Generic;
using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IOpenGenericMethodsWithClosedGenericListReturnTypes
    {
        IEnumerable<object> GetEnumerable<T>();
        IList<object> GetIList<T>();
        List<object> GetList<T>();
        IQueryable<object> GetIQueryable<T>();
    }
}