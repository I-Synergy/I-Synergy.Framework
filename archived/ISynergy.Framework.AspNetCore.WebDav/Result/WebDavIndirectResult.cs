using ISynergy.Framework.AspNetCore.WebDav.Logging;
using ISynergy.Framework.AspNetCore.WebDav.Response;
using ISynergy.Framework.AspNetCore.WebDav.Server;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Result
{
    /// <summary>
    /// An <see cref="IActionResult"/> implementation that takes a <see cref="IWebDavResult"/>
    /// </summary>
    public class WebDavIndirectResult : StatusCodeResult
    {
        private static readonly IEnumerable<MediaType> _supportedMediaTypes = new[] { "text/xml", "application/xml" }.Select(x => new MediaType(x)).ToList();

        private readonly IWebDavDispatcher _dispatcher;

        private readonly IWebDavResult _result;

        private readonly ILogger<WebDavIndirectResult> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavIndirectResult"/> class.
        /// </summary>
        /// <param name="dispatcher">The WebDAV HTTP method dispatcher</param>
        /// <param name="result">The result of the WebDAV operation</param>
        /// <param name="logger">The logger for a <see cref="WebDavIndirectResult"/></param>
        public WebDavIndirectResult(IWebDavDispatcher dispatcher, IWebDavResult result, ILogger<WebDavIndirectResult> logger)
            : base((int)result.StatusCode)
        {
            _dispatcher = dispatcher;
            _result = result;
            _logger = logger;
        }

        /// <inheritdoc />
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            // Sets the HTTP status code
            await base.ExecuteResultAsync(context);

            // Sets the reason phrase
            var responseFeature = context.HttpContext.Features.Get<IHttpResponseFeature>();
            if (responseFeature != null)
                responseFeature.ReasonPhrase = _result.StatusCode.GetReasonPhrase();

            if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                var loggingResponse = new LoggingWebDavResponse(_dispatcher);
                await _result.ExecuteResultAsync(loggingResponse, context.HttpContext.RequestAborted);
                if (!string.IsNullOrEmpty(loggingResponse.ContentType))
                {
                    var mediaType = new MediaType(loggingResponse.ContentType);
                    if (_supportedMediaTypes.Any(x => mediaType.IsSubsetOf(x)))
                    {
                        var doc = loggingResponse.Load();
                        if (doc != null)
                            _logger.LogDebug(doc.ToString(SaveOptions.OmitDuplicateNamespaces));
                    }
                }
            }

            // Writes the XML response
            await _result.ExecuteResultAsync(new WebDavResponse(_dispatcher, response), context.HttpContext.RequestAborted);
        }
    }
}
