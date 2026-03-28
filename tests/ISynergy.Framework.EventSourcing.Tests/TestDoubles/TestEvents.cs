using ISynergy.Framework.EventSourcing.Events;

namespace ISynergy.Framework.EventSourcing.TestDoubles;

/// <summary>
/// Domain event representing an order being created.
/// </summary>
internal record OrderCreated(Guid OrderId, string CustomerName, decimal Total) : DomainEventBase;

/// <summary>
/// Domain event representing an order being shipped.
/// </summary>
internal record OrderShipped(Guid OrderId, string TrackingNumber) : DomainEventBase;

/// <summary>
/// Domain event representing an order being cancelled.
/// </summary>
internal record OrderCancelled(Guid OrderId, string Reason) : DomainEventBase;
