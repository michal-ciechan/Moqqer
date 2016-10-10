using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class Leaf : ILeaf
    {
        public void Grow()
        {
            throw new Exception();
        }
    }
}