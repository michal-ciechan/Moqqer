using System;
using System.Linq.Expressions;
using Moq;

namespace MoqqerNamespace.Extensions
{
    public static class MoqqerExtensions
    {
        public static MoqFluentVerifyBuilder<T>.IHasVerifyAction Verify<T>
            (this Moqqer moq, Expression<Action<T>> action) 
            where T : class
        {
            return new MoqFluentVerifyBuilder<T>(moq.Of<T>(), action);
        }
    }



    public class MoqFluentVerifyBuilder<T> : MoqFluentVerifyBuilder<T>.IHasVerifyAction where T : class
    {
        private readonly Mock<T> _mock;
        private readonly Expression<Action<T>> _action;

        internal MoqFluentVerifyBuilder(Mock<T> mock, Expression<Action<T>> action)
        {
            _mock = mock;
            _action = action;
        }

        private void ExecuteVerify(Times times)
        {
            _mock.Verify(_action, times);
        }

        public void Never()
        {
            ExecuteVerify(Moq.Times.Never());
        }

        public void Once()
        {
            ExecuteVerify(Moq.Times.Once());
        }

        public void WasCalled()
        {
            ExecuteVerify(Moq.Times.AtLeastOnce());
        }

        public void Times(int times)
        {
            ExecuteVerify(Moq.Times.Exactly(times));
        }

        public void Times(Times times)
        {
            ExecuteVerify(times);
        }

        public interface IHasVerifyAction
        {
            void Never();
            void Once();
            void WasCalled();
            void Times(int times);
            void Times(Times times);
        }
    }

}
