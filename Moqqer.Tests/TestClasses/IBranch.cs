using System.Security.Cryptography.X509Certificates;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IBranch
    {
        int NumberOfLeaves { get; }
        ILeaf Leaf { get; }
        Leaf ConcreteLeaf { get; }
        ILeaf GetLeaf();
        ILeaf GetLeaf(int arg);
    }
}