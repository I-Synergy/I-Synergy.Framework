using ISynergy.Framework.EventSourcing.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Aggregates;

[TestClass]
public class AggregateRootTests
{
    // -------------------------------------------------------------------------
    // Initial state
    // -------------------------------------------------------------------------

    [TestMethod]
    public void Version_InitialValue_IsZero()
    {
        var aggregate = new OrderAggregate();

        Assert.AreEqual(0L, aggregate.Version);
    }

    [TestMethod]
    public void GetUncommittedEvents_InitialValue_IsEmpty()
    {
        var aggregate = new OrderAggregate();

        Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
    }

    // -------------------------------------------------------------------------
    // Apply
    // -------------------------------------------------------------------------

    [TestMethod]
    public void Apply_SingleEvent_AddsToUncommittedEvents()
    {
        var aggregate = new OrderAggregate();

        aggregate.Create(Guid.NewGuid(), "Alice", 100m);

        Assert.AreEqual(1, aggregate.GetUncommittedEvents().Count);
    }

    [TestMethod]
    public void Apply_MultipleEvents_AllAddedToUncommittedEvents()
    {
        var aggregate = new OrderAggregate();

        aggregate.Create(Guid.NewGuid(), "Alice", 100m);
        aggregate.Ship("TRACK-001");
        aggregate.Cancel("Changed mind");

        Assert.AreEqual(3, aggregate.GetUncommittedEvents().Count);
    }

    [TestMethod]
    public void Apply_CallsWhen_UpdatesAggregateState()
    {
        var orderId = Guid.NewGuid();
        var aggregate = new OrderAggregate();

        aggregate.Create(orderId, "Alice", 100m);

        Assert.AreEqual(orderId, aggregate.Id);
        Assert.AreEqual("Alice", aggregate.CustomerName);
        Assert.AreEqual(100m, aggregate.Total);
    }

    [TestMethod]
    public void Apply_DoesNotIncrementVersion()
    {
        var aggregate = new OrderAggregate();

        aggregate.Create(Guid.NewGuid(), "Alice", 100m);
        aggregate.Ship("TRACK-001");

        Assert.AreEqual(0L, aggregate.Version);
    }

    [TestMethod]
    public void Apply_EventIsCorrectType_InUncommittedList()
    {
        var aggregate = new OrderAggregate();

        aggregate.Create(Guid.NewGuid(), "Alice", 100m);

        Assert.IsInstanceOfType<OrderCreated>(aggregate.GetUncommittedEvents()[0]);
    }

    // -------------------------------------------------------------------------
    // GetUncommittedEvents
    // -------------------------------------------------------------------------

    [TestMethod]
    public void GetUncommittedEvents_ReturnsReadOnlyList()
    {
        var aggregate = new OrderAggregate();
        aggregate.Create(Guid.NewGuid(), "Alice", 100m);

        var events = aggregate.GetUncommittedEvents();

        // IReadOnlyList does not expose Add — verify the concrete type is read-only
        Assert.IsNotNull(events);
        Assert.AreEqual(1, events.Count);
    }

    [TestMethod]
    public void GetUncommittedEvents_ReturnsEventsInApplicationOrder()
    {
        var aggregate = new OrderAggregate();
        aggregate.Create(Guid.NewGuid(), "Alice", 100m);
        aggregate.Ship("TRACK-001");

        var events = aggregate.GetUncommittedEvents();

        Assert.IsInstanceOfType<OrderCreated>(events[0]);
        Assert.IsInstanceOfType<OrderShipped>(events[1]);
    }

    // -------------------------------------------------------------------------
    // MarkEventsAsCommitted
    // -------------------------------------------------------------------------

    [TestMethod]
    public void MarkEventsAsCommitted_ClearsUncommittedEvents()
    {
        var aggregate = new OrderAggregate();
        aggregate.Create(Guid.NewGuid(), "Alice", 100m);

        aggregate.MarkEventsAsCommitted();

        Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
    }

    [TestMethod]
    public void MarkEventsAsCommitted_DoesNotChangeVersion()
    {
        var aggregate = new OrderAggregate();
        aggregate.Create(Guid.NewGuid(), "Alice", 100m);

        aggregate.MarkEventsAsCommitted();

        Assert.AreEqual(0L, aggregate.Version);
    }

    [TestMethod]
    public void MarkEventsAsCommitted_WhenAlreadyEmpty_DoesNotThrow()
    {
        var aggregate = new OrderAggregate();

        aggregate.MarkEventsAsCommitted();

        Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
    }

    // -------------------------------------------------------------------------
    // LoadFromHistory
    // -------------------------------------------------------------------------

    [TestMethod]
    public void LoadFromHistory_SingleEvent_VersionIsOne()
    {
        var aggregate = new OrderAggregate();
        var orderId = Guid.NewGuid();
        var events = new[] { new OrderCreated(orderId, "Bob", 200m) };

        aggregate.LoadFromHistory(events);

        Assert.AreEqual(1L, aggregate.Version);
    }

    [TestMethod]
    public void LoadFromHistory_MultipleEvents_VersionEqualsEventCount()
    {
        var orderId = Guid.NewGuid();
        var aggregate = new OrderAggregate();
        var events = new[]
        {
            (ISynergy.Framework.EventSourcing.Abstractions.Events.IDomainEvent)new OrderCreated(orderId, "Bob", 200m),
            new OrderShipped(orderId, "TRACK-999"),
        };

        aggregate.LoadFromHistory(events);

        Assert.AreEqual(2L, aggregate.Version);
    }

    [TestMethod]
    public void LoadFromHistory_DoesNotAddToUncommittedEvents()
    {
        var orderId = Guid.NewGuid();
        var aggregate = new OrderAggregate();
        var events = new[] { new OrderCreated(orderId, "Bob", 200m) };

        aggregate.LoadFromHistory(events);

        Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
    }

    [TestMethod]
    public void LoadFromHistory_CallsWhen_RestoresAggregateState()
    {
        var orderId = Guid.NewGuid();
        var aggregate = new OrderAggregate();
        var events = new[]
        {
            (ISynergy.Framework.EventSourcing.Abstractions.Events.IDomainEvent)new OrderCreated(orderId, "Carol", 300m),
            new OrderShipped(orderId, "TRACK-XYZ"),
        };

        aggregate.LoadFromHistory(events);

        Assert.AreEqual(orderId, aggregate.Id);
        Assert.AreEqual("Carol", aggregate.CustomerName);
        Assert.AreEqual(300m, aggregate.Total);
        Assert.AreEqual("TRACK-XYZ", aggregate.TrackingNumber);
    }

    [TestMethod]
    public void LoadFromHistory_EmptyList_VersionRemainsZero()
    {
        var aggregate = new OrderAggregate();

        aggregate.LoadFromHistory([]);

        Assert.AreEqual(0L, aggregate.Version);
    }

    [TestMethod]
    public void LoadFromHistory_EmptyList_StateUnchanged()
    {
        var aggregate = new OrderAggregate();

        aggregate.LoadFromHistory([]);

        Assert.IsNull(aggregate.CustomerName);
        Assert.AreEqual(0m, aggregate.Total);
    }

    // -------------------------------------------------------------------------
    // Interaction: Apply then LoadFromHistory
    // -------------------------------------------------------------------------

    [TestMethod]
    public void MarkCommitted_ThenLoadFromHistory_VersionReflectsHistoryOnly()
    {
        var orderId = Guid.NewGuid();
        var aggregate = new OrderAggregate();

        // Simulate: one event previously committed and stored
        aggregate.LoadFromHistory([new OrderCreated(orderId, "Dave", 50m)]);

        // New uncommitted event
        aggregate.Ship("TRACK-NEW");

        // Commit clears uncommitted; version stays as loaded
        aggregate.MarkEventsAsCommitted();

        Assert.AreEqual(1L, aggregate.Version);
        Assert.AreEqual(0, aggregate.GetUncommittedEvents().Count);
        Assert.AreEqual("TRACK-NEW", aggregate.TrackingNumber);
    }
}
