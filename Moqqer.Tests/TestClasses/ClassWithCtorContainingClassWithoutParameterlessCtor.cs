namespace Moqqer.Namespace.Tests.TestClasses
{
    public class ClassWithCtorContainingClassWithoutParameterlessCtor
    {
        public ClassWithCtorContainingClassWithoutParameterlessCtor(ParameterlessClass ctorParam)
        {
            CtorParam = ctorParam;
        }

        public ParameterlessClass CtorParam { get; set; }
    }
}