namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWitIInterfaceWithGenericMethodParam
    {
        public ClassWitIInterfaceWithGenericMethodParam(IInterfaceWithGenericMethod ctorParam)
        {
            CtorParam = ctorParam;
        }

        public IInterfaceWithGenericMethod CtorParam { get; set; }
    }
}