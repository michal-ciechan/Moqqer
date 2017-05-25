using System;
using System.Collections.Generic;
using MoqqerNamespace.Helpers;

namespace MoqqerNamespace.DefaultFactories
{
    class ListDefaultFactory : IDefaultFactory
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

            return moq.GetOrCreateObject(listType);
        }
    }
}