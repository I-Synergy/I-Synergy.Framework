using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Remote
{
    /// <summary>
    /// The implementation of the <see cref="IHttpMessageHandlerFactory"/>
    /// </summary>
    public class DefaultHttpMessageHandlerFactory : IHttpMessageHandlerFactory
    {
        /// <inheritdoc />
        public Task<HttpMessageHandler> CreateAsync(Uri baseUrl, CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpMessageHandler>(new HttpClientHandler());
        }
    }
}
