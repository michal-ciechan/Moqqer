using Moq;
using Moqqer.Namespace.Tests.TestClasses;
using NUnit.Framework;

namespace Moqqer.Namespace.Tests.Samples
{
    [TestFixture]
    public class IntroExampleTests
    {
        private Moqqer _moq;
        private SomeClass _subject;

        // Setup run per test.
        [SetUp]
        public void A_TestInit()
        {
            // Create instance of Moqqer
            _moq = new Moqqer();

            // Create an instance of SomeClass injecting mocks into constructor
            _subject = _moq.Create<SomeClass>();
        }

        [Test]
        public void DoWork_CallsRepositoryGetData()
        {
            _subject.CallA(); // Calls private field IDependencyA.Call()

            _moq.Of<IDepencyA>()
                .Verify(x => x.Call(), Times.Once);
        }
    }
}
