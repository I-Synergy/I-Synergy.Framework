using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Storage.Abstractions
{
    /// <summary>
    /// Interface IStorageService
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Uploads the file asynchronous.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Uri&gt;.</returns>
        Task<Uri> UploadFileAsync(Stream stream, string contentType, string filename, string folder, bool overwrite = false, CancellationToken cancellationToken = default);
        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.IO.Stream&gt;.</returns>
        Task<Stream> DownloadFileAsync(string filename, string folder, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates the file asynchronous.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Uri&gt;.</returns>
        Task<Uri> UpdateFileAsync(Stream stream, string contentType, string filename, string folder, CancellationToken cancellationToken = default);
        /// <summary>
        /// Removes the file asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> RemoveFileAsync(string filename, string folder, CancellationToken cancellationToken = default);
    }
}
