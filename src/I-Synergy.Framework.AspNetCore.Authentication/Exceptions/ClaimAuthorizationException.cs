using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Exceptions
{
    public abstract class ClaimAuthorizationException : Exception
    {
        protected ClaimAuthorizationException()
        {
        }

        protected ClaimAuthorizationException(string message) : base(message)
        {
        }

        protected ClaimAuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}