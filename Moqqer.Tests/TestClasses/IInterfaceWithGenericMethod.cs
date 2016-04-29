using System.Linq;

namespace Moqqer.Namespace.Tests.TestClasses
{
    public interface IInterfaceWithGenericMethod
    {
        IQueryable<T> Queryable<T>();
    }
}