using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>PROPFIND</c> handler
    /// </summary>
    public interface IPropFindHandler : IClass1Handler
    {
        /// <summary>
        /// Queries properties (dead or live) for a given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to query the properties for</param>
        /// <param name="request">Some information about the properties to query</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> PropFindAsync(string path, propfind request, CancellationToken cancellationToken);
    }
}
