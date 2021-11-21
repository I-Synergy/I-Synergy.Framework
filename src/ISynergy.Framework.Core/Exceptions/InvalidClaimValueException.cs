using System;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Core.Exceptions
{
    /// <summary>
    /// Class InvalidClaimValueException.
    /// Implements the <see cref="ClaimAuthorizationException" />
    /// </summary>
    /// <seealso cref="ClaimAuthorizationException" />
    public class InvalidClaimValueException : ClaimAuthorizationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidClaimValueException"/> class.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        public InvalidClaimValueException(string claimType)
            : base($"Claim '{claimType}' is found, but has an invalid value.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidClaimValueException"/> class.
        /// </summary>
        public InvalidClaimValueException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidClaimValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidClaimValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidClaimValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
