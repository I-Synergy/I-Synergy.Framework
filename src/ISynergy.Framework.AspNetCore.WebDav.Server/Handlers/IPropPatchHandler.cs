using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>PROPPATCH</c> handler
    /// </summary>
    public interface IPropPatchHandler : IClass1Handler
    {
        /// <summary>
        /// Patches (sets or removes) properties from the given <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path to patch the properties for</param>
        /// <param name="request">The properties to patch</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> PropPatchAsync(string path, propertyupdate request, CancellationToken cancellationToken);
    }
}
