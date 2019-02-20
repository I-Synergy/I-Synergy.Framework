using AspNet.Security.OpenIdConnect.Primitives;
using System;

namespace ISynergy.Exceptions
{
    public class OpenIdConnectException : Exception
    {
        public OpenIdConnectResponse Response { get; }

        public OpenIdConnectException(OpenIdConnectResponse response) : base(response.ErrorDescription)
        {
            Response = response;
        }

        public OpenIdConnectException() : base()
        {
        }

        public OpenIdConnectException(string message) : base(message)
        {
        }

        public OpenIdConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}