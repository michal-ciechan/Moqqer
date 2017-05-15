using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.Moq
{
    [TestFixture]
    public class MoqTests
    {
        [Test]
        public void Moq_Setup_IsAny_CallsAany()
        {
            var mock = new Mock<IParameterisedMethodClass>();

            mock.Setup(q => q.Call(It.IsAny<int>())).Returns("Any");


            var res = mock.Object.Call(25);

            res.Should().Be("Any");
        }

        [Test]
        public void Moq_Setup_IsAny_CallsAny()
        {
            var mock = new Mock<IParameterisedMethodClass>();

            mock.Setup(q => q.Call(It.IsAny<int>())).Returns("Any");


            var res = mock.Object.Call(25);

            res.Should().Be("Any");
        }

        [Test]
        public void Moq_Setup_IsAny_ThenIsSpecific_CallsSpecific()
        {
            var mock = new Mock<IParameterisedMethodClass>();

            mock.Setup(q => q.Call(It.IsAny<int>())).Returns("Any");

            mock.Setup(q => q.Call(It.Is<int>(x => x == 25))).Returns("25");

            var res = mock.Object.Call(25);

            res.Should().Be("25");
        }

        [Test]
        public void Moq_Setup_IsAny_ThenSpecific_CallsSpecific()
        {
            var mock = new Mock<IParameterisedMethodClass>();

            mock.Setup(q => q.Call(It.IsAny<int>())).Returns("Any");

            mock.Setup(q => q.Call(25)).Returns("25");

            var res = mock.Object.Call(25);

            res.Should().Be("25");
        }

        [Test]
        public void Moq_GenericMethod_GetsMock()
        {
            var mock = new Mock<IInterfaceWithGenericMethod>();

            //mock.Setup(q => q.Queryable<object>()).Returns(() =>
            //{
            //    var asQueryable = new HashSet<object>() {"Test"}.AsQueryable();
            //    return asQueryable;
            //});


            var res = mock.Object.Queryable<StringCtor>();
            
            res.Should().NotBeNull();
        }

        [Test]
        public void Moq_SetupReturnsCallback()
        {
            var mock = new Mock<IBranch>();

            var call = new CallContext<ILeaf>()
            {
                Arguments = new object[1],
                CallType = CallType.Method
            };

            var leaf = new Leaf(88);

            var factory = new Factory<ILeaf>(context => leaf);

            var factoryExpr = Expression.Constant(factory);

            var method = factory.GetType().GetMethod(nameof(factory.GetMethodParameter));

            var typeExpression = Expression.Constant(typeof(Branch));
            var calledMethod = typeof(Branch).GetMethod(nameof(Branch.GetLeaf), new[] { typeof(int) });
            var calledMethodExpr = Expression.Constant(calledMethod);

            var methodParams = calledMethod.GetParameters();

            var methodParamExpressions = methodParams
                .Select(x => Expression.Parameter(x.ParameterType))
                .ToArray();

            Expression[] methodParamConvertedExpressions = methodParamExpressions
                .Select(x => Expression.Convert(x, typeof(object)))
                .ToArray();

            var array = Expression.NewArrayInit(typeof(object), methodParamConvertedExpressions);

            var defaultMockExpression = Expression.Constant(new Mock<ILeaf>().Object);

            var getMethodParameterExpression = Expression.Call(factoryExpr, method, new Expression[]
            {
                typeExpression,
                calledMethodExpr,
                array,
                defaultMockExpression
            });

            var delegateType = typeof(Func<int, string, int, object, ILeaf>);

            var getMethodParamConvertedExpression = Expression.Convert(getMethodParameterExpression, typeof(ILeaf));

            var lambda = Expression.Lambda(delegateType, getMethodParamConvertedExpression,
                methodParamExpressions);

            var lambdaFunc = (Func<int, string, int, object, ILeaf>)lambda.Compile();

            var res = lambdaFunc(1, "Test", 2, null);

            //var functionCallExpr = Expression.Call(factoryExpr, method, callExpr);

            mock.Setup(x => x.GetLeaf(It.IsAny<int>())).Returns(() => new Leaf());
        }
    }
}