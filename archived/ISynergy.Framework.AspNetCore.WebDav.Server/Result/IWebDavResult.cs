using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server
{
    /// <summary>
    /// The result of a WebDAV operation
    /// </summary>
    public interface IWebDavResult
    {
        /// <summary>
        /// Gets the WebDAV status code
        /// </summary>
        WebDavStatusCode StatusCode { get; }

        /// <summary>
        /// Writes the result to a <paramref name="response"/>
        /// </summary>
        /// <param name="response">The response object to write to</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The async task</returns>
        Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct);
    }
}
