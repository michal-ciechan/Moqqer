namespace Moqqer.Namespace.Tests.Helpers
{
    public interface IMockSetup
    {
        IDepencyA GetA();
        IDepencyB GetB(string test);
        SomeClass GetClass();
    }
}