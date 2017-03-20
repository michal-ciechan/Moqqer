using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MoqqerNamespace.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void SetItems<T>(this ObservableCollection<T> collection, IList<T> items)
        {
            var type = typeof(Collection<T>);

            var propertyInfo = type.GetField("items", BindingFlags.NonPublic | BindingFlags.Instance );

            if (propertyInfo == null) throw new MoqqerException($"Could not get 'items' property on {type.Name}");

            propertyInfo.SetValue(collection, items);
        }
    }
}
