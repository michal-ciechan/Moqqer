namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWith2Ctors1ContainingClassWithoutParameterlessCtor
    {
        public ClassWithoutParameterlessCtor ParameterlessCtorParam { get; set; }
        public IBranch InterfaceParam { get; }

        public ClassWith2Ctors1ContainingClassWithoutParameterlessCtor(
            IBranch interfaceParam)
        {
            InterfaceParam = interfaceParam;
        }

        public ClassWith2Ctors1ContainingClassWithoutParameterlessCtor(
            IBranch interfaceParam, ClassWithoutParameterlessCtor parameterlessCtorParam)
        {
            InterfaceParam = interfaceParam;
            ParameterlessCtorParam = parameterlessCtorParam;
        }
    }

    public class ClassWith2Ctors1ContainingString
    {
        public string String { get; set; }
        public IBranch InterfaceParam { get; }

        public ClassWith2Ctors1ContainingString(
            IBranch interfaceParam)
        {
            InterfaceParam = interfaceParam;
        }

        public ClassWith2Ctors1ContainingString(
            IBranch interfaceParam, string str)
        {
            InterfaceParam = interfaceParam;
            String = str;
        }
    }
}