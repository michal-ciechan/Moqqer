﻿using System;
using System.Collections.ObjectModel;
using MoqqerNamespace.Extensions;

namespace MoqqerNamespace.DefaultFactories
{
    class ObservableCollectionDefaultGenericFactory : BaseGenericDefaultGenericFactory
    {
        public override bool CanHandle(Moqqer moq, Type type, Type openType, Type[] genericArguments)
        {
            return openType == typeof(ObservableCollection<>);
        }

        public override object CreateGeneric<T>(Moqqer moq, Type type, Type openType)
        {
            var list = moq.List<T>();

            var collection = new ObservableCollection<T>(list);

            collection.SetItems(list);

            return collection;
        }
    }
}