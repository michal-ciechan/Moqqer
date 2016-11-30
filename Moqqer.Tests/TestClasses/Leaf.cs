using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class Leaf : ILeaf
    {
        public Leaf(int age)
        {
            Age = age;
        }

        public Leaf()
        {
            
        }

        public int Age { get;set; }
        public void Grow()
        {
            throw new Exception();
        }
    }
}