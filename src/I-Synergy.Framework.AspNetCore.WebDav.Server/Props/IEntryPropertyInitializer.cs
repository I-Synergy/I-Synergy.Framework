using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props
{
    /// <summary>
    /// Initzialize a newly created document or collection with properties
    /// </summary>
    public interface IEntryPropertyInitializer
    {
        /// <summary>
        /// Initialize a new document with properties
        /// </summary>
        /// <param name="document">The document to create the properties for</param>
        /// <param name="propertyStore">The property store</param>
        /// <param name="context">The PUT request context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>the task</returns>
        Task CreatePropertiesAsync(
            IDocument document,
            IPropertyStore propertyStore,
            IWebDavContext context,
            CancellationToken cancellationToken);

        /// <summary>
        /// Initialize a new collection with properties
        /// </summary>
        /// <param name="collection">The collection to create the properties for</param>
        /// <param name="propertyStore">The property store</param>
        /// <param name="context">The MKCOL request context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>the task</returns>
        Task CreatePropertiesAsync(
            ICollection collection,
            IPropertyStore propertyStore,
            IWebDavContext context,
            CancellationToken cancellationToken);
    }
}
