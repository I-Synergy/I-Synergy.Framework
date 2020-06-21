using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions;
using ISynergy.Framework.Storage.Azure.Abstractions;

namespace ISynergy.Framework.Storage.Azure.Services
{
    /// <summary>
    /// Class StorageService.
    /// Implements the <see cref="IStorageService" />
    /// </summary>
    /// <typeparam name="TAzureBlobOptions">The type of the t azure BLOB options.</typeparam>
    /// <seealso cref="IStorageService" />
    public class StorageService<TAzureBlobOptions> : IStorageService
        where TAzureBlobOptions : IAzureStorageBlobOptions
    {
        /// <summary>
        /// The azure document options
        /// </summary>
        private readonly TAzureBlobOptions _azureBlobOptions;
        /// <summary>
        /// The cloud storage account
        /// </summary>
        private readonly BlobContainerClient _blobContainer;
        /// <summary>
        /// The tenant service
        /// </summary>
        private readonly ITenantService _tenantService;

        /// <summary>
        /// Initializes a new instance of the <see name="StorageService" /> class.
        /// </summary>
        /// <param name="azureBlobOptions">The azure BLOB options.</param>
        /// <param name="tenantService">The tenant service.</param>
        public StorageService(TAzureBlobOptions azureBlobOptions, ITenantService tenantService)
        {
            Argument.IsNotNull(nameof(tenantService), tenantService);
            Argument.IsNotNullOrEmpty(nameof(tenantService.TenantId), tenantService.TenantId);

            _tenantService = tenantService;
            _azureBlobOptions = azureBlobOptions;

            _blobContainer = new BlobContainerClient(_azureBlobOptions.ConnectionString, _tenantService.TenantId.ToString());
            _blobContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageService{TAzureBlobOptions}"/> class.
        /// </summary>
        /// <param name="azureBlobOptions">The azure BLOB options.</param>
        /// <param name="containerName">Name of the container.</param>
        public StorageService(TAzureBlobOptions azureBlobOptions, string containerName)
        {
            Argument.IsNotNullOrEmpty(nameof(containerName), containerName);

            _azureBlobOptions = azureBlobOptions;

            _blobContainer = new BlobContainerClient(_azureBlobOptions.ConnectionString, containerName);
            _blobContainer.CreateIfNotExists(PublicAccessType.Blob);
        }

        /// <summary>
        /// upload file as an asynchronous operation.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Uri.</returns>
        /// <exception cref="System.IO.IOException">CloudBlob not found.</exception>
        /// <exception cref="IOException">CloudBlob not found.</exception>
        public async Task<Uri> UploadFileAsync(Stream stream, string contentType, string filename, string folder, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
            {
                await blobClient.UploadAsync(
                        stream,
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
        public async Task<Stream> DownloadFileAsync(string filename, string folder, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream();

            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
            {
                await blobClient
                    .DownloadToAsync(stream, cancellationToken)
                    .ConfigureAwait(false);

                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }

        /// <summary>
        /// update file as an asynchronous operation.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Uri.</returns>
        /// <exception cref="System.IO.IOException">CloudBlob not found.</exception>
        /// <exception cref="IOException">CloudBlob not found.</exception>
        public async Task<Uri> UpdateFileAsync(Stream stream, string contentType, string filename, string folder, CancellationToken cancellationToken = default)
        {
            if (_blobContainer.GetBlobClient(Path.Combine(folder, filename)) is BlobClient blobClient)
            {
                await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                await blobClient.UploadAsync(
                        stream,
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
