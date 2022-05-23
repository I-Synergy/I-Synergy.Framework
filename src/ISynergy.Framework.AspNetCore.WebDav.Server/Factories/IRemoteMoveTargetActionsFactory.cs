using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Remote
{
    /// <summary>
    /// The interface for a factory to create remote move target actions
    /// </summary>
    public interface IRemoteMoveTargetActionsFactory
    {
        /// <summary>
        /// Creates a remote target action impleemtation
        /// </summary>
        /// <param name="destinationUrl">The destination URL</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The remote target action implementation</returns>
        Task<IRemoteMoveTargetActions> CreateMoveTargetActionsAsync(Uri destinationUrl, CancellationToken cancellationToken);
    }
}
