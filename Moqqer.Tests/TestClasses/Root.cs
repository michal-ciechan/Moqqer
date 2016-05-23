namespace MoqqerNamespace.Tests.TestClasses
{
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
}