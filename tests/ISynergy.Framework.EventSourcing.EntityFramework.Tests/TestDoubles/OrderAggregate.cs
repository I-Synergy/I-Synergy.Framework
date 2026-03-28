using ISynergy.Framework.EventSourcing.Abstractions.Events;
using ISynergy.Framework.EventSourcing.Aggregates;

namespace ISynergy.Framework.EventSourcing.EntityFramework.TestDoubles;

internal sealed class OrderAggregate : AggregateRoot<Guid>
{
    public string? CustomerName { get; private set; }
    public decimal Total { get; private set; }
    public string? TrackingNumber { get; private set; }
    public bool IsCancelled { get; private set; }

    public void Place(Guid orderId, string customerName, decimal total) =>
        Apply(new OrderPlaced(orderId, customerName, total));

    public void Ship(string trackingNumber) =>
        Apply(new OrderShipped(Id, trackingNumber));

    public void Cancel(string reason) =>
        Apply(new OrderCancelled(Id, reason));

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case OrderPlaced e:
                Id = e.OrderId;
                CustomerName = e.CustomerName;
                Total = e.Total;
                break;
            case OrderShipped e:
                TrackingNumber = e.TrackingNumber;
                break;
            case OrderCancelled:
                IsCancelled = true;
                break;
        }
    }
}
