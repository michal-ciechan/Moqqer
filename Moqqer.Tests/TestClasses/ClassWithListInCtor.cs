using System.Collections.Generic;
using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithListInCtor
    {
        public List<string> StringList { get; }
        public IList<string> StringListInterface { get; }
        public IEnumerable<string> StringEnumerable { get; }
        public ICollection<string> StringCollection { get; }
        public IQueryable<string> StringQueryable { get; }

        public ClassWithListInCtor(
            List<string> stringList, 
            IList<string> stringListInterface, 
            IEnumerable<string> stringEnumerable, 
            ICollection<string> stringCollection, 
            IQueryable<string> stringQueryable)
        {
            StringList = stringList;
            StringListInterface = stringListInterface;
            StringEnumerable = stringEnumerable;
            StringCollection = stringCollection;
            StringQueryable = stringQueryable;
        }
    }
}