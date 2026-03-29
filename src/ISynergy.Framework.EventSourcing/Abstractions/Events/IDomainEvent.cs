namespace ISynergy.Framework.EventSourcing.Abstractions.Events;

/// <summary>
/// Marker interface for all domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Gets the unique identifier of this event instance.</summary>
    Guid EventId { get; }

    /// <summary>Gets the UTC timestamp when the event occurred.</summary>
    DateTimeOffset OccurredAt { get; }
}
