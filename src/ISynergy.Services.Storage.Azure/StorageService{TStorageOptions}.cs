using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Services.Options;
using Microsoft.Extensions.Options;

namespace ISynergy.Services
{
    /// <summary>
    /// Class StorageService.
    /// Implements the <see cref="IStorageService{TStorageOptions}" />
    /// </summary>
    /// <typeparam name="TStorageOptions">The type of the t azure BLOB options.</typeparam>
    /// <seealso cref="IStorageService{TStorageOptions}" />
    public class StorageService<TStorageOptions> : IStorageService<TStorageOptions>
        where TStorageOptions : class, IStorageOptions, new()
    {
        /// <summary>
        /// The azure document options
        /// </summary>
        private readonly TStorageOptions _storageOptions;
        /// <summary>
        /// The cloud storage account
        /// </summary>
        private readonly BlobContainerClient _blobContainer;
        /// <summary>
        /// The tenant service
        /// </summary>
        private readonly ITenantService _tenantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageService{TStorageOptions}" /> class.
        /// </summary>
        /// <param name="storageOptions">The azure BLOB options.</param>
        /// <param name="tenantService">The tenant service.</param>
        public StorageService(IOptions<TStorageOptions> storageOptions, ITenantService tenantService)
        {
            Argument.IsNotNull(nameof(storageOptions), storageOptions.Value);
            Argument.IsNotNull(nameof(tenantService), tenantService);
            Argument.IsNotNullOrEmpty(nameof(tenantService.TenantId), tenantService.TenantId);

            _tenantService = tenantService;
            _storageOptions = storageOptions.Value;

            _blobContainer = new BlobContainerClient(_storageOptions.ConnectionString, _tenantService.TenantId.ToString());
            _blobContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageService{TStorageOptions}"/> class.
        /// </summary>
        /// <param name="storageOptions">The azure BLOB options.</param>
        /// <param name="containerName">Name of the container.</param>
        public StorageService(IOptions<TStorageOptions> storageOptions, string containerName)
        {
            Argument.IsNotNull(nameof(storageOptions), storageOptions.Value);
            Argument.IsNotNullOrEmpty(nameof(containerName), containerName);

            _storageOptions = storageOptions.Value;

            _blobContainer = new BlobContainerClient(_storageOptions.ConnectionString, containerName);
            _blobContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        /// <summary>
        /// upload file as an asynchronous operation.
        /// </summary>
        /// <param name="fileBytes">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Uri.</returns>
        /// <exception cref="IOException">CloudBlob not found.</exception>
        /// <exception cref="IOException">CloudBlob not found.</exception>
        public async Task<Uri> UploadFileAsync(byte[] fileBytes, string contentType, string filename, string folder, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
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
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>MemoryStream.</returns>
        public async Task<byte[]> DownloadFileAsync(string filename, string folder, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream();

            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
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
        /// <param name="fileBytes">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Uri.</returns>
        /// <exception cref="IOException">CloudBlob not found.</exception>
        /// <exception cref="IOException">CloudBlob not found.</exception>
        public async Task<Uri> UpdateFileAsync(byte[] fileBytes, string contentType, string filename, string folder, CancellationToken cancellationToken = default)
        {
            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
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
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> RemoveFileAsync(string filename, string folder, CancellationToken cancellationToken = default)
        {
            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
            {
                return await blobClient.DeleteIfExistsAsync()
                    .ConfigureAwait(false);
            }

            return false;
        }
    }
}
