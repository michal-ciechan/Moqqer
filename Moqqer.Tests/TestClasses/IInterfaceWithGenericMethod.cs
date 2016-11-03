using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IInterfaceWithGenericMethod
    {
        IQueryable<T> Queryable<T>();
        IQueryable<T> Query<T>();
    }
}