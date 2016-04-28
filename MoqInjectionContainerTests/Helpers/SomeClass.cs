namespace MoqInjectionContainerTests.Helpers
{
    public class SomeClass
    {
        private readonly IDepencyA _a;
        private readonly IDepencyB _b;
        public IMockSetup Mock { get; set; }

        public SomeClass(IDepencyA a, IDepencyB b, IMockSetup mock)
        {
            _a = a;
            _b = b;
            Mock = mock;
        }

        public void CallA()
        {
            _a.Call();
        }

        public void CallB()
        {
            _b.Call();
        }
    }
}