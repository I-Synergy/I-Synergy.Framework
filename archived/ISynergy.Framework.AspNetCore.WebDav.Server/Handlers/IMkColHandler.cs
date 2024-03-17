using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>MKCOL</c> handler
    /// </summary>
    public interface IMkColHandler : IClass1Handler
    {
        /// <summary>
        /// Creates a collection at the given path
        /// </summary>
        /// <param name="path">The path to the collection to create</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> MkColAsync(string path, CancellationToken cancellationToken);
    }
}
