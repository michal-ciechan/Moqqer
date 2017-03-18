using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IContext
    {
        IQueryable<Leaf> Leaves { get; }
        IQueryable<Parent> Parents{ get; }
    }
}