using System.Linq;

namespace Moqqer.Namespace.Tests.Helpers
{
    public interface IInterfaceWithGenericMethod
    {
        IQueryable<T> Queryable<T>();
    }
}