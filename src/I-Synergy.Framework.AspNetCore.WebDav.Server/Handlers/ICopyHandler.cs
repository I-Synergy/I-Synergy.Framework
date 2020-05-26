using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>COPY</c> handler
    /// </summary>
    public interface ICopyHandler : IClass1Handler
    {
        /// <summary>
        /// Copies from the source to the destination
        /// </summary>
        /// <param name="path">The source to copy</param>
        /// <param name="destination">The destination to copy to</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> CopyAsync(string path, Uri destination, CancellationToken cancellationToken);
    }
}
