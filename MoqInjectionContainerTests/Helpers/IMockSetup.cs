using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoqInjectionContainerTests.Helpers
{
    public interface IMockSetup
    {
        IDepencyA GetA();
        IDepencyB GetB(string test);
        SomeClass GetClass();
    }
}
