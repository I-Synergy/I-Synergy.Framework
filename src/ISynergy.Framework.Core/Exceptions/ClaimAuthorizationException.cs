using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Core.Exceptions
{
    /// <summary>
    /// Class ClaimAuthorizationException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public abstract class ClaimAuthorizationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimAuthorizationException"/> class.
        /// </summary>
        protected ClaimAuthorizationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimAuthorizationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected ClaimAuthorizationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimAuthorizationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected ClaimAuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ClaimAuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
