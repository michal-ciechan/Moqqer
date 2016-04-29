using System.Linq;
using FluentAssertions;
using Moqqer.Namespace.Helpers;
using Moqqer.Namespace.Tests.TestClasses;
using NUnit.Framework;

namespace Moqqer.Namespace.Tests.Helpers
{
    public class TypeHelperTests
    {
        [Test]
        public void GetMockableMethods_ClassWithNonVirtualInterfaceProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualInterfaceProperty);

            var methods = type.GetMockableMethods().Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }

        [Test]
        public void GetMockableMethods_ClassWithNonVirtualNullableProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualNullableProperty);

            var methods = type.GetMockableMethods().Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }


        [Test]
        public void GetMockableMethods_ClassWithNonVirtualProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualProperty);

            var methods = type.GetMockableMethods().Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }

        [Test]
        public void GetMockableMethods_IInterfaceWithGenericMethod_DoesNotReturnGenericMethod()
        {
            var type = typeof(IInterfaceWithGenericMethod);

            var methods = type.GetMockableMethods().Select(x => x.Name).ToList();

            methods.Should().BeEmpty();
        }

        [Test]
        public void GetMockableMethods_IMockSetup_ReturnsAllPublic()
        {
            var type = typeof(IMockSetup);

            var methods = type.GetMockableMethods().Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo("GetA", "GetB");
        }
    }
}
