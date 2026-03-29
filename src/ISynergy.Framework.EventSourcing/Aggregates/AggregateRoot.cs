using ISynergy.Framework.EventSourcing.Abstractions.Aggregates;
using ISynergy.Framework.EventSourcing.Abstractions.Events;

namespace ISynergy.Framework.EventSourcing.Aggregates;

/// <summary>
/// Base class for event-sourced aggregate roots.
/// Subclasses call <see cref="Apply{TEvent}"/> to raise events and implement
/// <see cref="When"/> to mutate state in response to them.
/// </summary>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public abstract class AggregateRoot<TId> : IAggregateRoot<TId>
{
    private readonly List<IDomainEvent> _uncommittedEvents = [];

    /// <inheritdoc />
    public TId Id { get; protected set; } = default!;

    /// <inheritdoc />
    /// <remarks>
    /// Incremented only by <see cref="LoadFromHistory"/>. Events raised via <see cref="Apply{TEvent}"/>
    /// do not increment this value — they are tracked separately in the uncommitted events list.
    /// This design keeps <see cref="Version"/> equal to the last persisted version, which is the
    /// correct <c>expectedVersion</c> to pass to the event store when saving.
    /// </remarks>
    public long Version { get; private set; }

    /// <summary>
    /// Raises an event: mutates state via <see cref="When"/> and queues it for persistence.
    /// </summary>
    protected void Apply<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        When(@event);
        _uncommittedEvents.Add(@event);
    }

    /// <summary>
    /// Applies a domain event to the aggregate state. Use pattern-matching on the event type.
    /// </summary>
    protected abstract void When(IDomainEvent @event);

    /// <inheritdoc />
    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var e in events)
        {
            When(e);
            Version++;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IDomainEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

    /// <inheritdoc />
    public void MarkEventsAsCommitted() => _uncommittedEvents.Clear();
}
