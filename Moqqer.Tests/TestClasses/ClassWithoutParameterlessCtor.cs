namespace Moqqer.Namespace.Tests.TestClasses
{
    public class ClassWithoutParameterlessCtor
    {
        public ClassWithoutParameterlessCtor(IDepencyA a)
        {
            A = a;
        }

        public IDepencyA A { get; set; }
    }
}