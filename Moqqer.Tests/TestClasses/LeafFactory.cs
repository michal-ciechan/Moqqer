using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class LeafFactory
    {
        public Func<ILeaf> CreateLeaf { get; }

        public LeafFactory(Func<ILeaf> createLeaf)
        {
            CreateLeaf = createLeaf;
        }
    }
}