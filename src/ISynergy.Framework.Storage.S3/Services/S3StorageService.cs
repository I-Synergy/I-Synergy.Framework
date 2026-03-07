using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.S3.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Storage.S3.Services;

/// <summary>
/// Storage service implementation for AWS S3 and S3-compatible providers (MinIO, etc.).
/// Implements <see cref="IStorageService" />.
/// </summary>
/// <seealso cref="IStorageService" />
internal class S3StorageService : IStorageService, IDisposable
{
    private readonly ILogger<S3StorageService> _logger;
    private readonly AmazonS3Client _client;
    private readonly S3StorageOptions _options;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="S3StorageService"/> class.
    /// </summary>
    /// <param name="options">The S3 storage options.</param>
    /// <param name="logger">The logger.</param>
    public S3StorageService(
        IOptions<S3StorageOptions> options,
        ILogger<S3StorageService> logger)
    {
        _options = options.Value;

        Argument.IsNotNullOrEmpty(_options.AccessKey);
        Argument.IsNotNullOrEmpty(_options.SecretKey);

        _logger = logger;

        var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
        var config = new AmazonS3Config
        {
            ForcePathStyle = _options.ForcePathStyle
        };

        if (!string.IsNullOrEmpty(_options.ServiceUrl))
        {
            config.ServiceURL = _options.ServiceUrl;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(_options.Region);
        }

        _client = new AmazonS3Client(credentials, config);
    }

    /// <summary>
    /// upload file as an asynchronous operation.
    /// </summary>
    /// <param name="connectionStringName">Not used for S3; credentials are configured via <see cref="S3StorageOptions"/>.</param>
    /// <param name="containerName">The S3 bucket name.</param>
    /// <param name="fileBytes">The file content.</param>
    /// <param name="contentType">The MIME content type.</param>
    /// <param name="filename">The file name.</param>
    /// <param name="folder">The folder prefix within the bucket.</param>
    /// <param name="overwrite">If <c>true</c>, overwrites an existing object; otherwise throws <see cref="IOException"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The URI of the uploaded object.</returns>
    /// <exception cref="IOException">Thrown when <paramref name="overwrite"/> is <c>false</c> and the object already exists.</exception>
    public async Task<Uri> UploadFileAsync(
        string connectionStringName,
        string containerName,
        byte[] fileBytes,
        string contentType,
        string filename,
        string folder,
        bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        if (!overwrite && await ObjectExistsAsync(containerName, key, cancellationToken).ConfigureAwait(false))
            throw new IOException($"Object '{key}' already exists in bucket '{containerName}'. Use overwrite=true to replace it.");

        var request = new PutObjectRequest
        {
            BucketName = containerName,
            Key = key,
            InputStream = new MemoryStream(fileBytes),
            ContentType = contentType,
            AutoCloseStream = true
        };

        await _client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Uploaded object {Key} to bucket {Bucket}", key, containerName);

        return BuildObjectUri(containerName, key);
    }

    /// <summary>
    /// download file as an asynchronous operation.
    /// </summary>
    /// <param name="connectionStringName">Not used for S3; credentials are configured via <see cref="S3StorageOptions"/>.</param>
    /// <param name="containerName">The S3 bucket name.</param>
    /// <param name="filename">The file name.</param>
    /// <param name="folder">The folder prefix within the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file content as a byte array.</returns>
    public async Task<byte[]> DownloadFileAsync(
        string connectionStringName,
        string containerName,
        string filename,
        string folder,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        var request = new GetObjectRequest
        {
            BucketName = containerName,
            Key = key
        };

        using var response = await _client.GetObjectAsync(request, cancellationToken).ConfigureAwait(false);
        using var stream = new MemoryStream();

        await response.ResponseStream.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Downloaded object {Key} from bucket {Bucket}", key, containerName);

        return stream.ToArray();
    }

    /// <summary>
    /// update file as an asynchronous operation.
    /// </summary>
    /// <param name="connectionStringName">Not used for S3; credentials are configured via <see cref="S3StorageOptions"/>.</param>
    /// <param name="containerName">The S3 bucket name.</param>
    /// <param name="fileBytes">The updated file content.</param>
    /// <param name="contentType">The MIME content type.</param>
    /// <param name="filename">The file name.</param>
    /// <param name="folder">The folder prefix within the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The URI of the updated object.</returns>
    public async Task<Uri> UpdateFileAsync(
        string connectionStringName,
        string containerName,
        byte[] fileBytes,
        string contentType,
        string filename,
        string folder,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        await DeleteObjectIfExistsAsync(containerName, key, cancellationToken).ConfigureAwait(false);

        var request = new PutObjectRequest
        {
            BucketName = containerName,
            Key = key,
            InputStream = new MemoryStream(fileBytes),
            ContentType = contentType,
            AutoCloseStream = true
        };

        await _client.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Updated object {Key} in bucket {Bucket}", key, containerName);

        return BuildObjectUri(containerName, key);
    }

    /// <summary>
    /// remove file as an asynchronous operation.
    /// </summary>
    /// <param name="connectionStringName">Not used for S3; credentials are configured via <see cref="S3StorageOptions"/>.</param>
    /// <param name="containerName">The S3 bucket name.</param>
    /// <param name="filename">The file name.</param>
    /// <param name="folder">The folder prefix within the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the object was deleted; <c>false</c> if it did not exist.</returns>
    public async Task<bool> RemoveFileAsync(
        string connectionStringName,
        string containerName,
        string filename,
        string folder,
        CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(containerName);

        var key = GetObjectKey(folder, filename);

        return await DeleteObjectIfExistsAsync(containerName, key, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _client.Dispose();
            _disposed = true;
        }
    }

    private static string GetObjectKey(string folder, string filename)
        => string.IsNullOrEmpty(folder)
            ? filename
            : $"{folder.TrimEnd('/')}/{filename}";

    private Uri BuildObjectUri(string bucketName, string key)
    {
        var serviceUrl = _options.ServiceUrl;
        return string.IsNullOrEmpty(serviceUrl)
            ? new Uri($"https://{bucketName}.s3.amazonaws.com/{key}")
            : new Uri($"{serviceUrl.TrimEnd('/')}/{bucketName}/{key}");
    }

    private async Task<bool> ObjectExistsAsync(string bucketName, string key, CancellationToken cancellationToken)
    {
        try
        {
            await _client.GetObjectMetadataAsync(bucketName, key, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task<bool> DeleteObjectIfExistsAsync(string bucketName, string key, CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteObjectRequest { BucketName = bucketName, Key = key };
            await _client.DeleteObjectAsync(request, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
