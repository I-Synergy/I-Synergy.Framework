using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.Aggregates;
using System.Text.Json;

namespace Sample.EventSourcing.Api.Domain;

/// <summary>
/// Event-sourced Order aggregate root.
/// All state changes are expressed as domain events applied via <see cref="AggregateRoot{TId}.Apply{TEvent}"/>.
/// </summary>
public sealed class Order : AggregateRoot<Guid>
{
    public string CustomerName { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public string? TrackingNumber { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTimeOffset PlacedAt { get; private set; }

    /// <summary>Places a new order, raising <see cref="OrderPlaced"/>.</summary>
    public void Place(Guid orderId, string customerName, decimal total) =>
        Apply(new OrderPlaced(orderId, customerName, total));

    /// <summary>Ships the order, raising <see cref="OrderShipped"/>.</summary>
    /// <exception cref="InvalidOperationException">Order is not in Pending status.</exception>
    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot ship an order with status {Status}.");

        Apply(new OrderShipped(Id, trackingNumber));
    }

    /// <summary>Cancels the order, raising <see cref="OrderCancelled"/>.</summary>
    /// <exception cref="InvalidOperationException">Order is already shipped or cancelled.</exception>
    public void Cancel(string reason)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot cancel an order with status {Status}.");

        Apply(new OrderCancelled(Id, reason));
    }

    /// <summary>Updates the customer name and/or total, raising <see cref="OrderDetailsUpdated"/>.</summary>
    /// <exception cref="InvalidOperationException">Order is not in Pending status.</exception>
    public void UpdateDetails(string customerName, decimal total)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot edit an order with status {Status}.");

        Apply(new OrderDetailsUpdated(Id, customerName, total));
    }

    /// <inheritdoc />
    public override string GetSnapshotData() =>
        JsonSerializer.Serialize(new OrderSnapshot(Id, CustomerName, Total, Status, TrackingNumber, CancellationReason, PlacedAt));

    /// <inheritdoc />
    public override void RestoreState(string json)
    {
        var snap = JsonSerializer.Deserialize<OrderSnapshot>(json);
        if (snap is null) return;

        Id = snap.Id;
        CustomerName = snap.CustomerName;
        Total = snap.Total;
        Status = snap.Status;
        TrackingNumber = snap.TrackingNumber;
        CancellationReason = snap.CancellationReason;
        PlacedAt = snap.PlacedAt;
    }

    /// <inheritdoc />
    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case OrderPlaced e:
                Id = e.OrderId;
                CustomerName = e.CustomerName;
                Total = e.Total;
                Status = OrderStatus.Pending;
                PlacedAt = e.OccurredAt;
                break;

            case OrderShipped e:
                TrackingNumber = e.TrackingNumber;
                Status = OrderStatus.Shipped;
                break;

            case OrderCancelled e:
                CancellationReason = e.Reason;
                Status = OrderStatus.Cancelled;
                break;

            case OrderDetailsUpdated e:
                CustomerName = e.CustomerName;
                Total = e.Total;
                break;
        }
    }
}

/// <summary>Mutable DTO used to serialize/deserialize <see cref="Order"/> snapshot state.</summary>
internal sealed record OrderSnapshot(
    Guid Id,
    string CustomerName,
    decimal Total,
    OrderStatus Status,
    string? TrackingNumber,
    string? CancellationReason,
    DateTimeOffset PlacedAt);
