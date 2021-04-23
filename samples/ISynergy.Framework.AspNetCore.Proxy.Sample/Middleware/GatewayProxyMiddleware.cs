using Flurl;
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Proxy.Sample.Middleware
{
    /// <summary>
    /// A Gateway Proxy middleware.
    /// </summary>
    public class GatewayProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly GatewayProxyOptions _options;

        /// <summary>
        /// Constructor for the gateway proxy middleware.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public GatewayProxyMiddleware(RequestDelegate next, IOptions<GatewayProxyOptions> options, ILogger<GatewayProxyMiddleware> logger)
        {
            _next = next;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Task to invoke the middleware.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpClientFactory"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IHttpClientFactory httpClientFactory)
        {
            GenerateDebugOutput(context.Request);

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

                        using var httpClient = httpClientFactory.CreateClient();
                        using var responseMessage = await httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

                        GenerateDebugOutput(context.Request);

                        await context.SendProxiedHttpRequestAsync(responseMessage);

                        return;
                    }
                }
            }

            await _next(context);
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
