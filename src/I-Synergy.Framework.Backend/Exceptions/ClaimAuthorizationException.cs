using System;

namespace ISynergy.Exceptions
{
    public abstract class ClaimAuthorizationException : Exception
    {
        protected ClaimAuthorizationException(string message) : base(message)
        {
        }
    }
}