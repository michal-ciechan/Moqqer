using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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

		[Test]
		public void CanCreate_Tuple_ShouldBeCompleteTaskWithDefault()
		{
			_moq.CanCreate(typeof(Tuple<string>)).Should().BeTrue();
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

		[Test]
		public void Create_ClassWith2Ctors1ContainingString()
		{
			var mock = _moq.Create<ClassWith2Ctors1ContainingString>(true);

			mock.InterfaceParam.Should().NotBeNull();
			mock.String.Should().NotBeNull();
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
		///     Issue #40
		/// </summary>
		[Test]
		public void Create_ClassWithCtorContainingIInterfaceWithOutAndRefParamMethods_CanCreate()
		{
			var res = _moq.Create<ClassWithCtorContainingIInterfaceWithOutAndRefParamMethods>();

			res.CtorParam.Should().NotBeNull();
		}
		

		[Test]
		public void Create_ClassWitIInterfaceWithGenericMethodParam_CanCreate()
		{
			var res = _moq.Create<ClassWitIInterfaceWithGenericMethodParam>();

			res.Should().NotBeNull();
			res.CtorParam.Should().NotBeNull();
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

		[Test]
		public void Create_GeneratesClassConstructorParameter()
		{
			var mock = _moq.Create<ClassWithCtorContainingClassWithParameterlessCtor>(true);

			mock.CtorParam.Should().NotBeNull();
		}

		[Test]
		public void Create_GeneratesInterfaceConstructorParameter()
		{
			var mock = _moq
				.Create<ClassWithCtorContainingClassWithoutParameterlessCtor>(true);

			mock.CtorParam.A.Should().NotBeNull();
		}

		[Test]
		public void Create_MockConcreteReturnTypes_MockedInterfaceWithConcreteClassProperty()
		{
			_moq.MockConcreteReturnTypes = true;

			var tree = _moq.Create<Tree>(true);

			tree.Branch.ConcreteLeaf.Should()
				.NotBeNull();
		}


		[Test]
		public void
			Factory_For_ClassWithoutParameterlessCtor_ShouldFindAndCreateClassContainingClassWithoutParameterlessCtor()
		{
			var instance = _moq.Create<ClassWithoutParameterlessCtor>();

			_moq.Factory<ClassWithoutParameterlessCtor>(_ => instance);

			var parent = _moq.Create<ClassWithCtorContainingClassWithoutParameterlessCtor>();

			parent.CtorParam.Should().BeSameAs(instance);
		}

		[Test]
		public void Factory_For_ConstructorCallOnly_OnlyConstructorInjectedReturnsCustom()
		{
			// Create custom leaf
			var customLeaf = new Leaf(25);

			// Register factory function
			_moq.Factory<ILeaf>(context =>
					context.CallType == CallType.Constructor
						? customLeaf // Returned when being injected into constructor
						: context.Default // All other times
			);

			// Get instance of Branch
			var branch = _moq.Create<Branch>();

			// Its leaf you should custom leaf
			branch.Leaf.Should().BeSameAs(customLeaf);
			branch.Leaf.Age.Should().Be(25);

			// Get an instance of tree
			var tree = _moq.Create<Tree>();

			// Indirector (non Ctor injection) will be default
			tree.Branch.Leaf.Should().NotBeSameAs(customLeaf);
			tree.Branch.GetLeaf().Should().NotBeSameAs(customLeaf);
			tree.Branch.GetLeaf(3).Should().NotBeSameAs(customLeaf);
		}

		[Test]
		public void Factory_For_MethodCallOnly_OnlyMockedMethodCallsReturnCustom()
		{
			var customLeaf = new Leaf(25);

			_moq.Factory<ILeaf>(context =>
				context.CallType == CallType.Method && context.Arg<int>() == 25
					? customLeaf
					: context.Default
			);


			var tree = _moq.Create<Tree>();

			tree.Branch.GetLeaf(25).Should()
				.BeSameAs(customLeaf);

			tree.Branch.Leaf.Should()
				.NotBeSameAs(customLeaf);

			tree.Branch.GetLeaf().Should()
				.NotBeSameAs(customLeaf);

			tree.Branch.GetLeaf(4).Should()
				.NotBeSameAs(customLeaf);

			_moq.Create<Branch>().Leaf.Should()
				.NotBeSameAs(customLeaf);
		}

		[Test]
		public void Factory_For_PropertyCallOnly_OnlyMockedMethodCallsReturnCustom()
		{
			var customLeaf = new Leaf(25);

			_moq.Factory<ILeaf>(context =>
				context.CallType == CallType.Property
					? customLeaf
					: context.Default
			);


			var tree = _moq.Create<Tree>();

			tree.Branch.Leaf.Should()
				.BeSameAs(customLeaf);

			tree.Branch.GetLeaf().Should()
				.NotBeSameAs(customLeaf);

			tree.Branch.GetLeaf(0).Should()
				.NotBeSameAs(customLeaf);

			tree.Branch.GetLeaf(25).Should()
				.NotBeSameAs(customLeaf);

			_moq.Create<Branch>().Leaf.Should()
				.NotBeSameAs(customLeaf);
		}

		[Test]
		public void GetInstance_Tuple_ShouldBeCompleteTaskWithDefault()
		{
			Action act = () => _moq.GetInstance<Tuple<string>>();

			act.Should().Throw<MoqqerException>().Which.Message.Should()
				.Contain("Cannot get Type('Tuple<string>') as it does not have a default constructor.");
		}

		[Test]
		public void HasObjectOrDefault_TupleTask_ShouldBeCompleteTaskWithDefault()
		{
			_moq.HasObjectOrDefault(typeof(Task<Tuple<string>>)).Should().BeFalse();
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
            void Action() => _moq.Object<ClassWithCtorContainingClassWithParameterlessCtor>();

            Assert.That(Action, Throws.TypeOf<MoqqerException>());
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
            void Action() => _moq.Object<ClassHavingParameterlessConcreteClass>();

            Assert.That(Action, Throws.TypeOf<MoqqerException>());
        }

		[Test]
		public void Of_ClassWithNonVirtualMember_CanMock()
		{
			var res = _moq.Of<ClassWithNonVirtualMemeber>();

			res.Should().NotBeNull();
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
		public void Of_IHaveTasks_Mocks_TupleTask_Method_ReturnsCompletedTaskWithDefaultTuple()
		{
			var res = _moq.Of<IHaveTasks>().Object;

			var task = res.TupleTask();

			task.Should().NotBeNull();
			task.IsCompleted.Should().BeTrue();

			task.Result.Should().Be(default(Tuple<string>));
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
		public void Of_IInterfaceWithOutAndRefParamMethods_CanMock()
		{
			var res = _moq.Of<IInterfaceWithOutAndRefParamMethods>();

			res.Should().NotBeNull();
		}

		[Test]
		public void Of_IOpenGenericMethodsWithClosedGenericListReturnTypes_CanMock()
		{
			var res = _moq.Of<IOpenGenericMethodsWithClosedGenericListReturnTypes>();

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
	}
}