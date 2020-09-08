using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem
{
    /// <summary>
    /// An entry in the WebDAV file system
    /// </summary>
    public interface IEntry
    {
        /// <summary>
        /// Gets the name of the entry
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the file system of this entry
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the collection that contains this entry
        /// </summary>
        /// <remarks>
        /// This property can be <c>null</c> when this entry is the root collection.
        /// </remarks>
        ICollection Parent { get; }

        /// <summary>
        /// Gets the path of the entry
        /// </summary>
        Uri Path { get; }

        /// <summary>
        /// Gets the last time this entry was modified
        /// </summary>
        DateTime LastWriteTimeUtc { get; }

        /// <summary>
        /// Gets the time this entry was created
        /// </summary>
        DateTime CreationTimeUtc { get; }

        /// <summary>
        /// Deletes this entry
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the delete operation</returns>
        Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Sets the last write time
        /// </summary>
        /// <param name="lastWriteTime">The new last write time</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The async task</returns>
        Task SetLastWriteTimeUtcAsync(DateTime lastWriteTime, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the creation time
        /// </summary>
        /// <param name="creationTime">The new creation time</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The async task</returns>
        Task SetCreationTimeUtcAsync(DateTime creationTime, CancellationToken cancellationToken);
    }
}
