namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithCtorContainingClassWithoutParameterlessCtor
    {
        public ClassWithCtorContainingClassWithoutParameterlessCtor(ClassWithParameterlessCtor ctorParam)
        {
            CtorParam = ctorParam;
        }

        public ClassWithParameterlessCtor CtorParam { get; set; }
    }
}