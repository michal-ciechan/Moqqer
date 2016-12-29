using System.Collections.Generic;
using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface ISuperInterface : ISubInterface
    {
        string SuperString();
        IQueryable<string> SuperQueryable();
        List<string> SuperList();
    }
}