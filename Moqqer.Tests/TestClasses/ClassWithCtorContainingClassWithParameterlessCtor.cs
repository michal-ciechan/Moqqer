namespace MoqqerNamespace.Tests.TestClasses
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