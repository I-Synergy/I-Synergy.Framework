using ISynergy.Framework.EventSourcing.Events;

namespace Sample.EventSourcing.Api.Domain;

/// <summary>Raised when a new order is placed.</summary>
public record OrderPlaced(Guid OrderId, string CustomerName, decimal Total) : DomainEventBase;

/// <summary>Raised when an order is dispatched with a tracking number.</summary>
public record OrderShipped(Guid OrderId, string TrackingNumber) : DomainEventBase;

/// <summary>Raised when an order is cancelled before shipment.</summary>
public record OrderCancelled(Guid OrderId, string Reason) : DomainEventBase;

/// <summary>Raised when a pending order's customer name or total is updated.</summary>
public record OrderDetailsUpdated(Guid OrderId, string CustomerName, decimal Total) : DomainEventBase;
