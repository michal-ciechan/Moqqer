namespace MoqqerNamespace.Tests.TestClasses
{
    class Branch : IBranch
    {
        public Branch(int numberOfLeaves)
        {
            NumberOfLeaves = numberOfLeaves;
        }
        public int NumberOfLeaves { get; }
        public ILeaf Leaf { get; }
    }
}