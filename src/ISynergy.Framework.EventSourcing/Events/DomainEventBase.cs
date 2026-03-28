using ISynergy.Framework.EventSourcing.Abstractions.Events;

namespace ISynergy.Framework.EventSourcing.Events;

/// <summary>
/// Base record for all domain events. Provides a unique <see cref="EventId"/> and <see cref="OccurredAt"/> timestamp.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
