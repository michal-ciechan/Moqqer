namespace Moqqer.Namespace.Tests.Classes
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

        public void CallA()
        {
            A.Call();
        }

        public void CallB()
        {
            B.Call();
        }
    }

    public class Root : IRoot
    {
        public ITree Tree { get; }

        public Root(ITree tree)
        {
            Tree = tree;
        }

        public void Water()
        {
            Tree.Branch.Leaf.Grow();
        }
    }

    public interface IRoot
    {
        ITree Tree { get; }
    }

    public interface ITree
    {
        IBranch Branch { get; }
    }

    public interface IBranch
    {
        ILeaf Leaf { get; }
    }

    public interface ILeaf
    {
        void Grow();
    }
}