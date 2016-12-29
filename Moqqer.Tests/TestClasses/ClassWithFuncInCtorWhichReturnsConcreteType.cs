using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithFuncInCtorWhichReturnsConcreteType
    {
        public Func<ClassWithParameterlessCtor> Func1 { get; }

        public ClassWithFuncInCtorWhichReturnsConcreteType(Func<ClassWithParameterlessCtor> func)
        {
            Func1 = func;
        }
    }
}