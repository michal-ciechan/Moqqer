namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassHavingParameterlessConcreteClass
    {
        public ClassHavingParameterlessConcreteClass(ParameterlessClass @class)
        {
            Class = @class;
        }

        public ParameterlessClass Class { get; set; }
    }
}