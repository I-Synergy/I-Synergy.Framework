using ISynergy.Framework.EventSourcing.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Events;

[TestClass]
public class DomainEventBaseTests
{
    [TestMethod]
    public void EventId_IsNonEmptyGuid()
    {
        var @event = new OrderCreated(Guid.NewGuid(), "Alice", 99.99m);

        Assert.AreNotEqual(Guid.Empty, @event.EventId);
    }

    [TestMethod]
    public void OccurredAt_IsApproximatelyUtcNow()
    {
        var before = DateTimeOffset.UtcNow;
        var @event = new OrderCreated(Guid.NewGuid(), "Alice", 99.99m);
        var after = DateTimeOffset.UtcNow;

        Assert.IsTrue(@event.OccurredAt >= before);
        Assert.IsTrue(@event.OccurredAt <= after);
    }

    [TestMethod]
    public void OccurredAt_IsUtcOffset()
    {
        var @event = new OrderCreated(Guid.NewGuid(), "Alice", 99.99m);

        Assert.AreEqual(TimeSpan.Zero, @event.OccurredAt.Offset);
    }

    [TestMethod]
    public void TwoInstances_HaveDifferentEventIds()
    {
        var first = new OrderCreated(Guid.NewGuid(), "Alice", 99.99m);
        var second = new OrderCreated(Guid.NewGuid(), "Bob", 49.99m);

        Assert.AreNotEqual(first.EventId, second.EventId);
    }

    [TestMethod]
    public void EventId_IsStableOnSameInstance()
    {
        var @event = new OrderCreated(Guid.NewGuid(), "Alice", 99.99m);

        Assert.AreEqual(@event.EventId, @event.EventId);
    }

    [TestMethod]
    public void OccurredAt_IsStableOnSameInstance()
    {
        var @event = new OrderCreated(Guid.NewGuid(), "Alice", 99.99m);

        Assert.AreEqual(@event.OccurredAt, @event.OccurredAt);
    }
}
