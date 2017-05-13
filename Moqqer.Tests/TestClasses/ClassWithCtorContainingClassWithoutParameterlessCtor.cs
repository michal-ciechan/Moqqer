namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithCtorContainingClassWithoutParameterlessCtor
    {
        public ClassWithCtorContainingClassWithoutParameterlessCtor(ClassWithoutParameterlessCtor ctorParam)
        {
            CtorParam = ctorParam;
        }

        public ClassWithoutParameterlessCtor CtorParam { get; set; }
    }
}