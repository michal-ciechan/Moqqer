namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithCtorParamOf<T>
    {
        public T Parameter { get; }

        public ClassWithCtorParamOf(T parameter)
        {
            Parameter = parameter;
        }
    }
}