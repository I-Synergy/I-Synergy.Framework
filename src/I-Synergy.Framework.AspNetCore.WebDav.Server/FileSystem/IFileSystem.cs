using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem
{
    /// <summary>
    /// The file system
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Gets the root collection
        /// </summary>
        AsyncLazy<ICollection> Root { get; }

        /// <summary>
        /// Gets a value indicating whether the file system allows seeking and partial reading.
        /// </summary>
        bool SupportsRangedRead { get; }

        /// <summary>
        /// Gets the property store to be used for the file system.
        /// </summary>
        IPropertyStore PropertyStore { get; }

        /// <summary>
        /// Gets the global lock manager
        /// </summary>
        ILockManager LockManager { get; }

        /// <summary>
        /// Finds an entry for a given path
        /// </summary>
        /// <param name="path">The root-relative path</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The result of the search operation</returns>
        Task<SelectionResult> SelectAsync(string path, CancellationToken ct);
    }
}
