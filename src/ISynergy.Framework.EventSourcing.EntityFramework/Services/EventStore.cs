using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.EntityFramework.Abstractions;
using ISynergy.Framework.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

/// <summary>
/// EF Core implementation of <see cref="IEventStore"/>.
/// Appends events to the <c>EventStore</c> table and reads them back with automatic
/// tenant scoping via <see cref="EventSourcingDbContext"/>'s global query filter.
/// </summary>
/// <remarks>
/// Register as scoped (matches <see cref="EventSourcingDbContext"/> lifetime):
/// <code>
/// services.AddScoped&lt;IEventStore, EventStore&gt;();
/// </code>
/// </remarks>
public sealed class EventStore : IEventStore
{
    private readonly EventSourcingDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IEventTypeResolver _typeResolver;
    private readonly IEventSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of <see cref="EventStore"/>.
    /// </summary>
    public EventStore(
        EventSourcingDbContext context,
        ITenantService tenantService,
        IEventTypeResolver typeResolver,
        IEventSerializer serializer)
    {
        _context = context;
        _tenantService = tenantService;
        _typeResolver = typeResolver;
        _serializer = serializer;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Performs an optimistic concurrency check before inserting: if the current highest version
    /// for <paramref name="aggregateId"/> does not match <paramref name="expectedVersion"/>, an
    /// <see cref="InvalidOperationException"/> is thrown. The unique DB index on
    /// <c>(AggregateId, AggregateVersion)</c> provides a second line of defence against races.
    /// </remarks>
    public async Task<long> AppendEventAsync(
        Guid tenantId,
        string aggregateType,
        Guid aggregateId,
        long expectedVersion,
        IDomainEvent @event,
        string? metadataJson = null,
        CancellationToken cancellationToken = default)
    {
        // Bypass the global tenant query filter so the version check uses the explicit tenantId.
        var hotVersion = await _context.Events
            .IgnoreQueryFilters()
            .Where(e => e.AggregateId == aggregateId && e.TenantId == tenantId)
            .OrderByDescending(e => e.AggregateVersion)
            .Select(e => (long?)e.AggregateVersion)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        // If no hot events exist the aggregate may have been archived: the snapshot
        // records the highest version that was moved to cold storage, which is the
        // true current version for concurrency purposes.
        var currentVersion = hotVersion ?? await _context.Snapshots
            .IgnoreQueryFilters()
            .Where(s => s.AggregateId == aggregateId && s.TenantId == tenantId)
            .Select(s => (long?)s.Version)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ?? 0L;

        if (currentVersion != expectedVersion)
            throw new InvalidOperationException(
                $"Optimistic concurrency conflict for aggregate {aggregateId}: " +
                $"expected version {expectedVersion} but current version is {currentVersion}.");

        var newVersion = expectedVersion + 1;
        var eventTypeName = _typeResolver.GetTypeName(@event.GetType());
        var data = _serializer.Serialize(@event);

        var record = new EventRecord(
            EventId: @event.EventId,
            TenantId: tenantId,
            AggregateType: aggregateType,
            AggregateId: aggregateId,
            AggregateVersion: newVersion,
            EventType: eventTypeName,
            Data: data,
            Metadata: metadataJson,
            Timestamp: DateTimeOffset.UtcNow,
            UserId: _tenantService.UserName is { Length: > 0 } u ? u : null);

        _context.Events.Add(record);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return newVersion;
    }

    /// <inheritdoc />
    public async Task<List<EventRecord>> GetEventsForAggregateAsync(
        string aggregateType,
        Guid aggregateId,
        long? fromVersion = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Events
            .Where(e => e.AggregateType == aggregateType && e.AggregateId == aggregateId);

        if (fromVersion.HasValue)
            query = query.Where(e => e.AggregateVersion >= fromVersion.Value);

        return await query
            .OrderBy(e => e.AggregateVersion)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<EventRecord>> GetEventsByTypeAsync(
        string eventType,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Events.Where(e => e.EventType == eventType);

        if (from.HasValue)
            query = query.Where(e => e.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.Timestamp <= to.Value);

        return await query
            .OrderBy(e => e.Timestamp)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<EventRecord>> GetEventsSinceAsync(
        DateTimeOffset since,
        CancellationToken cancellationToken = default) =>
        await _context.Events
            .Where(e => e.Timestamp >= since)
            .OrderBy(e => e.Timestamp)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Snapshot?> GetSnapshotAsync(
        Guid aggregateId,
        CancellationToken cancellationToken = default) =>
        await _context.Snapshots
            .FirstOrDefaultAsync(s => s.AggregateId == aggregateId, cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// Performs an upsert: if a snapshot already exists for the same
    /// <see cref="Snapshot.AggregateId"/> and current tenant, the old row is replaced.
    /// </remarks>
    public async Task SaveSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Snapshots
            .FirstOrDefaultAsync(s => s.AggregateId == snapshot.AggregateId, cancellationToken)
            .ConfigureAwait(false);

        if (existing is not null)
            _context.Snapshots.Remove(existing);

        _context.Snapshots.Add(snapshot);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
