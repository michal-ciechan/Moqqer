namespace MoqqerNamespace.Tests.TestClasses
{
    class Branch : IBranch
    {
        public Branch(ILeaf leaf)
        {
            Leaf = leaf;
        }
        public int NumberOfLeaves { get; }
        public ILeaf Leaf { get; }
        public ILeaf GetLeaf()
        {
            return Leaf;
        }
        public ILeaf GetLeaf(int arg)
        {
            return Leaf;
        }
    }
}