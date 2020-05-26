using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server
{
    /// <summary>
    /// A WebDAV result returning an <see cref="XElement"/> in the response body
    /// </summary>
    public class WebDavXmlResult : WebDavResult
    {
        private readonly XElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavXmlResult"/> class.
        /// </summary>
        /// <param name="statusCode">The WebDAV status code</param>
        /// <param name="element">The element to be serialized as the response body</param>
        public WebDavXmlResult(WebDavStatusCode statusCode, XElement element)
            : base(statusCode)
        {
            _element = element;
        }

        /// <inheritdoc />
        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            await base.ExecuteResultAsync(response, ct);
            response.Dispatcher.Formatter.Serialize(response.Body, _element);
        }
    }
}
