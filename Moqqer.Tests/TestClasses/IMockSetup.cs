using System;
using System.Threading.Tasks;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IMockSetup
    {
        IDepencyA GetA();
        IDepencyB GetB(string test);
        SomeClass GetClass();
        Tuple<string, int> Get2TypeTuple();
        Task<Tuple<string, int>> Get2TypeTupleTask();
    }
}