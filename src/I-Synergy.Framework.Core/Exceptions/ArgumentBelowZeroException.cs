using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Core.Exceptions
{
    /// <summary>
    /// Class ArgumentBelowZeroException.
    /// Implements the <see cref="ArgumentOutOfRangeException" />
    /// </summary>
    /// <seealso cref="ArgumentOutOfRangeException" />
    [Serializable]
    public class ArgumentBelowZeroException : ArgumentOutOfRangeException
    {
        /// <summary>
        /// The message
        /// </summary>
        private const string message = "Argument should be bigger than 0.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentBelowZeroException"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that causes this exception.</param>
        public ArgumentBelowZeroException(string paramName) : base(paramName, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentBelowZeroException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public ArgumentBelowZeroException(Exception innerException) : base(message, innerException, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentBelowZeroException"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="actualValue">The actual value.</param>
        public ArgumentBelowZeroException(string paramName, object actualValue) : base(paramName, actualValue, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentBelowZeroException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">An object that describes the source or destination of the serialized data.</param>
        protected ArgumentBelowZeroException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentBelowZeroException"/> class.
        /// </summary>
        public ArgumentBelowZeroException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentBelowZeroException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ArgumentBelowZeroException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
