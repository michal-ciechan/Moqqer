using System;
using System.Linq;
using System.Text;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithDefaultMethods
    {
        public IDefaultMethods DefaultMethods { get; }

        public ClassWithDefaultMethods(IDefaultMethods defaultMethods)
        {
            DefaultMethods = defaultMethods;
        }
    }
}
