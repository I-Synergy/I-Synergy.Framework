using ISynergy.Framework.EventSourcing.EntityFramework;
using ISynergy.Framework.EventSourcing.Models;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EventSourcing.Storage.Services;

/// <summary>
/// Reads the full event history for an aggregate by combining cold-tier storage archives
/// with the current hot-tier events in <see cref="EventSourcingDbContext"/>.
/// </summary>
public sealed class EventArchiveReader : IEventArchiveReader
{
    private readonly EventSourcingDbContext _context;
    private readonly IEventArchiveStorage _storage;

    /// <summary>Initializes a new instance of <see cref="EventArchiveReader"/>.</summary>
    public EventArchiveReader(
        EventSourcingDbContext context,
        IEventArchiveStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EventRecord>> GetFullHistoryAsync(
        Guid tenantId,
        string streamType,
        Guid aggregateId,
        CancellationToken ct = default)
    {
        // Load archive index entries for this stream (ordered by VersionFrom).
        // IgnoreQueryFilters because the caller passes an explicit tenantId.
        var archiveIndexes = await _context.ArchiveIndexes
            .IgnoreQueryFilters()
            .Where(a => a.TenantId == tenantId
                     && a.StreamId == aggregateId
                     && a.StreamType == streamType)
            .OrderBy(a => a.VersionFrom)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        // Collect all cold events from each blob segment.
        var coldEvents = new List<EventRecord>();
        foreach (var index in archiveIndexes)
        {
            var segment = await _storage.DownloadEventsAsync(tenantId, index.BlobPath, ct)
                .ConfigureAwait(false);
            coldEvents.AddRange(segment);
        }

        // Load hot events from the event store (already tenant-scoped via query filter
        // because IEventStore uses the ambient tenant from ITenantService, but since the
        // caller passes an explicit tenantId we bypass the filter and query directly).
        var hotEvents = await _context.Events
            .IgnoreQueryFilters()
            .Where(e => e.TenantId == tenantId
                     && e.AggregateType == streamType
                     && e.AggregateId == aggregateId)
            .OrderBy(e => e.AggregateVersion)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        // Merge cold + hot, deduplicate by version (cold wins — same data), then sort.
        var allVersions = new Dictionary<long, EventRecord>();

        foreach (var e in coldEvents)
            allVersions[e.AggregateVersion] = e;

        foreach (var e in hotEvents)
            allVersions.TryAdd(e.AggregateVersion, e);

        return [.. allVersions.Values.OrderBy(e => e.AggregateVersion)];
    }
}
