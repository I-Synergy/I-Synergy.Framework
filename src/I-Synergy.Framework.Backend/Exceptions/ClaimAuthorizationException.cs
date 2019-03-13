using System;

namespace ISynergy.Exceptions
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