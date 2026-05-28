using Amazon.S3;
using Amazon.S3.Model;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.EventSourcing.Models;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ISynergy.Framework.EventSourcing.Storage.S3.Services;

/// <summary>
/// AWS S3 (and S3-compatible) implementation of <see cref="IEventArchiveStorage"/>.
/// Events are stored as a JSON array in a tenant-specific bucket at object key
/// <c>{streamType}/{streamId}/{vFrom}-{vTo}.json</c>.
/// Each tenant receives its own isolated bucket named <c>{bucketNamePrefix}-{tenantId}</c>.
/// </summary>
public sealed class S3EventArchiveStorage : IEventArchiveStorage
{
    private readonly AmazonS3Client _client;
    private readonly string _bucketNamePrefix;
    private readonly ILogger<S3EventArchiveStorage> _logger;

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>Initializes a new instance of <see cref="S3EventArchiveStorage"/>.</summary>
    /// <param name="client">
    /// Pre-configured <see cref="AmazonS3Client"/>. Register via
    /// <c>services.AddSingleton(new AmazonS3Client(credentials, config))</c>.
    /// </param>
    /// <param name="bucketNamePrefix">
    /// A short, lowercase prefix prepended to every per-tenant bucket name
    /// (e.g. <c>"myapp"</c> → bucket <c>myapp-{tenantId}</c>).
    /// Must be lowercase, contain only letters, digits, and hyphens, and leave
    /// room for the trailing <c>-{36-char-guid}</c> within the 63-character S3 limit.
    /// </param>
    /// <param name="logger">Logger.</param>
    public S3EventArchiveStorage(
        AmazonS3Client client,
        string bucketNamePrefix,
        ILogger<S3EventArchiveStorage> logger)
    {
        Argument.IsNotNull(client);
        Argument.IsNotNullOrEmpty(bucketNamePrefix);
        Argument.IsNotNull(logger);

        _client = client;
        _bucketNamePrefix = bucketNamePrefix;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> UploadEventsAsync(
        Guid tenantId,
        string streamType,
        Guid streamId,
        long versionFrom,
        long versionTo,
        IReadOnlyList<EventRecord> events,
        CancellationToken ct = default)
    {
        var bucketName = BucketNameFor(tenantId);
        await EnsureBucketExistsAsync(bucketName, ct).ConfigureAwait(false);

        // Tenant identity is already encoded in the bucket name, so the object key
        // only needs stream coordinates.
        var key = $"{streamType}/{streamId}/{versionFrom}-{versionTo}.json";
        var json = JsonSerializer.Serialize(events, s_jsonOptions);

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = new MemoryStream(Encoding.UTF8.GetBytes(json)),
            ContentType = "application/json",
            AutoCloseStream = true
        };

        await _client.PutObjectAsync(request, ct).ConfigureAwait(false);

        _logger.LogDebug(
            "Uploaded {Count} events to S3 {Bucket}/{Key}",
            events.Count, bucketName, key);

        return key;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EventRecord>> DownloadEventsAsync(
        Guid tenantId,
        string storagePath,
        CancellationToken ct = default)
    {
        var bucketName = BucketNameFor(tenantId);

        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = storagePath
        };

        using var response = await _client.GetObjectAsync(request, ct).ConfigureAwait(false);
        using var stream = new MemoryStream();

        await response.ResponseStream.CopyToAsync(stream, ct).ConfigureAwait(false);

        var json = Encoding.UTF8.GetString(stream.ToArray());
        var events = JsonSerializer.Deserialize<List<EventRecord>>(json, s_jsonOptions);

        _logger.LogDebug(
            "Downloaded {Count} events from S3 {Bucket}/{Key}",
            events?.Count ?? 0, bucketName, storagePath);

        return events ?? [];
    }

    /// <summary>
    /// Returns the S3 bucket name for the given tenant.
    /// The prefix and tenant GUID (lowercase) are combined as <c>{prefix}-{tenantId}</c>,
    /// giving each tenant a fully isolated storage bucket.
    /// </summary>
    private string BucketNameFor(Guid tenantId)
        => $"{_bucketNamePrefix}-{tenantId.ToString("D").ToLowerInvariant()}";

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct)
    {
        try
        {
            await _client.PutBucketAsync(
                new PutBucketRequest { BucketName = bucketName, UseClientRegion = true },
                ct).ConfigureAwait(false);
        }
        catch (AmazonS3Exception ex) when (ex.ErrorCode is "BucketAlreadyOwnedByYou")
        {
            // Bucket already exists and belongs to this account — safe to continue.
        }
    }
}
