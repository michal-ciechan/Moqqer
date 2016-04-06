# Moqqer

Moqqer is a MockFactory/Repository/Container for Moq. It automatically injects mocks into the constructor of an application while keeping a reference to the Mock for future use.

This helps to ease the pain of creating mocks each time, as welll less hassle when you change the constructors or refactor code.

Moqqer also automatically stubs methods of an interface to return Mocks of the return type (where applicable).

## Example Usage

    [TestClass]
    public class ClassUnderTestTests
    {
        private Moqqer _moq;
        private ClassUnderTest _subject;

        [TestInitialize]
        public void A_TestInit()
        {
            // Create instance of Moqqer
            _moq = new Moqqer();
            
            // Create an instance of ClassUnderTest injecting mocks
            _subject = _moq.Get<ClassUnderTest>();
        }

        [TestMethod]
        public void DoWork_CallsRepositoryGetData()
        {
            _subject.DoWork();

            _moq.Of<IRepository>()
                .Verify(x => x.GetData(), Times.Once);
        }
    }

## Installing

Find it on NuGet under MoqInjectionContainer

https://www.nuget.org/packages/MoqInjectionContainer/

## Source Code

Currently the source code exists in [github/codePerf/MoqInjectionContainer][1] but will be migrated to here in the near future.

[1][https://github.com/michal-ciechan/codePERF/tree/master/MoqInjectionContainer]
