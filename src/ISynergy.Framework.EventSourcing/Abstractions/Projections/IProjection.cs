using ISynergy.Framework.EventSourcing.Abstractions.Events;

namespace ISynergy.Framework.EventSourcing.Abstractions.Projections;

/// <summary>
/// Handles a single domain event type to update a read model.
/// </summary>
/// <typeparam name="TEvent">The domain event type this projection handles.</typeparam>
public interface IProjection<TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Applies the event to the read model.
    /// </summary>
    Task ProjectAsync(TEvent @event, CancellationToken cancellationToken = default);
}
