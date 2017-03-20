using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Moq;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;

namespace MoqqerNamespace.Tests
{
    [TestFixture]
    public class MoqqerTests
    {
        [SetUp]
        public void A_Setup()
        {
            _moq = new Moqqer();
        }

        private Moqqer _moq;

        [Test]
        public void CallA_CallsDependencyA()
        {
            var subject = _moq.Create<SomeClass>();

            subject.CallA();

            _moq.Of<IDepencyA>().Verify(x => x.Call(), Times.Once);
        }

        /// <summary>
        ///     Issue #1
        /// </summary>
        [Test]
        public void Create_ClassWith2Ctors1ContainingClassWithoutParameterlessCtor_ShouldReturnObject()
        {
            var res = _moq.Create<ClassWith2Ctors1ContainingClassWithoutParameterlessCtor>();

            res.Should().NotBeNull();
            res.InterfaceParam.Should().NotBeNull();
            res.ParameterlessCtorParam.Should().BeNull();
        }

        /// <summary>
        ///     Issue #1
        /// </summary>
        [Test]
        public void Create_ClassWith2Ctors1ContainingString_ShouldReturn()
        {
            var res = _moq.Create<ClassWith2Ctors1ContainingString>();

            res.InterfaceParam.Should().NotBeNull();
            res.String.Should().BeSameAs(string.Empty);
        }

        /// <summary>
        ///     Issue #2 - GetInstance should return Default if exists.
        /// </summary>
        [Test]
        public void Create_ClassWithDefaultMethods_IListIsDefaultObjectsList()
        {
            var res = _moq.Create<ClassWithDefaultMethods>();

            res.DefaultMethods.ListInterface()
                .Should().BeSameAs(_moq.Object<List<string>>());
        }

        /// <summary>
        ///     Issue #2
        /// </summary>
        [Test]
        public void Create_ClassWithDefaultMethods_ListIsDefaultObjectsList()
        {
            var res = _moq.Create<ClassWithDefaultMethods>();

            res.DefaultMethods.List()
                .Should().BeSameAs(_moq.Object<List<string>>());
        }

        /// <summary>
        ///     Issue #15
        /// </summary>
        [Test]
        public void Of_IHaveTasks_Mocks_SimpleTask_Method_ReturnsCompletedTask()
        {
            var res = _moq.Of<IHaveTasks>().Object;

            var task = res.SimpleTask();

            task.Should().NotBeNull();
            task.IsCompleted.Should().BeTrue();
        }

        /// <summary>
        ///     Issue #15
        /// </summary>
        [Test]
        public void Of_IHaveTasks_Mocks_StringTask_Method_ReturnsCompletedTask_WithDefaultString()
        {
            var res = _moq.Of<IHaveTasks>().Object;

            var task = res.StringTask();

            task.Should().NotBeNull();
            task.IsCompleted.Should().BeTrue();

            task.Result.Should().Be(_moq.Object<string>());
        }
        /// <summary>
        ///     Issue #15
        /// </summary>
        [Test]
        public void Of_IHaveTasks_Mocks_LeafTask_Method_ReturnsCompletedTask_WithLeafMoq()
        {
            var res = _moq.Of<IHaveTasks>().Object;

            var task = res.LeafTask();

            task.Should().NotBeNull();
            task.IsCompleted.Should().BeTrue();

            task.Result.Should().NotBeNull();

            task.Result.Should().Be(_moq.Of<ILeaf>().Object);
        }

        [Test]
        public void Create_ClassWitIInterfaceWithGenericMethodParam_CanCreate()
        {
            var res = _moq.Create<ClassWitIInterfaceWithGenericMethodParam>();

            res.Should().NotBeNull();
            res.CtorParam.Should().NotBeNull();
        }

        [Test]
        public void Mock_Call_ParameterlessMethod_ReturningInterface_ReturnsMockedInterface()
        {
            var subject = _moq.Create<SomeClass>();

            var res = subject.Mock.GetA();

            res.Should().NotBeNull();
        }

        [Test]
        public void MockGet_ClassWithNonVirtualInterfaceProperty_CanSetup()
        {
            var res = _moq.Of<ClassWithNonVirtualInterfaceProperty>();
            res.Should().NotBeNull();
        }

        [Test]
        public void MockGet_ClassWithNonVirtualNullableProperty_CanSetup()
        {
            var res = _moq.Create<ClassWithNonVirtualNullableProperty>();
            res.Should().NotBeNull();
        }

        [Test]
        public void MockGet_ClassWithNonVirtualProperty_CanSetup()
        {
            var res = _moq.Create<ClassWithNonVirtualProperty>();
            res.Should().NotBeNull();
        }

        [Test]
        [Ignore("Need an extension point in Moq library first")]
        public void MockOf_IInterfaceWithGenericMethod_CanSetup()
        {
            var res = _moq.Of<IInterfaceWithGenericMethod>();
            res.Should().NotBeNull();
        }

        [Test]
        public void Object_ClassWithCtorContainingClassWithoutParameterlessCtor_ShouldThrowException()
        {
            TestDelegate action =
                () => _moq.Object<ClassWithCtorContainingClassWithoutParameterlessCtor>();

            Assert.That(action, Throws.TypeOf<MoqqerException>());
        }


        [Test]
        public void Object_ClassWithCtorContainingClassWithParameterlessCtor_ShouldInjectDefaultObject()
        {
            TestDelegate action =
                () => _moq.Object<ClassHavingParameterlessConcreteClass>();

            Assert.That(action, Throws.TypeOf<MoqqerException>());
        }


        [Test]
        public void Object_ClassWithCtorContainingClassWithParameterlessCtor_ShouldReturn()
        {
            TestDelegate action =
                () => _moq.Object<ClassHavingParameterlessConcreteClass>();

            Assert.That(action, Throws.TypeOf<MoqqerException>());
        }

        [Test]
        public void Of_IOpenGenericMethodsWithClosedGenericListReturnTypes_CanMock()
        {
            var res = _moq.Of<IOpenGenericMethodsWithClosedGenericListReturnTypes>();

            res.Should().NotBeNull();
        }

        [Test]
        public void Of_IOpenGenericInterfaceWithTListReturnTypes_ListPropertiesReturnDefaultLists()
        {
            _moq.List<Leaf>().Add(new Leaf());

            var res = _moq.Of<IOpenGenericInterfaceWithTListReturnTypes<Leaf>>().Object;

            res.Should().NotBeNull();

            res.GetList().Should().BeSameAs(_moq.List<Leaf>());
            res.GetIList().Should().BeSameAs(_moq.List<Leaf>());
            res.GetEnumerable().Should().BeSameAs(_moq.List<Leaf>());

            // IQueryable should be equivalent as List.AsQueryable() gets returned which is a wrapper
            res.GetIQueryable().Should().BeEquivalentTo(_moq.List<Leaf>());
        }

        [Test]
        public void Of_ClassWithNonVirtualMemeber_CanMock()
        {
            var res = _moq.Of<ClassWithNonVirtualMemeber>();

            res.Should().NotBeNull();
        }

        [Test]
        public void SetupMockMethods_IInterfaceWithGenericMethod_ShouldReturnNull()
        {
            var mock = new Mock<IInterfaceWithGenericMethod>();

            var type = typeof(IInterfaceWithGenericMethod);

            _moq.SetupMockMethods(mock, type);

            mock.Object.Query<string>().Should().BeEmpty();
        }

        [Test]
        public void SetupMockMethods_SomeClassGetA_ShouldNotBeNull()
        {
            var mock = new Mock<IMockSetup>();

            var type = typeof(IMockSetup);

            _moq.SetupMockMethods(mock, type);

            mock.Object.GetA().Should().NotBeNull();
        }

        [Test]
        public void Type_GetMethods_ReturnsAllPublic()
        {
            var type = typeof(SomeClass);

            var res = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var methodInfo in res)
                Console.WriteLine(methodInfo.Name);
        }
        [Test]
        public void Create_DefaultListImplementations()
        {
            var res = _moq.Create<ClassWithListInCtor>();

            res.StringList.Should().NotBeNull()
                .And.BeSameAs(_moq.Object<List<string>>());
            res.StringCollection.Should().NotBeNull()
                .And.BeSameAs(_moq.Object<List<string>>());
            res.StringEnumerable.Should().NotBeNull()
                .And.BeSameAs(_moq.Object<List<string>>());
            res.StringListInterface.Should().NotBeNull()
                .And.BeSameAs(_moq.Object<List<string>>());
            res.StringQueryable.Should().NotBeNull()
                .And.BeEquivalentTo(_moq.Object<List<string>>());
        }
    }


}