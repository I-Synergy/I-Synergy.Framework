namespace ISynergy.Framework.AspNetCore.Authentication.Exceptions
{
    /// <summary>
    /// Class ClaimNotFoundException.
    /// Implements the <see cref="ClaimAuthorizationException" />
    /// </summary>
    /// <seealso cref="ClaimAuthorizationException" />
    public class ClaimNotFoundException : ClaimAuthorizationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException"/> class.
        /// </summary>
        public ClaimNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException"/> class.
        /// </summary>
        /// <param name="claimType">Type of the claim.</param>
        public ClaimNotFoundException(string claimType) : base($"Claim '{claimType}' not found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClaimNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Serializable constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ClaimNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
