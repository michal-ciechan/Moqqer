using System.Collections.Generic;
using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface ISubInterface
    {
        string SubString();
        IQueryable<string> SubQueryable();
        List<string> SubList();
    }
}