using ISynergy.Framework.EventSourcing.Models;

namespace ISynergy.Framework.EventSourcing.Storage.Abstractions;

/// <summary>
/// Provider-agnostic interface for uploading and downloading event batches to/from
/// a cold-tier storage back-end (e.g. Azure Blob Storage, AWS S3, local filesystem).
/// </summary>
public interface IEventArchiveStorage
{
    /// <summary>
    /// Serializes <paramref name="events"/> to JSON and writes them to cold storage.
    /// </summary>
    /// <param name="tenantId">The owning tenant.</param>
    /// <param name="streamType">The aggregate type name (e.g. "Order").</param>
    /// <param name="streamId">The aggregate identifier.</param>
    /// <param name="versionFrom">The lowest <c>AggregateVersion</c> in the batch.</param>
    /// <param name="versionTo">The highest <c>AggregateVersion</c> in the batch.</param>
    /// <param name="events">The events to store.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// An opaque storage path/key that is recorded in <c>EventArchiveIndex.BlobPath</c>
    /// and passed back to <see cref="DownloadEventsAsync"/> when reading.
    /// </returns>
    Task<string> UploadEventsAsync(
        Guid tenantId,
        string streamType,
        Guid streamId,
        long versionFrom,
        long versionTo,
        IReadOnlyList<EventRecord> events,
        CancellationToken ct = default);

    /// <summary>
    /// Downloads and deserializes events from the cold storage path recorded in
    /// <c>EventArchiveIndex.BlobPath</c>.
    /// </summary>
    /// <param name="tenantId">
    /// The owning tenant. Used by implementations that isolate each tenant in a
    /// dedicated storage container.
    /// </param>
    /// <param name="storagePath">The path returned by <see cref="UploadEventsAsync"/>.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IReadOnlyList<EventRecord>> DownloadEventsAsync(Guid tenantId, string storagePath, CancellationToken ct = default);
}
