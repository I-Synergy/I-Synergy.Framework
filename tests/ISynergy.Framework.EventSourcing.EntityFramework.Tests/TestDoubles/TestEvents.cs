using ISynergy.Framework.EventSourcing.Events;

namespace ISynergy.Framework.EventSourcing.EntityFramework.TestDoubles;

internal record OrderPlaced(Guid OrderId, string CustomerName, decimal Total) : DomainEventBase;
internal record OrderShipped(Guid OrderId, string TrackingNumber) : DomainEventBase;
internal record OrderCancelled(Guid OrderId, string Reason) : DomainEventBase;
