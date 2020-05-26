using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model.Headers;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>UNLOCK</c> handler
    /// </summary>
    public interface IUnlockHandler : IClass2Handler
    {
        /// <summary>
        /// Removes a LOCK with the given <paramref name="stateToken"/> from the given <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path to remove the lock for</param>
        /// <param name="stateToken">The state token of the lock to remove</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> UnlockAsync(string path, LockTokenHeader stateToken, CancellationToken cancellationToken);
    }
}
