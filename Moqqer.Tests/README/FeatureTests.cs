using FluentAssertions;
using Moq;
using MoqqerNamespace.Extensions;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;

// ReSharper disable once RedundantUsingDirective

namespace MoqqerNamespace.Tests.README
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
            var subject = _moq.Create<StringCtor>();

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

        [Test]
        public void QuickerVerification()
        {
            // Quickly Verify that a mock member was never called
            _moq.Verify<ILeaf>(x => x.Grow()).Never();

            // Or that it was called once
            _moq.Of<ILeaf>().Object.Grow();
            _moq.Verify<ILeaf>(x => x.Grow()).Once();

            // Or called X number of times
            _moq.Of<ILeaf>().Object.Grow();
            _moq.Verify<ILeaf>(x => x.Grow()).Times(2);

            // Or fallback to using the Moq.Times class
            _moq.Of<ILeaf>().Object.Grow();
            _moq.Verify<ILeaf>(x => x.Grow()).Times(Times.AtLeast(3));
            _moq.Verify<ILeaf>(x => x.Grow()).Times(Times.Between(3,7, Range.Inclusive));
        }

        [Test]
        public void ConcreteImplementation()
        {
            // Create your concrete implementation
            var fizz = new Fizz(3);
            var buzz = new Buzz(5);

            // Set it as the implementation of an interface
            _moq.Use<IFizz>(fizz);
            _moq.Create<FizzBuzzGame>().Fizz.Should().BeSameAs(fizz);
            
            // Or use for all interfaces
            _moq.Use(buzz).ForAllImplementedInterfaces();
            _moq.Create<FizzBuzzGame>().Buzz.Should().BeSameAs(buzz);

            // Allow you to set a default value for value types
            _moq.Use(25);
            _moq.Create<Fizz>().Divisor.Should().Be(25);

            // Allow you to set a default value as well as reference types
            _moq.Use("GitHub");
            _moq.Create<StringCtor>().Text.Should().Be("GitHub");
        }
    }
}
