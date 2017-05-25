using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoqqerNamespace.DefaultFactories
{
    public interface IDefaultFactory
    {
        bool CanHandle(Moqqer moq, Type type, Type openType, Type[] genericArguments);
        object Create(Moqqer moq, Type type, Type openType, Type[] genericArguments);
    }
}
