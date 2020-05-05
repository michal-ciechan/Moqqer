using System;
using System.Linq;
using System.Threading.Tasks;
using MoqqerNamespace.Helpers;

namespace MoqqerNamespace.DefaultFactories
{
    class TaskDefaultGenericFactory : BaseGenericDefaultGenericFactory
    {
        public override bool CanHandle(Moqqer moq, Type type, Type openType, Type[] genericArguments)
        {
            return openType == typeof(Task<>) && genericArguments.All(x => AreMockable(moq, x));
        }

        private bool AreMockable(Moqqer moq, Type type)
        {
            return moq.CanGetDefaultOrMock(type) && !type.IsGenericType;
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