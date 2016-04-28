namespace Moqqer.Namespace.Tests.Helpers
{
    public class ClassWithCtorContainingClassWithParameterlessCtor
    {
        public ClassWithCtorContainingClassWithParameterlessCtor(ClassWithParameterlessCtor ctorParam)
        {
            CtorParam = ctorParam;
        }

        public ClassWithParameterlessCtor CtorParam { get; set; }
    }
}