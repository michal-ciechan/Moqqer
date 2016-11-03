using System.Linq;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithQueryableInCtor
    {
        public IQueryable<string> StringQueryable { get; }

        public ClassWithQueryableInCtor(IQueryable<string> stringQueryable)
        {
            StringQueryable = stringQueryable;
        }
    }
}