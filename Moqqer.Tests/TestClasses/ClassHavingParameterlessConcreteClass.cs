namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassHavingParameterlessConcreteClass
    {
        public ClassHavingParameterlessConcreteClass(ClassWithParameterlessCtor ctor)
        {
            Ctor = ctor;
        }

        public ClassWithParameterlessCtor Ctor { get; set; }
    }
}