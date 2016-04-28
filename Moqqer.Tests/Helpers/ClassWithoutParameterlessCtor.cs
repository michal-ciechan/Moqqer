namespace Moqqer.Namespace.Tests.Helpers
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