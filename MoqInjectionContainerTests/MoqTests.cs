using FluentAssertions;
using Moq;
using MoqInjectionContainerTests.Helpers;
using NUnit.Framework;

namespace MoqInjectionContainerTests
{
    [TestFixture]
    public class MoqTests
    {
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
        public void Moq_Setup_IsAny_ThenIsSpecific_CallsSpecific()
        {
            var mock = new Mock<IParameterisedMethodClass>();

            mock.Setup(q => q.Call(It.IsAny<int>())).Returns("Any");

            mock.Setup(q => q.Call(It.Is<int>(x => x == 25))).Returns("25");

            var res = mock.Object.Call(25);

            res.Should().Be("25");
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
        public void Moq_Setup_IsAny_CallsAany()
        {
            var mock = new Mock<IParameterisedMethodClass>();

            mock.Setup(q => q.Call(It.IsAny<int>())).Returns("Any");


            var res = mock.Object.Call(25);

            res.Should().Be("Any");
        }
    }
}