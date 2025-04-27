using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Options;
using ISynergy.Framework.Storage.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Storage.Azure.Services;

/// <summary>
/// Class StorageService.
/// Implements the <see cref="IStorageService" />
/// </summary>
/// <typeparam name="TStorageOptions">The type of the t azure BLOB options.</typeparam>
/// <seealso cref="IStorageService" />
internal class StorageService<TStorageOptions> : IStorageService
    where TStorageOptions : class, IStorageOptions, new()
{
    private readonly ILogger _logger;
    private readonly TStorageOptions _storageOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageService{TStorageOptions}"/> class.
    /// </summary>
    /// <param name="storageOptions">The azure BLOB options.</param>
    /// <param name="logger"></param>
    public StorageService(
        IOptions<TStorageOptions> storageOptions,
        ILogger<StorageService<TStorageOptions>> logger)
    {
        Argument.IsNotNull(storageOptions.Value);
        _storageOptions = storageOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// upload file as an asynchronous operation.
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="fileBytes">The file stream.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Uri.</returns>
    /// <exception cref="IOException">CloudBlob not found.</exception>
    /// <exception cref="IOException">CloudBlob not found.</exception>
    public async Task<Uri> UploadFileAsync(string containerName, byte[] fileBytes, string contentType, string filename, string folder, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = new BlobContainerClient(_storageOptions.ConnectionString, containerName);
        blobContainer.CreateIfNotExists(PublicAccessType.Blob);

        if (blobContainer.GetBlobClient(Path.Combine(folder, filename)) is { } blobClient)
        {
            await blobClient.UploadAsync(
                    new MemoryStream(fileBytes),
                    new BlobHttpHeaders
                    {
                        ContentType = contentType
                    },
                    conditions: overwrite ? null : new BlobRequestConditions { IfNoneMatch = new ETag("*") },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return blobClient.Uri;
        }

        throw new IOException("CloudBlob not found.");
    }

    /// <summary>
    /// download file as an asynchronous operation.
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>MemoryStream.</returns>
    public async Task<byte[]> DownloadFileAsync(string containerName, string filename, string folder, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = new BlobContainerClient(_storageOptions.ConnectionString, containerName);

        blobContainer.CreateIfNotExists(PublicAccessType.Blob);
        var stream = new MemoryStream();

        if (blobContainer.GetBlobClient(Path.Combine(folder, filename)) is { } blobClient)
        {
            await blobClient
                .DownloadToAsync(stream, cancellationToken)
                .ConfigureAwait(false);

            stream.Seek(0, SeekOrigin.Begin);
        }

        return stream.ToArray();
    }

    /// <summary>
    /// update file as an asynchronous operation.
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="fileBytes">The file stream.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Uri.</returns>
    /// <exception cref="IOException">CloudBlob not found.</exception>
    /// <exception cref="IOException">CloudBlob not found.</exception>
    public async Task<Uri> UpdateFileAsync(string containerName, byte[] fileBytes, string contentType, string filename, string folder, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = new BlobContainerClient(_storageOptions.ConnectionString, containerName);
        blobContainer.CreateIfNotExists(PublicAccessType.Blob);

        if (blobContainer.GetBlobClient(Path.Combine(folder, filename)) is { } blobClient)
        {
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            await blobClient.UploadAsync(
                    new MemoryStream(fileBytes),
                    new BlobHttpHeaders
                    {
                        ContentType = contentType
                    },
                    conditions: null,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return blobClient.Uri;
        }

        throw new IOException("CloudBlob not found.");
    }

    /// <summary>
    /// remove file as an asynchronous operation.
    /// </summary>
    /// <param name="containerName"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task<bool> RemoveFileAsync(string containerName, string filename, string folder, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = new BlobContainerClient(_storageOptions.ConnectionString, containerName);
        blobContainer.CreateIfNotExists(PublicAccessType.Blob);

        if (blobContainer.GetBlobClient(Path.Combine(folder, filename)) is { } blobClient)
        {
            return await blobClient.DeleteIfExistsAsync()
                .ConfigureAwait(false);
        }

        return false;
    }
}
