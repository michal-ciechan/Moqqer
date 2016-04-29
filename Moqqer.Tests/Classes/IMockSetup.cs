namespace Moqqer.Namespace.Tests.Classes
{
    public interface IMockSetup
    {
        IDepencyA GetA();
        IDepencyB GetB(string test);
        SomeClass GetClass();
    }
}