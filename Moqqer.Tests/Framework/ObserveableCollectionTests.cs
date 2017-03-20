using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using MoqqerNamespace.Extensions;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.Framework
{
    [TestFixture]
    class ObserveableCollectionTests
    {

        [SetUp]
        public void A_TestInitialise()
        {
        }

        [Test]
        public void ObservableCollectionExtensions_SetItems_IsSynchronized()
        {
            var list = new List<int>();

            var obs = new ObservableCollection<int>();

            obs.SetItems(list);

            obs.Should().HaveCount(0);
            list.Should().HaveCount(0);

            list.Add(1);

            obs.Should().HaveCount(1);
            list.Should().HaveCount(1);

            obs.Remove(1);

            obs.Should().HaveCount(0);
            list.Should().HaveCount(0);
        }
    }
}
