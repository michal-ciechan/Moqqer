using System;
using System.Reflection;

namespace MoqqerNamespace
{
    public interface ICallContext
    {
        MethodInfo Method { get; }
        PropertyInfo Property { get; }
        object[] Arguments { get; }
        Type ParentClass { get; set; }
        CallType CallType { get; }
    }

    public interface ICallContext<out T> : ICallContext
    {
        T Default { get; }
    }

    public enum CallType
    {
        Constructor,
        Method,
        Property
    }

    public class CallContext<T> : ICallContext<T>
    {
        public T Default { get; internal set; }
        public MethodInfo Method { get; internal set; }
        public PropertyInfo Property { get; internal set; }
        public object[] Arguments { get; internal set; }
        public Type ParentClass { get; set; }
        public CallType CallType { get; internal set; }
        public ConstructorInfo Constructor { get; set; }

        public TArg Arg<TArg>(TArg defaultReturn = default(TArg))
        {
            if (Arguments == null)
                return defaultReturn;

            foreach (var argument in Arguments)
            {
                if (argument is TArg)
                    return (TArg)argument;
            }

            return defaultReturn;
        }

        public TArg Arg<TArg>(int argIndex, TArg defaultReturn = default(TArg))
        {
            if (Arguments == null)
                return defaultReturn;

            if (Arguments.Length < argIndex)
                return defaultReturn;

            var arg = Arguments[argIndex];

            if (arg is TArg)
                return (TArg) arg;

            return defaultReturn;
        }
    }
}