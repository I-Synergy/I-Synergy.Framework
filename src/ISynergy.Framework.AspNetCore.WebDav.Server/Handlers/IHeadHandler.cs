using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>HEAD</c> handler
    /// </summary>
    public interface IHeadHandler : IClass1Handler
    {
        /// <summary>
        /// Gets the information about an element at the given path
        /// </summary>
        /// <param name="path">The path to the element to get the information for</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> HeadAsync(string path, CancellationToken cancellationToken);
    }
}
