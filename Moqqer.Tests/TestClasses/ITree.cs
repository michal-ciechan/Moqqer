using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface ITree
    {
        IBranch Branch { get; }
    }

    public class LeafFactory
    {
        public Func<ILeaf> CreateLeaf { get; }

        public LeafFactory(Func<ILeaf> createLeaf)
        {
            CreateLeaf = createLeaf;
        }
    }
}