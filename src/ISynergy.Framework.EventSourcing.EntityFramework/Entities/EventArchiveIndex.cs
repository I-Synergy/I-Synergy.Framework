namespace ISynergy.Framework.EventSourcing.EntityFramework.Entities;

/// <summary>
/// Metadata record that tracks where a stream's archived events are stored in blob storage.
/// One row is written per archive operation per stream, recording the blob path and version range.
/// </summary>
public sealed class EventArchiveIndex
{
    /// <summary>Gets or sets the unique identifier for this index entry.</summary>
    public Guid IndexId { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the tenant that owns this stream.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Gets or sets the aggregate identifier (stream ID).</summary>
    public Guid StreamId { get; set; }

    /// <summary>Gets or sets the aggregate type name (e.g. "Order", "Invoice").</summary>
    public string StreamType { get; set; } = string.Empty;

    /// <summary>Gets or sets the lowest <c>AggregateVersion</c> covered by the blob.</summary>
    public long VersionFrom { get; set; }

    /// <summary>Gets or sets the highest <c>AggregateVersion</c> covered by the blob.</summary>
    public long VersionTo { get; set; }

    /// <summary>Gets or sets the total number of events in the blob.</summary>
    public int EventCount { get; set; }

    /// <summary>
    /// Gets or sets the blob path in the format
    /// <c>{tenantId}/{StreamType}/{StreamId}/{VersionFrom}-{VersionTo}.json</c>.
    /// </summary>
    public string BlobPath { get; set; } = string.Empty;

    /// <summary>Gets or sets when this archive entry was created.</summary>
    public DateTimeOffset ArchivedAt { get; set; }
}
