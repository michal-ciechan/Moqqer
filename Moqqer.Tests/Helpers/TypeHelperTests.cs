using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using MoqqerNamespace.Helpers;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.Helpers
{
    public class TypeHelperTests
    {
        [Test]
        public void GetMockableMethods_ClassWithNonVirtualInterfaceProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualInterfaceProperty);

            var methods = type.GetMockableMethods(x => false).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }
        [Test]
        public void GetMockableMethods_IInterfaceWithGenericMethod_ReturnsNoMethods()
        {
            var type = typeof(IInterfaceWithGenericMethod);

            var methods = type.GetMockableMethods(x => false).Select(x => x.Name).ToList();

            methods.Should().BeEmpty(" all methods of type IInterfaceWithGenericMethod are open Generic. Currently no supported way by Mock<T> to return a custo value.");
        }

        /// <summary>
        /// Issue #3
        /// </summary>
        [Test(Description = "Test That it will return methods which can be injected by Moqqer")]
        public void GetMockableMethods_MethodNonMockableButCanInject_ReturnsMethod()
        {
            var type = typeof(IDefaultMethods);
            
            var methods = type.GetMockableMethods(t => t == typeof(List<string>))
            .Select(x => x.Name).ToList();

            methods.Should().Contain("List");
        }

        [Test]
        public void GetMockableMethods_ClassWithNonVirtualNullableProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualNullableProperty);

            var methods = type.GetMockableMethods(x => false).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }


        [Test]
        public void GetMockableMethods_ClassWithNonVirtualProperty_ReturnsAllPublic()
        {
            var type = typeof(ClassWithNonVirtualProperty);

            var methods = type.GetMockableMethods(x => false).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo();
        }

        [Test]
        public void GetMockableMethods_IInterfaceWithGenericMethod_DoesNotReturnGenericMethod()
        {
            var type = typeof(IInterfaceWithGenericMethod);

            var methods = type.GetMockableMethods(x => false).Select(x => x.Name).ToList();

            methods.Should().BeEmpty();
        }

        [Test]
        public void GetMockableMethods_IMockSetup_ReturnsAllPublic()
        {
            var type = typeof(IMockSetup);

            var methods = type.GetMockableMethods(x => false).Select(x => x.Name).ToList();

            methods.Should().BeEquivalentTo("GetA", "GetB");
        }

        [Test]
        public void FindConstructor_ClassWith2Ctors1ContainingClassWithoutParameterlessCtor_WhichCannotBeInjected_ReturnsCtorWithInterfaceParam()
        {
            var type = typeof(ClassWith2Ctors1ContainingClassWithoutParameterlessCtor);

            var methods = type.FindConstructor(x => false)
                .GetParameters()
                .Select(x => x.ParameterType);

            methods.Should().BeEquivalentTo(typeof(IBranch));
        }

        [Test]
        public void FindConstructor_ClassWith2Ctors1ContainingClassWithoutParameterlessCtor_WhichCanBeInjected_ReturnsCtorWithBothParams()
        {
            var type = typeof(ClassWith2Ctors1ContainingClassWithoutParameterlessCtor);

            var methods = type.FindConstructor(x => true)
                .GetParameters()
                .Select(x => x.ParameterType);

            methods.Should().BeEquivalentTo(typeof(IBranch), typeof(ClassWithoutParameterlessCtor));
        }

        [Test]
        public void IsClosedGenericAssignableToOpenGenericType_List()
        {
            var closed = typeof(List<string>);
            var open = typeof(List<>);
            var openInterface = typeof(IList<>);
            var openBaseInterface = typeof(IEnumerable<>);

            closed.IsClosedGenericAssignableToOpenGenericType(open).Should().BeTrue();
            closed.IsClosedGenericAssignableToOpenGenericType(openInterface).Should().BeTrue();
            closed.IsClosedGenericAssignableToOpenGenericType(openBaseInterface).Should().BeTrue();
        }
        [Test]
        public void IsOpenGenericAssignableToOpenGenericType_List()
        {
            var from = typeof(List<>);
            var to = typeof(List<>);
            var toInterface = typeof(IList<>);
            var toBaseInterface = typeof(IEnumerable<>);

            from.IsClosedGenericAssignableToOpenGenericType(to).Should().BeTrue();
            from.IsClosedGenericAssignableToOpenGenericType(toInterface).Should().BeTrue();
            from.IsClosedGenericAssignableToOpenGenericType(toBaseInterface).Should().BeTrue();

            from.IsClosedGenericAssignableToOpenGenericType(typeof(Tuple<int>)).Should().BeFalse();
        }

    }
}
