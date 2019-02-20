using System;

namespace ISynergy.Exceptions
{
    public class ClaimNotFoundException : ClaimAuthorizationException
    {
        public ClaimNotFoundException() : base()
        {
        }

        public ClaimNotFoundException(string claimType) : base($"Claim '{claimType}' not found.")
        {
        }

        public ClaimNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}