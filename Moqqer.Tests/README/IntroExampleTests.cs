using Moq;
using MoqqerNamespace.Extensions;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.README
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
        }

        [Test]
        public void DoWork_CallsRepositoryGetData()
        {
            // 1 -  First create an instance of Moqqer which will 
            //      act as a container for Mocks and Objects.
            _moq = new Moqqer();

            // 2 -  Get Moqqer to create an instance of some class you want to test. 
            // It will auto inject mocks in the its constructor.
           _subject = _moq.Create<SomeClass>();

            // 3 -  Call the mothod you want to test on SomeClass
            _subject.CallA(); // Calls private field IDependencyA.Call()

            // 4 -  Verify a mock that was auto injected was called.
            _moq.Of<IDepencyA>()
                .Verify(x => x.Call(), Times.Once);

            //      Alternatively use the Verify extension method
            _moq.Verify<IDepencyA>(x => x.Call())
                .Once();

            // 5 -  Test and Refactor to your hearts content 
            //      without worrying about changing the constructor calls!
        }
    }
}
