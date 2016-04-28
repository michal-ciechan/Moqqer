using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoqInjectionContainerTests.Helpers
{
    public interface IInterfaceWithGenericMethod
    {
        IQueryable<T> Queryable<T>();
    }
}
