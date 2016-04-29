using System.Linq;

namespace Moqqer.Namespace.Tests.Classes
{
    public interface IInterfaceWithGenericMethod
    {
        IQueryable<T> Queryable<T>();
    }
}