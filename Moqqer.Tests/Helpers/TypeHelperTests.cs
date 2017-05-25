using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        [Test]
        public void GetMockableMethods_ISuperInterface_ReturnsBaseInterfaceStringMethod()
        {
            var type = typeof(ISuperInterface);

            var methods = type.GetMockableMethods(x => x == typeof(string)).Select(x => x.Name).ToList();

            methods.Should().Contain("SubString");
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

        [Test]
        [TestCase(typeof(List<>), "List<T>")]
        [TestCase(typeof(List<List<string>>), "List<List<string>>")]
        [TestCase(typeof(List<string>), "List<string>")]
        [TestCase(typeof(string), "string")]
        [TestCase(typeof(object), "object")]
        [TestCase(typeof(Tuple), "Tuple")]
        [TestCase(typeof(Tuple<>), "Tuple<T1>")]
        [TestCase(typeof(Tuple<,>), "Tuple<T1,T2>")]
        [TestCase(typeof(Tuple<string,int>), "Tuple<string,int>")]
        [TestCase(typeof(object[]), "object[]")]
        [TestCase(typeof(object[][]), "object[][]")]
        [TestCase(typeof(object[,]), "object[,]")]
        [TestCase(typeof(Tuple<int,long>[]), "Tuple<int,long>[]")]
        public void Type_Describe(Type type, string expectedDescription)
        {
            type.Describe().Should().Be(expectedDescription);
        }

        [Test]
        public void MethodInfo_Describe()
        {
            var type = typeof(IAllMethodCombinations);

            var count = type.GetMembers().Length;

            for (int i = 0; i < count; i++)
                RunMethod(i);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void RunMethod(int i)
        {
            var type = typeof(IAllMethodCombinations);

            var meth = type.GetMethod("Method" + i);
            var expected = methods[i];

            var description = meth.Describe() + ";";

            description.Should().Be(expected);
        }

        string[] methods = @"
            void Method0();
            Task<T> Method1<T>();
            Tuple<T1,T2> Method2<T1,T2>();
            TReturnType Method3<TReturnType>();
            void Method4<TInputType>(TInputType type);
            void Method5(int i, string b);
            void Method6(Tuple<int,string> tuple, string b);
            void Method7(Task<int> t, object a);
            void Method8(object[] a);"
        .Split('\r')
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        public interface IAllMethodCombinations
        {
            // OpenGenericMethods
            void Method0();
            Task<T> Method1<T>();
            Tuple<T1,T2> Method2<T1,T2>();
            TReturnType Method3<TReturnType>();
            void Method4<TInputType>(TInputType type);
            void Method5(int i, string b);
            void Method6(Tuple<int,string> tuple, string b);
            void Method7(Task<int> t, object a);
            void Method8(object[] a);
        }
        

    }
}
