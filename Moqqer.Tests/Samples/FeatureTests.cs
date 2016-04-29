using FluentAssertions;
using Moq;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;
// ReSharper disable once RedundantUsingDirective

namespace MoqqerNamespace.Tests.Samples
{
    [TestFixture]
    public class FeatureTests
    {
        private Moqqer _moq;

        // Setup run per test.
        [SetUp]
        public void A_TestInit()
        {
            // Create instance of Moqqer
            _moq = new Moqqer();

            // Create an instance of SomeClass injecting mocks into constructor

        }

        [Test]
        public void ConstrcutorInejction()
        {
            var subject = _moq.Create<SomeClass>();

            var depencyA = _moq.Of<IDepencyA>().Object;
            var depencyB = _moq.Of<IDepencyB>().Object;
            var expected = _moq.Of<IMockSetup>().Object;

            subject.A.Should().BeSameAs(depencyA);
            subject.B.Should().BeSameAs(depencyB);
            subject.Mock.Should().BeSameAs(expected);
        }

        [Test]
        public void DefaultObjectInjection()
        {
            var subject = _moq.Create<StringConstructorClass>();

            subject.Text.Should().Be(string.Empty);
        }

        [Test]
        public void RecursiveMocking()
        {
            var root = _moq.Create<Root>();

            root.Tree.Branch.Leaf.Grow();

            _moq.Of<ILeaf>().Verify(x => x.Grow(), Times.Once);
        }

        [Test]
        public void LazyMocking()
        {
            var root = _moq.Create<Root>();

            _moq.Mocks.Should().NotContainKey(typeof(IBranch));

            root.Tree.Branch.Leaf.Grow();

            _moq.Mocks.Should().ContainKey(typeof(IBranch));
        }

        [Test]
        public void ConcreteClassInjection()
        {
            var subject = _moq.Create<ClassHavingParameterlessConcreteClass>();

            var classObject = subject.Class;
            var moqedObject = _moq.Object<ParameterlessClass>();

            classObject.Should().BeSameAs(moqedObject);
        }
    }
}
