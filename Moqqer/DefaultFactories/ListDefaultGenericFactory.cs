using System;
using System.Collections.Generic;
using DryIoc;
using MoqqerNamespace.Helpers;

namespace MoqqerNamespace.DefaultFactories
{
    class ListDefaultGenericFactory : IDefaultGenericFactory
    {
        public bool CanHandle(Moqqer moq, Type type, Type openType, Type[] genericArguments)
        {
            return typeof(List<>)
                .IsOpenGenericAssignableToOpenGenericType(openType);
        }

        public object Create(Moqqer moq, Type type, Type openType, Type[] genericArguments)
        {
            return GetOrCreateList(moq, genericArguments);
        }

        internal object GetOrCreateList(Moqqer moq, Type[] genericArguments)
        {
            var listType = typeof(List<>).MakeGenericType(genericArguments);

            var container = moq.Container;

            if (container.IsRegistered(listType))
                return container.Resolve(listType);

            return container.Resolve(listType);
        }
    }
}