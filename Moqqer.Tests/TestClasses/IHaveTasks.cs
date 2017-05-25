using System;
using System.Threading.Tasks;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IHaveTasks
    {
        Task SimpleTask();

        Task<string> StringTask();
        Task<ILeaf> LeafTask();
        Task<Tuple<string>> TupleTask();
    }
}