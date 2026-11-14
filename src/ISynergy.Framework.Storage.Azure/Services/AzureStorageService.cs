using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Storage.Azure.Services;

/// <summary>
/// Class StorageService.
/// Implements the <see cref="IStorageService" />
/// </summary>
/// <remarks>
/// Authentication precedence:
/// an injected <see cref="BlobServiceClient"/> (for example from Aspire) is used first,
/// then identity-based authentication when a service URI is configured, and finally the
/// configured connection string.
/// </remarks>
/// <seealso cref="IStorageService" />
internal class AzureStorageService : IStorageService
{
    private readonly AzureStorageOptions _options;
    private readonly TokenCredential? _credential;
    private readonly BlobServiceClient? _blobServiceClient;

    private TokenCredential? _resolvedCredential;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureStorageService"/> class.
    /// </summary>
    /// <param name="options">The Azure storage options.</param>
    /// <param name="credential">
    /// Optional credential used for identity-based authentication. When omitted, a credential is
    /// created from <see cref="AzureStorageOptions.ManagedIdentityClientId"/>.
    /// </param>
    /// <param name="blobServiceClient">
    /// Optional pre-configured client (for example registered by Aspire). When supplied, it takes
    /// precedence over both identity configuration and the connection string.
    /// </param>
    public AzureStorageService(
        IOptions<AzureStorageOptions> options,
        TokenCredential? credential = null,
        BlobServiceClient? blobServiceClient = null)
    {
        Argument.IsNotNull(options);

        _options = options.Value;
        _credential = credential;
        _blobServiceClient = blobServiceClient;
    }

    /// <summary>
    /// upload file as an asynchronous operation.
    /// </summary>
    /// <param name="connectionStringName"></param>
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
    public async Task<Uri> UploadFileAsync(string connectionStringName, string containerName, byte[] fileBytes, string contentType, string filename, string folder, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(connectionStringName);
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = CreateBlobContainerClient(containerName);

        _ = await blobContainer
            .CreateIfNotExistsAsync(
                publicAccessType: PublicAccessType.None,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

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
    /// <param name="connectionStringName"></param>
    /// <param name="containerName"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>MemoryStream.</returns>
    public async Task<byte[]> DownloadFileAsync(string connectionStringName, string containerName, string filename, string folder, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(connectionStringName);
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = CreateBlobContainerClient(containerName);

        _ = await blobContainer
            .CreateIfNotExistsAsync(
                publicAccessType: PublicAccessType.None,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
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
    /// <param name="connectionStringName"></param>
    /// <param name="containerName"></param>
    /// <param name="fileBytes">The file stream.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Uri.</returns>
    /// <exception cref="IOException">CloudBlob not found.</exception>
    /// <exception cref="IOException">CloudBlob not found.</exception>
    public async Task<Uri> UpdateFileAsync(string connectionStringName, string containerName, byte[] fileBytes, string contentType, string filename, string folder, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(connectionStringName);
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = CreateBlobContainerClient(containerName);

        _ = await blobContainer
            .CreateIfNotExistsAsync(
                publicAccessType: PublicAccessType.None,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (blobContainer.GetBlobClient(Path.Combine(folder, filename)) is { } blobClient)
        {
            _ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken)
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
    /// <param name="connectionStringName"></param>
    /// <param name="containerName"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public async Task<bool> RemoveFileAsync(string connectionStringName, string containerName, string filename, string folder, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(connectionStringName);
        Argument.IsNotNullOrEmpty(containerName);

        var blobContainer = CreateBlobContainerClient(containerName);

        _ = await blobContainer
            .CreateIfNotExistsAsync(
                publicAccessType: PublicAccessType.None,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (blobContainer.GetBlobClient(Path.Combine(folder, filename)) is { } blobClient)
        {
            return await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return false;
    }

    /// <summary>
    /// Creates a container client using the first available authentication mechanism:
    /// injected client, identity endpoint, then connection string.
    /// </summary>
    internal BlobContainerClient CreateBlobContainerClient(string containerName)
    {
        if (_blobServiceClient is not null)
            return _blobServiceClient.GetBlobContainerClient(containerName);

        if (_options.TryGetServiceUri(out var serviceUri))
        {
            var containerUri = new Uri($"{serviceUri.AbsoluteUri.TrimEnd('/')}/{containerName}");
            return new BlobContainerClient(containerUri, GetOrCreateCredential());
        }

        var connectionString = _options.GetEffectiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"{nameof(AzureStorageOptions)} is not configured. Set " +
                $"{nameof(AzureStorageOptions)}:{nameof(AzureStorageOptions.ServiceEndpoint)} " +
                $"or {nameof(AzureStorageOptions)}:{nameof(AzureStorageOptions.AccountName)} for " +
                $"identity-based authentication, or " +
                $"{nameof(AzureStorageOptions)}:{nameof(AzureStorageOptions.ConnectionString)} as a fallback.");
        }

        return new BlobContainerClient(connectionString, containerName);
    }

    /// <summary>
    /// Returns the configured credential, creating it lazily so the service can be resolved
    /// without acquiring tokens until a storage operation actually runs.
    /// </summary>
    /// <remarks>
    /// <see cref="DefaultAzureCredential"/> probes several credential sources and is not fully
    /// trim/AOT safe. Applications publishing with <c>PublishAot</c> should inject an explicit
    /// <see cref="TokenCredential"/> (for example <see cref="ManagedIdentityCredential"/>) or set
    /// <see cref="AzureStorageOptions.ManagedIdentityClientId"/> to use
    /// <see cref="ManagedIdentityCredential"/>.
    /// </remarks>
    private TokenCredential GetOrCreateCredential()
    {
        if (_credential is not null)
            return _credential;

        return _resolvedCredential ??= string.IsNullOrWhiteSpace(_options.ManagedIdentityClientId)
            ? new DefaultAzureCredential()
            : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(_options.ManagedIdentityClientId));
    }
}
