﻿using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server
{
    /// <summary>
    /// Gets the WebDAV result with a value to be returned in the response body
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized as response body</typeparam>
    public class WebDavResult<T> : WebDavResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavResult{T}"/> class.
        /// </summary>
        /// <param name="statusCode">The WebDAV status code</param>
        /// <param name="data">The data to be returned in the response body</param>
        public WebDavResult(WebDavStatusCode statusCode, T data)
            : base(statusCode)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the data to be returned in the response body
        /// </summary>
        public T Data { get; }

        /// <inheritdoc />
        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            var formatter = response.Dispatcher.Formatter;
            response.ContentType = formatter.ContentType;
            await base.ExecuteResultAsync(response, ct);
            formatter.Serialize(response.Body, Data);
        }
    }
}
