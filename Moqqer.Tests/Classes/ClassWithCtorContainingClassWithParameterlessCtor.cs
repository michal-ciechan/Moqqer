namespace Moqqer.Namespace.Tests.Classes
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