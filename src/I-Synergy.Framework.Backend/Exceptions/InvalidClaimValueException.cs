using System;

namespace ISynergy.Exceptions
{
    public class InvalidClaimValueException : ClaimAuthorizationException
    {
        public InvalidClaimValueException(string claimType)
            : base($"Claim '{claimType}' is found, but has an invalid value.")
        {
        }

        public InvalidClaimValueException() : base()
        {
        }

        public InvalidClaimValueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}