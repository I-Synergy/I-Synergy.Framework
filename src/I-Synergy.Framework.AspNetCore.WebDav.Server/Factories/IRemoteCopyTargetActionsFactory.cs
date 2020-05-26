using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Remote
{
    /// <summary>
    /// The interface for a factory to create remote copy target actions
    /// </summary>
    public interface IRemoteCopyTargetActionsFactory
    {
        /// <summary>
        /// Creates a remote target action impleemtation
        /// </summary>
        /// <param name="destinationUrl">The destination URL</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The remote target action implementation</returns>
        Task<IRemoteCopyTargetActions> CreateCopyTargetActionsAsync(Uri destinationUrl, CancellationToken cancellationToken);
    }
}
