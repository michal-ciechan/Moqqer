namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithCtorParamsOf<T1, T2>
    {
        public T1 Parameter1 { get; }
        public T2 Parameter2 { get; }

        public ClassWithCtorParamsOf(T1 parameter1, T2 parameter2)
        {
            Parameter1 = parameter1;
            Parameter2 = parameter2;
        }
    }
}