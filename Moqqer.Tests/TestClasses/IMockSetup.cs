namespace Moqqer.Namespace.Tests.TestClasses
{
    public interface IMockSetup
    {
        IDepencyA GetA();
        IDepencyB GetB(string test);
        SomeClass GetClass();
    }
}