using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MoqqerNamespace;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.README
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DefaultMocksTests
    {
        private Moqqer _moq;

        [SetUp]
        public void A_TestSetup()
        {
            _moq = new Moqqer();
        }

        [Test]
        public void String()
        {
            _moq.Object<string>().Should().Be(string.Empty);
        }

        [Test]
        public void List()
        {
            MoqObjectOfShouldReturn<List<int>, List<int>>();
        }

        [Test]
        public void IList()
        {
            MoqObjectOfShouldReturn<IList<int>, List<int>>();
        }
        
        public void MoqObjectOfShouldReturn<TType, TMockType>() where TType : class
        {
            _moq.Object<TType>()
                .Should().BeOfType<TMockType>()
                .Which.Should().NotBeNull();

            var obj1 = _moq.Object<TType>();
            var obj2 = _moq.Object<TType>();

            obj1.Should().BeSameAs(obj2);
        }
    }
}