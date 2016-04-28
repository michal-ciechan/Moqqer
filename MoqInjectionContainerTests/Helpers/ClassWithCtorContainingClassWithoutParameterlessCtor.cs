namespace MoqInjectionContainerTests.Helpers
{
    public class ClassWithCtorContainingClassWithoutParameterlessCtor
    {
        public ClassWithParameterlessCtor CtorParam { get; set; }

        public ClassWithCtorContainingClassWithoutParameterlessCtor(ClassWithParameterlessCtor ctorParam)
        {
            CtorParam = ctorParam;
        }
    }
}