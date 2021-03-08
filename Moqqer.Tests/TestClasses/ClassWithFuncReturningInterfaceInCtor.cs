using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public class ClassWithFuncReturningInterfaceInCtor
    {
        public Func<IDepencyA> FuncOfIDependencyA { get; }

        public ClassWithFuncReturningInterfaceInCtor(Func<IDepencyA> funcOfIDependencyA)
        {
            FuncOfIDependencyA = funcOfIDependencyA;
        }
    }
}