using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithFuncInCtor
    {
        public Func<IDepencyA> FuncOfIDependencyA { get; }

        public ClassWithFuncInCtor(Func<IDepencyA> funcOfIDependencyA)
        {
            FuncOfIDependencyA = funcOfIDependencyA;
        }
    }
}