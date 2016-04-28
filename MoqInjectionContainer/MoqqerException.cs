using System;
using System.Runtime.Serialization;

namespace MoqInjectionContainer
{
    internal class MoqqerException : Exception
    {
        public MoqqerException()
        {
        }

        public MoqqerException(string message) : base(message)
        {
        }

        public MoqqerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoqqerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}