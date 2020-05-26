using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Exceptions
{
    public class InvalidClaimValueException : ClaimAuthorizationException
    {
        public InvalidClaimValueException(string claimType)
            : base($"Claim '{claimType}' is found, but has an invalid value.")
        {
        }

        public InvalidClaimValueException()
        {
        }

        public InvalidClaimValueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}