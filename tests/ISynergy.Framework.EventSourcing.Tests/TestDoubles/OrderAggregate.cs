using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.Aggregates;

namespace ISynergy.Framework.EventSourcing.TestDoubles;

/// <summary>
/// Concrete aggregate used in tests to exercise <see cref="AggregateRoot{TId}"/> behaviour.
/// </summary>
internal class OrderAggregate : AggregateRoot<Guid>
{
    public string? CustomerName { get; private set; }
    public decimal Total { get; private set; }
    public string? TrackingNumber { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }

    public void Create(Guid orderId, string customerName, decimal total)
        => Apply(new OrderCreated(orderId, customerName, total));

    public void Ship(string trackingNumber)
        => Apply(new OrderShipped(Id, trackingNumber));

    public void Cancel(string reason)
        => Apply(new OrderCancelled(Id, reason));

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case OrderCreated e:
                Id = e.OrderId;
                CustomerName = e.CustomerName;
                Total = e.Total;
                break;
            case OrderShipped e:
                TrackingNumber = e.TrackingNumber;
                break;
            case OrderCancelled e:
                IsCancelled = true;
                CancellationReason = e.Reason;
                break;
        }
    }
}
