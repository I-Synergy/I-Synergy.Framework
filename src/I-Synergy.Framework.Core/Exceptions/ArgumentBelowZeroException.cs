using System;
using System.Runtime.Serialization;

namespace ISynergy.Exceptions
{
    [Serializable]
    public class ArgumentBelowZeroException : ArgumentOutOfRangeException
    {
        private const string message = "Argument should be bigger than 0.";

        public ArgumentBelowZeroException(string paramName) : base(paramName, message)
        {
        }

        public ArgumentBelowZeroException(Exception innerException) : base(message, innerException, message)
        {
        }

        public ArgumentBelowZeroException(string paramName, object actualValue) : base(paramName, actualValue, message)
        {
        }

        protected ArgumentBelowZeroException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ArgumentBelowZeroException()
        {
        }

        public ArgumentBelowZeroException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}