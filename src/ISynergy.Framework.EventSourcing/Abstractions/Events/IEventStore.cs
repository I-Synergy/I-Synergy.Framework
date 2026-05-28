using ISynergy.Framework.EventSourcing.Models;

namespace ISynergy.Framework.EventSourcing.Abstractions.Events;

/// <summary>
/// Provides append-only storage and retrieval of domain events, with optional snapshot support.
/// </summary>
/// <remarks>
/// All read methods are implicitly scoped to the current tenant. Concrete implementations
/// must resolve the tenant from an injected provider (e.g. <c>ITenantProvider</c>) and apply
/// it as a filter on every query — analogous to how <c>ApplyTenantFilters()</c> works in the
/// EF Core DbContext base.
/// </remarks>
public interface IEventStore
{
    /// <summary>
    /// Appends a new event to the store and returns the resulting aggregate version.
    /// </summary>
    /// <param name="tenantId">
    /// The tenant that owns this event stream. Must be stored alongside the event and used
    /// to scope all subsequent reads for this aggregate.
    /// </param>
    /// <param name="aggregateType">The aggregate type name (e.g. "Order").</param>
    /// <param name="aggregateId">The aggregate instance identifier.</param>
    /// <param name="expectedVersion">
    /// The version the caller expects the aggregate stream to be at before this event is written.
    /// Use the value of <see cref="ISynergy.Framework.EventSourcing.Abstractions.Aggregates.IAggregateRoot{TId}.Version"/> captured before new events were raised.
    /// The store must reject the append when the persisted version does not match, preventing
    /// lost-update conflicts when multiple writers target the same aggregate concurrently.
    /// </param>
    /// <param name="event">The domain event to persist. The event type name is derived from the runtime type.</param>
    /// <param name="metadataJson">Optional metadata serialized as a JSON string (e.g. correlation ID, causation ID) to be stored alongside the event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate version after this event was appended.</returns>
    Task<long> AppendEventAsync(
        Guid tenantId,
        string aggregateType,
        Guid aggregateId,
        long expectedVersion,
        IDomainEvent @event,
        string? metadataJson = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all events for a specific aggregate, optionally starting from a given version.
    /// Results are automatically scoped to the current tenant.
    /// </summary>
    Task<List<EventRecord>> GetEventsForAggregateAsync(
        string aggregateType,
        Guid aggregateId,
        long? fromVersion = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns events of a given type within an optional date range.
    /// Results are automatically scoped to the current tenant.
    /// </summary>
    Task<List<EventRecord>> GetEventsByTypeAsync(
        string eventType,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all events recorded on or after the specified timestamp.
    /// Results are automatically scoped to the current tenant.
    /// </summary>
    Task<List<EventRecord>> GetEventsSinceAsync(
        DateTimeOffset since,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the latest snapshot for an aggregate, or <c>null</c> if none exists.
    /// Results are automatically scoped to the current tenant.
    /// </summary>
    Task<Snapshot?> GetSnapshotAsync(
        Guid aggregateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a snapshot of an aggregate's state at a given version.
    /// The <see cref="Snapshot.TenantId"/> on the snapshot determines the tenant scope.
    /// </summary>
    Task SaveSnapshotAsync(
        Snapshot snapshot,
        CancellationToken cancellationToken = default);
}
