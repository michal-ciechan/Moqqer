namespace Moqqer.Namespace.Tests.Helpers
{
    public class SomeClass
    {
        private readonly IDepencyA _a;
        private readonly IDepencyB _b;

        public SomeClass(IDepencyA a, IDepencyB b, IMockSetup mock)
        {
            _a = a;
            _b = b;
            Mock = mock;
        }

        public IMockSetup Mock { get; set; }

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