using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ISynergy.Framework.EventSourcing.Models;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ISynergy.Framework.EventSourcing.Storage.Azure.Services;

/// <summary>
/// Azure Blob Storage implementation of <see cref="IEventArchiveStorage"/>.
/// Events are stored as a JSON array at path
/// <c>{tenantId}/{streamType}/{streamId}/{vFrom}-{vTo}.json</c>.
/// </summary>
public sealed class AzureBlobEventArchiveStorage : IEventArchiveStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureBlobEventArchiveStorage> _logger;

    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>Initializes a new instance of <see cref="AzureBlobEventArchiveStorage"/>.</summary>
    /// <param name="blobServiceClient">
    /// Pre-configured <see cref="BlobServiceClient"/>. Register via
    /// <c>AddAzureBlobServiceClient("blobs")</c> in Aspire or
    /// <c>services.AddSingleton(new BlobServiceClient(connStr))</c>.
    /// </param>
    /// <param name="containerName">The blob container name (e.g. <c>event-archive</c>).</param>
    /// <param name="logger">Logger.</param>
    public AzureBlobEventArchiveStorage(
        BlobServiceClient blobServiceClient,
        string containerName,
        ILogger<AzureBlobEventArchiveStorage> logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = containerName;
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
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerNameFor(tenantId));
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct)
            .ConfigureAwait(false);

        // Tenant identity is already encoded in the container name, so the blob path
        // only needs stream coordinates.
        var blobPath = $"{streamType}/{streamId}/{versionFrom}-{versionTo}.json";
        var blobClient = containerClient.GetBlobClient(blobPath);

        var json = JsonSerializer.Serialize(events, s_jsonOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct)
            .ConfigureAwait(false);

        _logger.LogDebug(
            "Uploaded {Count} events to blob {Container}/{Path}",
            events.Count, ContainerNameFor(tenantId), blobPath);

        return blobPath;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EventRecord>> DownloadEventsAsync(
        Guid tenantId,
        string storagePath,
        CancellationToken ct = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerNameFor(tenantId));
        var blobClient = containerClient.GetBlobClient(storagePath);

        var response = await blobClient.DownloadContentAsync(ct).ConfigureAwait(false);
        var json = response.Value.Content.ToString();

        var events = JsonSerializer.Deserialize<List<EventRecord>>(json, s_jsonOptions);
        return events ?? [];
    }

    /// <summary>
    /// Returns the blob container name for the given tenant.
    /// The tenant GUID (lowercase) is used directly as the container name,
    /// giving each tenant a fully isolated storage container.
    /// </summary>
    private static string ContainerNameFor(Guid tenantId) => tenantId.ToString("D").ToLowerInvariant();
}
