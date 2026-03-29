using ISynergy.Framework.EventSourcing.Aggregates;

namespace ISynergy.Framework.EventSourcing.Abstractions.Aggregates;

/// <summary>
/// Loads and saves event-sourced aggregates via an event store.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public interface IAggregateRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>, new()
{
    /// <summary>
    /// Loads an aggregate by replaying its event stream. Returns <c>null</c> when no events exist.
    /// </summary>
    Task<TAggregate?> LoadAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists all uncommitted events from the aggregate and marks them as committed.
    /// </summary>
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
}
