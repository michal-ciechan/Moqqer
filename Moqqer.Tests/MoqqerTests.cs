using System;
using System.Collections.Generic;
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
        private Moqqer _moq;

        [SetUp]
        public void A_Setup()
        {
            _moq = new Moqqer();

        }

        [Test]
        public void CallA_CallsDependencyA()
        {
            var subject = _moq.Create<SomeClass>();

            subject.CallA();

            _moq.Of<IDepencyA>().Verify(x => x.Call(), Times.Once);
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

        /// <summary>
        /// Issue #1
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
        /// Issue #1
        /// </summary>
        [Test]
        public void Create_ClassWith2Ctors1ContainingString_ShouldReturn()
        {
            var res = _moq.Create<ClassWith2Ctors1ContainingString>();

            res.InterfaceParam.Should().NotBeNull();
            res.String.Should().BeSameAs(string.Empty);
        }
        
        /// <summary>
        /// Issue #2
        /// </summary>
        [Test]
        public void Create_ClassWithDefaultMethods_ListIsDefaultObjectsList()
        {
            var res = _moq.Create<ClassWithDefaultMethods>();

            res.DefaultMethods.List()
                .Should().BeSameAs(_moq.Object<List<string>>());
        }

        /// <summary>
        /// Issue #2
        /// </summary>
        [Test]
        public void Create_ClassWithDefaultMethods_IListIsDefaultObjectsList()
        {
            var res = _moq.Create<ClassWithDefaultMethods>();

            res.DefaultMethods.ListInterface()
                .Should().BeSameAs(_moq.Object<List<string>>());
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
            {
                Console.WriteLine(methodInfo.Name);
            }
        }
    }
}