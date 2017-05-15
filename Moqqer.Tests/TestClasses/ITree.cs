namespace MoqqerNamespace.Tests.TestClasses
{
    public interface ITree
    {
        IBranch Branch { get; }
        IBranch GetBranch();
    }

    class Tree : ITree
    {
        public Tree(IBranch branch)
        {
            Branch = branch;
        }

        public IBranch Branch { get; }
        public IBranch GetBranch()
        {
            return Branch;
        }
    }
}