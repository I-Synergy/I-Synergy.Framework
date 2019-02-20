using System;
using System.Runtime.Serialization;

namespace ISynergy.Exceptions
{
    [Serializable]
    public class UnexpectedEnumValueException : Exception
    {
        public UnexpectedEnumValueException()
        {
        }

        public UnexpectedEnumValueException(string message)
            : base(message)
        {
        }

        public UnexpectedEnumValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UnexpectedEnumValueException(Type enumClass, object value)
            : base($"The value({enumClass}) of Enum type '{value}' was unexpected.")
        {
        }

        protected UnexpectedEnumValueException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}