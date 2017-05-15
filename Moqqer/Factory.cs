using System;
using System.Reflection;

namespace MoqqerNamespace
{
    internal class Factory<T> : IFactory
    {
        public Factory(Func<CallContext<T>, T> factoryFunction)
        {
            Function = factoryFunction;
        }

        internal Func<CallContext<T>, T> Function { get; set; }

        public object GetConstructorParameter(Type argumentType, ConstructorInfo ctor, object defaultMock)
        {
            var context = new CallContext<T>
            {
                CallType = CallType.Constructor,
                ParentClass = ctor.DeclaringType,
                Default = (T)defaultMock,
                Constructor = ctor,
            };

            var parameter = Function(context);

            return parameter;
        }

        public object GetMethodParameter(Type type, MethodInfo method, object[] args, object defaultMock)
        {
            var context = new CallContext<T>()
            {
                CallType = CallType.Method,
                ParentClass = type,
                Default = (T) defaultMock,
                Method = method,
                Arguments = args
            };

            var result = Function(context);

            return result;
        }
    }

    internal interface IFactory
    {
        object GetConstructorParameter(Type type, ConstructorInfo ctor, object defaultMock);
        object GetMethodParameter(Type type, MethodInfo method, object[] args, object defaultMock);
    }
}