namespace Moqqer.Namespace.Tests.Classes
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