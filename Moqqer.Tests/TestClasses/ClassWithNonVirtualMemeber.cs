using System.Collections.Generic;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithNonVirtualMemeber
    {
        public IList<string> ListOfStrings => new List<string>();
    }
}