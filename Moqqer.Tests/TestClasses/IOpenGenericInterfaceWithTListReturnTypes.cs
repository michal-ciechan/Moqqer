using System.Collections.Generic;
using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IOpenGenericInterfaceWithTListReturnTypes<T>
    {
        IQueryable<T> GetIQueryable();
        IEnumerable<T> GetEnumerable();
        IList<T> GetIList();
        List<T> GetList();
    }
}