namespace MoqqerNamespace.Tests.TestClasses
{
    public class SomeClass
    {
        public IDepencyA A { get; }
        public IDepencyB B { get; }

        public SomeClass(IDepencyA a, IDepencyB b, IMockSetup mock)
        {
            A = a;
            B = b;
            Mock = mock;
        }

        public IMockSetup Mock { get; set; }

        public virtual void CallA()
        {
            A.Call();
        }

        public virtual void CallB()
        {
            B.Call();
        }
    }
}