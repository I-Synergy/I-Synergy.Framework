using ISynergy.Framework.EventSourcing.Abstractions.Events;

namespace ISynergy.Framework.EventSourcing.Abstractions.Aggregates;

/// <summary>
/// Represents the root of a domain aggregate that encapsulates business rules and emits domain events.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IAggregateRoot<TId>
{
    /// <summary>Gets the aggregate identifier.</summary>
    TId Id { get; }

    /// <summary>
    /// Gets the version at which this aggregate was last loaded from the event store.
    /// Represents the number of events replayed via <see cref="LoadFromHistory"/>.
    /// Use this value as <c>expectedVersion</c> when appending new events to enforce optimistic concurrency.
    /// </summary>
    long Version { get; }

    /// <summary>Returns events raised since the last commit.</summary>
    IReadOnlyList<IDomainEvent> GetUncommittedEvents();

    /// <summary>Clears the uncommitted event list after they have been persisted.</summary>
    void MarkEventsAsCommitted();

    /// <summary>Replays a historical event stream to restore aggregate state.</summary>
    void LoadFromHistory(IEnumerable<IDomainEvent> events);
}
