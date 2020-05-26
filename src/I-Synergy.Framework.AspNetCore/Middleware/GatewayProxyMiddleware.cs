using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.Middleware
{
    /// <summary>
    /// A Gateway Proxy middleware.
    /// </summary>
    public class GatewayProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly GatewayProxyOptions _options;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor for the gateway proxy middleware.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public GatewayProxyMiddleware(RequestDelegate next, IOptions<GatewayProxyOptions> options, ILogger<GatewayProxyMiddleware> logger, IHttpClientFactory httpClientFactory)
        {
            Argument.IsNotNull(nameof(GatewayProxyOptions), options.Value);

            _next = next;
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Task to invoke the middleware.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpClientFactory"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            foreach (var proxy in _options.GatewayProxies)
            {
                foreach (var segment in proxy.SourcePaths)
                {
                    if (context.Request.Path.StartsWithSegments(segment) && proxy.AllowedMethods.Contains(context.Request.Method))
                    {
                        _logger.LogDebug($"TargetUri: {proxy.DestinationUri.AbsoluteUri}");

                        var url = new Url(proxy.DestinationUri)
                            .AppendPathSegment(context.Request.Path)
                            .SetQueryParam(context.Request.QueryString.ToString().Replace("?", ""))
                            .ToUri();

                        var targetRequestMessage = context.CreateProxyHttpRequest(url);

                        using var responseMessage = await _httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted).ConfigureAwait(false);

                        await context.SendProxiedHttpRequestAsync(responseMessage).ConfigureAwait(false);

                        return;
                    }
                }
            }

            await _next(context).ConfigureAwait(false);
        }

        private HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private void GenerateDebugOutput(HttpRequest request)
        {
            var requestTrace = new StringBuilder();

            foreach (var header in request.Headers)
            {
                requestTrace.AppendLine(header.ToString());
            }

            requestTrace.AppendLine($"Body: {request.Body?.ToString()}");
            requestTrace.AppendLine($"ContentLength: {request.ContentLength?.ToString()}");
            requestTrace.AppendLine($"ContentType: {request.ContentType?.ToString()}");
            requestTrace.AppendLine($"Host: {request.Host.ToString()}");
            requestTrace.AppendLine($"Method: {request.Method?.ToString()}");
            requestTrace.AppendLine($"Path: {request.Path.ToString()}");
            requestTrace.AppendLine($"Query: {request.Query?.ToString()}");
            requestTrace.AppendLine($"QueryString: {request.QueryString.ToString()}");
            requestTrace.AppendLine($"Scheme: {request.Scheme?.ToString()}");

            _logger.LogTrace(requestTrace.ToString());
        }
    }
}
