using System.Collections.Generic;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithListInCtor
    {
        public List<string> StringList { get; }
        public IList<string> StringListInterface { get; }
        public IEnumerable<string> StringEnumerable { get; }
        public ICollection<string> StringCollection { get; }

        public ClassWithListInCtor(List<string> stringList, IList<string> stringListInterface, IEnumerable<string> stringEnumerable, ICollection<string> stringCollection)
        {
            StringList = stringList;
            StringListInterface = stringListInterface;
            StringEnumerable = stringEnumerable;
            StringCollection = stringCollection;
        }
    }
}