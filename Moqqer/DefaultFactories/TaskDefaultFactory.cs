using System;
using System.Threading.Tasks;
using MoqqerNamespace.Helpers;

namespace MoqqerNamespace.DefaultFactories
{
    class TaskDefaultFactory : BaseGenericDefaultFactory
    {
        public override bool CanHandle(Type type, Type openType, Type[] genericArguments)
        {
            return openType == typeof(Task<>);
        }

        public override object CreateGeneric<T>(Moqqer moq, Type type, Type openType)
        {
            var instance = moq.GetInstance(typeof(T));

            // ReSharper disable once MergeConditionalExpression
            var result = instance is T
                ? (T) instance
                : default(T);

            return TaskHelper.FromResult(result);
        }
    }
}