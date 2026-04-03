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
using System.Text.Json;

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
        // Load ALL events for this stream (across all versions).
        var allEvents = await _context.Events
            .IgnoreQueryFilters()
            .Where(e => e.TenantId == tenantId
                     && e.AggregateType == aggregateType
                     && e.AggregateId == aggregateId)
            .OrderBy(e => e.AggregateVersion)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        // Identify which events to archive (before the cutoff).
        var toArchive = allEvents
            .Where(e => e.Timestamp < cutoff)
            .ToList();

        if (toArchive.Count == 0)
            return 0;

        var versionFrom = toArchive.Min(e => e.AggregateVersion);
        var versionTo   = toArchive.Max(e => e.AggregateVersion);

        // Ensure a snapshot exists at or above versionTo before deleting events.
        await EnsureSnapshotAsync(
            tenantId, aggregateType, aggregateId, allEvents, versionTo, ct)
            .ConfigureAwait(false);

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
        var toArchiveIds = toArchive.Select(e => e.EventId).ToHashSet();
        _context.Events.RemoveRange(
            allEvents.Where(e => toArchiveIds.Contains(e.EventId)));

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);

        _logger.LogDebug(
            "Archived {Count} events (v{From}-v{To}) for {Type}/{Id} → {Path}",
            toArchive.Count, versionFrom, versionTo, aggregateType, aggregateId, blobPath);

        return toArchive.Count;
    }

    private async Task EnsureSnapshotAsync(
        Guid tenantId,
        string aggregateType,
        Guid aggregateId,
        List<EventRecord> allEvents,
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
            return; // Snapshot covers all events we are about to archive.

        // Rebuild aggregate state up to requiredVersion.
        var eventsToReplay = allEvents
            .Where(e => e.AggregateVersion <= requiredVersion)
            .OrderBy(e => e.AggregateVersion)
            .ToList();

        var aggregate = CreateAggregate(aggregateType);
        if (aggregate is null)
        {
            _logger.LogWarning(
                "Cannot create snapshot: aggregate type '{AggregateType}' could not be instantiated. " +
                "Ensure the assembly is loaded and the type name matches.",
                aggregateType);
            return;
        }

        foreach (var record in eventsToReplay)
        {
            var domainEventType = _typeResolver.Resolve(record.EventType);
            if (domainEventType is null)
                continue;

            var domainEvent = _serializer.Deserialize(record.Data, domainEventType);
            if (domainEvent is null)
                continue;

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
