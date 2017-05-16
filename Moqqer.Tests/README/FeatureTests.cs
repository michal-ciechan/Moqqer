using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using FluentAssertions;
using Moq;
using MoqqerNamespace.Extensions;
using MoqqerNamespace.MoqqerQueryable;
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

            var classObject = subject.Ctor;
            var moqedObject = _moq.Object<ClassWithParameterlessCtor>();

            classObject.Should().BeSameAs(moqedObject);
        }

        [Test]
        public void ConcreteClassInject_AutoGenrate_True()
        {
            var mock = _moq
                .Create<ClassWithCtorContainingClassWithoutParameterlessCtor>(autogenerate: true);

            mock.CtorParam.A.Should().NotBeNull();
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

            // Allow you to set a default value for reference types
            _moq.Use("GitHub");
            _moq.Create<StringCtor>().Text.Should().Be("GitHub");
        }

        [Test]
        public void DefaultQuryableImplementation()
        {
            var item = new Leaf(25);

            _moq.List<Leaf>()
                .Add(item);

            // Contains `IQueryable Leaves { get; set; }`
            var ctx = _moq.Of<IContext>().Object;

            ctx.Leaves.Should().HaveCount(1);
            ctx.Leaves.First().Should().BeSameAs(item);
            ctx.Leaves.Should().BeEquivalentTo(_moq.List<Leaf>());
            ctx.Leaves.Where(x => x.Age == 25)
                .Should().HaveCount(1);
        }

        [Test]
        public void DefaultQueryableImplementation()
        {
            var list = _moq.Object<List<string>>();

            list.Add("Test");
            list.Add("SomeRandomString");

            var res = _moq.Create<ClassWithQueryableInCtor>();

            res.StringQueryable.ToList()
                .Should().BeEquivalentTo(_moq.Object<List<string>>());
        }


        [Test]
        public void DefaultQueryableImplementation_ExpressionNullGuarding()
        {
            _moq.Add(new Parent());

            var ctx = _moq.Of<IContext>().Object; // Get a mocked context

            ctx.Parents.Select(x => x.Name) // Standard accessor
                .Single().Should().Be(null);

            ctx.Parents.Select(x => (int?) x.Child.Age) // Linq2Objects can throw NullReferenceException
                .Single().Should().Be(null);
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void DefaultQueryableImplementation_ExpressionNullGuarding_TurnOff()
        {
            _moq.UseMoqqerEnumerableQuery = false;

            _moq.Add(new Parent());

            var ctx = _moq.Of<IContext>().Object; // Get a mocked context
            
            Action act = () => ctx.Parents.Select(x => (int?)x.Child.Age).Single();

            act.ShouldThrow<NullReferenceException>("x.Child is null");
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void DefaultQueryableImplementation_AsMoqqerQueryablef()
        {
            var list = new List<Parent>
            {
                new Parent()
            };

            var queryable = list.AsMoqqerQueryable();

            // list.AsQueryable() would throw a NullReferenceException as `Child` is `null`
            var name = queryable.FirstOrDefault(x => x.Child.Name == "Test");

            name.Should().Be(null);
        }


        [Test]
        [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
        public void FuncResolution()
        {
            var factory = _moq.Create<LeafFactory>();

            Func<ILeaf> func = factory.CreateLeaf;

            var leaf = func();

            leaf.Should().BeSameAs(_moq.Of<ILeaf>().Object);
        }

        [Test]
        [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
        public void List()
        {
            var item = new Leaf(25);

            // Get instance of List<T>
            _moq.List<Leaf>()
                .Add(item);

            // Extension method to add T item to List<T>
            _moq.Add(item);

            // Confirm List has 2 Items
            _moq.Of<IContext>()
                .Object.Leaves.Should().HaveCount(2);
        }

        [Test]
        public void FactoryMethod()
        {
            // Create custom leaf
            var customLeaf = new Leaf(25);
            
            // Register ILeaf factory function
            _moq.Factory<ILeaf>(context =>
                context.CallType == CallType.Constructor
                    ? customLeaf // For Ctor injection return custom leaf
                    : context.Default // Otherwise return default Mock
            );
            
            // Get instance of Branch (has ILeaf in Ctor)
            var branch = _moq.Create<Branch>();

            // Its ILeaf should be same as custom leaf
            branch.Leaf.Should().BeSameAs(customLeaf);
            branch.Leaf.Age.Should().Be(25);
            
            // Get an instance of tree
            var tree = _moq.Create<Tree>();

            // Indirect (non Ctor injection) should be default mock
            tree.Branch.Leaf.Should().NotBeSameAs(customLeaf);

            tree.Branch.Leaf.Should().BeSameAs(_moq.Of<ILeaf>().Object);
        }
    }
}
