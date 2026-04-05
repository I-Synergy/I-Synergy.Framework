using ISynergy.Framework.EventSourcing.Aggregates;
using ISynergy.Framework.EventSourcing.EntityFramework;
using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;
using ISynergy.Framework.EventSourcing.EntityFramework.Entities;
using ISynergy.Framework.EventSourcing.Models;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.EventSourcing.Storage.Services;

/// <summary>
/// Implements the tiered storage archive job.
/// <list type="number">
/// <item>Finds streams with events older than the retention window.</item>
/// <item>Takes a snapshot at the archival boundary (so the hot-path AggregateRepository
///      can still load the aggregate from snapshot + remaining hot events).</item>
/// <item>Uploads the to-be-archived events to cold storage via <see cref="IEventArchiveStorage"/>.</item>
/// <item>Records an <see cref="EventArchiveIndex"/> row.</item>
/// <item>Deletes the archived event rows from PostgreSQL.</item>
/// </list>
/// </summary>
[RequiresUnreferencedCode("Uses reflection to instantiate aggregate types by name.")]
[RequiresDynamicCode("Uses Activator.CreateInstance to construct aggregate instances at runtime.")]
public sealed class EventArchiver : IEventArchiver
{
    private readonly EventSourcingDbContext _context;
    private readonly IEventArchiveStorage _storage;
    private readonly IEventTypeResolver _typeResolver;
    private readonly IEventSerializer _serializer;
    private readonly EventArchiveSettings _settings;
    private readonly ILogger<EventArchiver> _logger;

    /// <summary>Initializes a new instance of <see cref="EventArchiver"/>.</summary>
    public EventArchiver(
        EventSourcingDbContext context,
        IEventArchiveStorage storage,
        IEventTypeResolver typeResolver,
        IEventSerializer serializer,
        IOptions<EventArchiveSettings> settings,
        ILogger<EventArchiver> logger)
    {
        _context = context;
        _storage = storage;
        _typeResolver = typeResolver;
        _serializer = serializer;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<EventArchiveResult> ArchiveOldEventsAsync(CancellationToken ct = default)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-_settings.RetentionDays);

        // Find distinct streams that have events before the cutoff.
        // IgnoreQueryFilters so we operate across ALL tenants.
        var streams = await _context.Events
            .IgnoreQueryFilters()
            .Where(e => e.Timestamp < cutoff)
            .GroupBy(e => new { e.TenantId, e.AggregateType, e.AggregateId })
            .Select(g => new { g.Key.TenantId, g.Key.AggregateType, g.Key.AggregateId })
            .Take(_settings.BatchSize)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        if (streams.Count == 0)
        {
            _logger.LogInformation("No streams eligible for archiving (cutoff: {Cutoff})", cutoff);
            return new EventArchiveResult(0, 0, 0);
        }

        _logger.LogInformation(
            "Archiving {Count} stream(s) with events older than {Cutoff}",
            streams.Count, cutoff);

        int streamsArchived = 0;
        long eventsArchived = 0;
        int errors = 0;

        foreach (var stream in streams)
        {
            try
            {
                var archived = await ArchiveStreamAsync(
                    stream.TenantId, stream.AggregateType, stream.AggregateId, cutoff, ct)
                    .ConfigureAwait(false);

                streamsArchived++;
                eventsArchived += archived;
            }
            catch (Exception ex)
            {
                errors++;
                _logger.LogError(ex,
                    "Failed to archive stream {AggregateType}/{AggregateId} for tenant {TenantId}",
                    stream.AggregateType, stream.AggregateId, stream.TenantId);
            }
        }

        _logger.LogInformation(
            "Archive complete: {Streams} streams, {Events} events archived, {Errors} errors",
            streamsArchived, eventsArchived, errors);

        return new EventArchiveResult(streamsArchived, eventsArchived, errors);
    }

    private async Task<int> ArchiveStreamAsync(
        Guid tenantId,
        string aggregateType,
        Guid aggregateId,
        DateTimeOffset cutoff,
        CancellationToken ct)
    {
        // Query only the events eligible for archiving (before the cutoff).
        // Events after the cutoff remain as hot-tier data and do not need to be loaded.
        var toArchive = await _context.Events
            .IgnoreQueryFilters()
            .Where(e => e.TenantId == tenantId
                     && e.AggregateType == aggregateType
                     && e.AggregateId == aggregateId
                     && e.Timestamp < cutoff)
            .OrderBy(e => e.AggregateVersion)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        if (toArchive.Count == 0)
            return 0;

        var versionFrom = toArchive[0].AggregateVersion;   // list is already sorted ascending
        var versionTo   = toArchive[^1].AggregateVersion;

        // Ensure a snapshot exists at or above versionTo before deleting events.
        // If the snapshot cannot be guaranteed (e.g., aggregate type unresolvable or
        // event deserialization fails), abort this stream to prevent data loss: events
        // would be removed from the hot tier but the aggregate could not be reconstituted.
        var snapshotReady = await EnsureSnapshotAsync(
            tenantId, aggregateType, aggregateId, toArchive, versionTo, ct)
            .ConfigureAwait(false);

        if (!snapshotReady)
        {
            _logger.LogWarning(
                "Skipping archive for stream {AggregateType}/{AggregateId} (tenant {TenantId}): " +
                "could not guarantee a valid snapshot at version {VersionTo}.",
                aggregateType, aggregateId, tenantId, versionTo);
            return 0;
        }

        // Upload archived events to cold storage.
        var blobPath = await _storage.UploadEventsAsync(
            tenantId, aggregateType, aggregateId,
            versionFrom, versionTo, toArchive, ct)
            .ConfigureAwait(false);

        // Record the archive index entry.
        _context.ArchiveIndexes.Add(new EventArchiveIndex
        {
            TenantId    = tenantId,
            StreamId    = aggregateId,
            StreamType  = aggregateType,
            VersionFrom = versionFrom,
            VersionTo   = versionTo,
            EventCount  = toArchive.Count,
            BlobPath    = blobPath,
            ArchivedAt  = DateTimeOffset.UtcNow
        });

        // Delete the archived hot-tier event rows.
        _context.Events.RemoveRange(toArchive);

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogDebug(
            "Archived {Count} events (v{From}-v{To}) for {Type}/{Id} → {Path}",
            toArchive.Count, versionFrom, versionTo, aggregateType, aggregateId, blobPath);

        return toArchive.Count;
    }

    /// <summary>
    /// Ensures a snapshot exists at or above <paramref name="requiredVersion"/> before events
    /// are deleted from the hot tier.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when a valid snapshot is guaranteed; <see langword="false"/> when
    /// the aggregate type cannot be instantiated (stream is skipped, no data is deleted).
    /// Throws if an event type cannot be resolved or deserialized, because proceeding would
    /// produce an incomplete snapshot and corrupt subsequent aggregate loads.
    /// </returns>
    private async Task<bool> EnsureSnapshotAsync(
        Guid tenantId,
        string aggregateType,
        Guid aggregateId,
        List<EventRecord> candidateEvents,
        long requiredVersion,
        CancellationToken ct)
    {
        // Check whether an up-to-date snapshot already exists.
        var existing = await _context.Snapshots
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                s => s.AggregateId == aggregateId && s.TenantId == tenantId, ct)
            .ConfigureAwait(false);

        if (existing is not null && existing.Version >= requiredVersion)
            return true; // Snapshot already covers all events we are about to archive.

        // Instantiate a fresh aggregate.
        var aggregate = CreateAggregate(aggregateType);
        if (aggregate is null)
        {
            _logger.LogWarning(
                "Cannot create snapshot: aggregate type '{AggregateType}' could not be instantiated. " +
                "Ensure the assembly is loaded and the type name matches.",
                aggregateType);
            return false;
        }

        // If a previous snapshot exists, restore state from it so we only replay delta events.
        long replayFromVersion = 0;
        if (existing is not null)
        {
            aggregate.RestoreState(existing.Data);
            aggregate.LoadFromSnapshot(existing.Version);
            replayFromVersion = existing.Version;
        }

        // Replay only the events after the previous snapshot boundary.
        // Failing to resolve or deserialize any event is treated as an error: proceeding would
        // persist an incomplete snapshot and corrupt subsequent aggregate loads after archiving.
        foreach (var record in candidateEvents.Where(e => e.AggregateVersion > replayFromVersion))
        {
            var domainEventType = _typeResolver.Resolve(record.EventType);
            if (domainEventType is null)
            {
                _logger.LogError(
                    "Cannot build snapshot for {AggregateType}/{AggregateId}: event type '{EventType}' " +
                    "could not be resolved. Aborting archive for this stream to prevent data loss.",
                    aggregateType, aggregateId, record.EventType);
                return false;
            }

            var domainEvent = _serializer.Deserialize(record.Data, domainEventType);
            if (domainEvent is null)
            {
                _logger.LogError(
                    "Cannot build snapshot for {AggregateType}/{AggregateId}: failed to deserialize " +
                    "event '{EventType}' at version {Version}. Aborting archive for this stream to prevent data loss.",
                    aggregateType, aggregateId, record.EventType, record.AggregateVersion);
                return false;
            }

            aggregate.LoadFromHistory([domainEvent]);
        }

        var snapshotData = aggregate.GetSnapshotData();
        var snapshot = new Snapshot(
            AggregateId:   aggregateId,
            TenantId:      tenantId,
            AggregateType: aggregateType,
            Version:       requiredVersion,
            Data:          snapshotData,
            Timestamp:     DateTimeOffset.UtcNow);

        if (existing is not null)
            _context.Snapshots.Remove(existing);

        _context.Snapshots.Add(snapshot);
        // SaveChanges is called by the caller after all changes are staged.
        return true;
    }

    [RequiresUnreferencedCode("Uses Type.GetType to resolve aggregate type by short name.")]
    [RequiresDynamicCode("Uses Activator.CreateInstance to instantiate aggregate type.")]
    private static AggregateRoot<Guid>? CreateAggregate(string aggregateTypeName)
    {
        // Try to resolve the aggregate type from all loaded assemblies by short class name.
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return []; }
            })
            .FirstOrDefault(t =>
                t.Name == aggregateTypeName
                && !t.IsAbstract
                && typeof(AggregateRoot<Guid>).IsAssignableFrom(t));

        if (type is null)
            return null;

        return Activator.CreateInstance(type) as AggregateRoot<Guid>;
    }
}
