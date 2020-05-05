using System.Linq;
using System.Threading.Tasks;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IInterfaceWithGenericMethod
    {
        IQueryable<T> Queryable<T>();
        Task<IQueryable<T>> QueryableAsync<T>();
        IQueryable<T> Query<T>();
    }
}