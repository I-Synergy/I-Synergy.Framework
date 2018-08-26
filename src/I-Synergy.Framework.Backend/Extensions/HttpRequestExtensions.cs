using Microsoft.AspNetCore.Http;
using System.Net;

namespace ISynergy.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsLocal(this HttpRequest httpRequest)
        {
            var connection = httpRequest.HttpContext.Connection;

            // for in memory server or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
                return true;

            if (connection.RemoteIpAddress != null)
            {
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return false;
        }
    }
}