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
            return moq.IsRegisteredOrCanMock(type) && !type.IsGenericType;
        }


        public override object CreateGeneric<T>(Moqqer moq, Type type, Type openType)
        {
            var instance = moq.GetInstance<T>();

            return TaskHelper.FromResult(instance);
        }
    }
}