namespace MoqInjectionContainerTests.Helpers
{
    public class ClassWithCtorContainingClassWithParameterlessCtor
    {
        public ClassWithParameterlessCtor CtorParam { get; set; }

        public ClassWithCtorContainingClassWithParameterlessCtor(ClassWithParameterlessCtor ctorParam)
        {
            CtorParam = ctorParam;
        }
    }
}