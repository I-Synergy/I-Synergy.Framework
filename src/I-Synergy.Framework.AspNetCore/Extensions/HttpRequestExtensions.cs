using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsLocal(this HttpRequest httpRequest)
        {
            var connection = httpRequest.HttpContext.Connection;

            // for in memory server or when dealing with default connection info
            if (connection.RemoteIpAddress is null && connection.LocalIpAddress is null)
                return true;

            if (connection.RemoteIpAddress != null)
            {
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return false;
        }

        public static HttpRequestMessage CreateProxyHttpRequest(this HttpContext _self, Uri uri)
        {
            var result = new HttpRequestMessage();
            var request = _self.Request;
            var requestMethod = _self.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                result.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                if (!result.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && result.Content != null)
                {
                    result.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            result.Headers.Host = uri.Authority;
            result.RequestUri = uri;
            result.Method = new HttpMethod(request.Method);

            return result;
        }
    }
}
