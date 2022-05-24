using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>OPTIONS</c> handler
    /// </summary>
    public interface IOptionsHandler : IClass1Handler
    {
        /// <summary>
        /// Queries the options for a given path.
        /// </summary>
        /// <remarks>
        /// This is used to identify the WebDAV capabilities at a given URL.
        /// </remarks>
        /// <param name="path">The root-relataive file system path to query the options for</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> OptionsAsync(string path, CancellationToken cancellationToken);
    }
}
