namespace MoqInjectionContainerTests.Helpers
{
    public class ClassWithoutParameterlessCtor
    {
        public IDepencyA A { get; set; }

        public ClassWithoutParameterlessCtor(IDepencyA a)
        {
            A = a;
        }
    }
}