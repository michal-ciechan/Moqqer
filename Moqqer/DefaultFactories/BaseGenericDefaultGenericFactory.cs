using System;
using System.Linq;
using System.Reflection;
using MoqqerNamespace.Helpers;

namespace MoqqerNamespace.DefaultFactories
{
    abstract class BaseGenericDefaultGenericFactory : IDefaultGenericFactory
    {
        public static readonly MethodInfo CreateGenericMethodInfo =
            typeof(BaseGenericDefaultGenericFactory).GetGenericMethod("CreateGeneric");

        public abstract bool CanHandle(Moqqer moq, Type type, Type openType, Type[] genericArguments);
        public object Create(Moqqer moq, Type type, Type openType, Type[] genericArguments)
        {
            var meth = CreateGenericMethodInfo.MakeGenericMethod(genericArguments);

            return meth.Invoke(this, new object[] {moq, type, openType});
        }

        public abstract object CreateGeneric<T>(Moqqer moq, Type type, Type openType);
    }
}