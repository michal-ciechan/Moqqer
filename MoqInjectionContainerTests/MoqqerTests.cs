using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Moq;
using MoqInjectionContainer;
using MoqInjectionContainerTests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MoqInjectionContainerTests
{
    [TestFixture]
    public class MoqqerTests
    {
        private MockRepository _factory;
        private Moqqer _moq;
        private SomeClass _subject;

        [SetUp]
        public void A_Setup()
        {
            _moq = new Moqqer();

            _factory = new MockRepository(MockBehavior.Default) {DefaultValue = DefaultValue.Mock};

            _subject = _moq.Create<SomeClass>();
        }


        [Test]
        public void CallA_CallsDependencyA()
        {
            _subject.CallA();

            _moq.Of<IDepencyA>().Verify(x => x.Call(), Times.Once);
        }

        [Test]
        public void Mock_Call_ParameterlessMethod_ReturningInterface_ReturnsMockedInterface()
        {
            var res = _subject.Mock.GetA();

            res.Should().NotBeNull();
        }
        
        [Test]
        public void Type_GetMethods_ReturnsAllPublic()
        {
            var type = typeof (SomeClass);

            var res = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var methodInfo in res)
            {
                Console.WriteLine(methodInfo.Name);
            }
        }

        [Test]
        public void GetMockableMethods_IMockSetup_ReturnsAllPublic()
        {
            var type = typeof (IMockSetup);

            var methods = Moqqer.GetMockableMethods(type).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo(new [] {"GetA", "GetB"});
        }


        [Test]
        public void GetMockableMethods_ClassWithNonVirtualProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualProperty);

            var methods = Moqqer.GetMockableMethods(type).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }

        [Test]
        public void GetMockableMethods_ClassWithNonVirtualNullableProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualNullableProperty);

            var methods = Moqqer.GetMockableMethods(type).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }

        [Test]
        public void GetMockableMethods_ClassWithNonVirtualInterfaceProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualInterfaceProperty);

            var methods = Moqqer.GetMockableMethods(type).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }

        [Test]
        public void SetupMockMethods_SomeClassGetA_ShouldNotBeNull()
        {
            var mock = new Mock<IMockSetup>();

            var type = typeof (IMockSetup);

            _moq.SetupMockMethods(mock, type);

            mock.Object.GetA().Should().NotBeNull();
        }


        [Test]
        public void Object_ClassWithCtorContainingClassWithParameterlessCtor_ShouldReturn()
        {
            TestDelegate action =
                () => _moq.Object<ClassWithCtorContainingClassWithParameterlessCtor>();

            Assert.That(action, Throws.TypeOf<MoqqerException>());
        }


        [Test]
        public void Object_ClassWithCtorContainingClassWithParameterlessCtor_ShouldInjectDefaultObject()
        {
            TestDelegate action =
                () => _moq.Object<ClassWithCtorContainingClassWithParameterlessCtor>();

            Assert.That(action, Throws.TypeOf<MoqqerException>());

        }

        [Test]
        public void Object_ClassWithCtorContainingClassWithoutParameterlessCtor_ShouldThrowException()
        {
            TestDelegate action =
                () => _moq.Object<ClassWithCtorContainingClassWithoutParameterlessCtor>();

            Assert.That(action, Throws.TypeOf<MoqqerException>());
        }

        [Test]
        public void MockOf_IInterfaceWithGenericMethod_CanSetup()
        {
            var res = _moq.Of<IInterfaceWithGenericMethod>();
        }

        [Test]
        public void MockGet_ClassWithNonVirtualProperty_CanSetup()
        {
            var res = _moq.Create<ClassWithNonVirtualProperty>();
        }

        [Test]
        public void MockGet_ClassWithNonVirtualNullableProperty_CanSetup()
        {
            var res = _moq.Create<ClassWithNonVirtualNullableProperty>();
        }

        [Test]
        public void MockGet_ClassWithNonVirtualInterfaceProperty_CanSetup()
        {
            var res = _moq.Of<ClassWithNonVirtualInterfaceProperty>();
        }

        [Test]
        public void GetMockableMethods_IInterfaceWithGenericMethod_DoesNotReturnGenericMethod()
        {
            var type = typeof(IInterfaceWithGenericMethod);

            var methods = Moqqer.GetMockableMethods(type).Select(x => x.Name).ToList();

            methods.Should().BeEmpty();
        }

    }
}