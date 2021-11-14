namespace ISynergy.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Class HttpRequestExtensions.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Determines whether the specified HTTP request is local.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns><c>true</c> if the specified HTTP request is local; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Creates the proxy HTTP request.
        /// </summary>
        /// <param name="_self">The self.</param>
        /// <param name="uri">The URI.</param>
        /// <returns>HttpRequestMessage.</returns>
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
