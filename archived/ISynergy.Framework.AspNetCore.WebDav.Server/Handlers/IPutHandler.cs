using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// Interface for the <c>PUT</c> handler
    /// </summary>
    public interface IPutHandler : IClass1Handler
    {
        /// <summary>
        /// Creates or updates a document at the given <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path where to create or update the document</param>
        /// <param name="data">The data to write to the new or existing document</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        Task<IWebDavResult> PutAsync(string path, Stream data, CancellationToken cancellationToken);
    }
}
