namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithCtorContainingIInterfaceWithOutAndRefParamMethods
    {
        public ClassWithCtorContainingIInterfaceWithOutAndRefParamMethods(IInterfaceWithOutAndRefParamMethods ctorParam)
        {
            CtorParam = ctorParam;
        }

        public IInterfaceWithOutAndRefParamMethods CtorParam { get; set; }
    }
}