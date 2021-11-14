using ISynergy.Framework.Storage.Abstractions.Options;

namespace ISynergy.Framework.Storage.Abstractions.Services
{
    /// <summary>
    /// Interface IStorageService
    /// </summary>
    public interface IStorageService<TStorageOptions>
        where TStorageOptions : class, IStorageOptions, new()
    {
        /// <summary>
        /// Uploads the file asynchronous.
        /// </summary>
        /// <param name="fileBytes">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Uri&gt;.</returns>
        Task<Uri> UploadFileAsync(byte[] fileBytes, string contentType, string filename, string folder, bool overwrite = false, CancellationToken cancellationToken = default);
        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.IO.Stream&gt;.</returns>
        Task<byte[]> DownloadFileAsync(string filename, string folder, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates the file asynchronous.
        /// </summary>
        /// <param name="fileBytes">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;Uri&gt;.</returns>
        Task<Uri> UpdateFileAsync(byte[] fileBytes, string contentType, string filename, string folder, CancellationToken cancellationToken = default);
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
