using System;
using System.Linq;
using MoqqerNamespace.MoqqerQueryable;

namespace MoqqerNamespace.DefaultFactories
{
    class QueryableDefaultFactory : BaseGenericDefaultFactory
    {
        public override bool CanHandle(Type type, Type openType, Type[] genericArguments)
        {
            return openType == typeof(IQueryable<>);
        }

        public override object CreateGeneric<T>(Moqqer moq, Type type, Type openType)
        {
            var list = moq.List<T>();

            return moq.UseMoqqerEnumerableQuery
                ? list.AsMoqqerQueryable()
                : list.AsQueryable();
        }
    }
}