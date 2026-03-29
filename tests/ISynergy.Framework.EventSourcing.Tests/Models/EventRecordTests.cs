using ISynergy.Framework.EventSourcing.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Models;

[TestClass]
public class EventRecordTests
{
    private static EventRecord CreateSampleRecord(string? metadata = null, string? userId = "user-123") =>
        new(
            EventId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            AggregateType: "Order",
            AggregateId: Guid.NewGuid(),
            AggregateVersion: 1L,
            EventType: "OrderCreated",
            Data: """{"orderId":"abc","customerName":"Alice"}""",
            Metadata: metadata,
            Timestamp: DateTimeOffset.UtcNow,
            UserId: userId);

    [TestMethod]
    public void Constructor_SetsAllProperties()
    {
        var eventId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var record = new EventRecord(
            EventId: eventId,
            TenantId: tenantId,
            AggregateType: "Order",
            AggregateId: aggregateId,
            AggregateVersion: 5L,
            EventType: "OrderShipped",
            Data: """{"trackingNumber":"TRACK-001"}""",
            Metadata: """{"correlationId":"xyz"}""",
            Timestamp: timestamp,
            UserId: "user-456");

        Assert.AreEqual(eventId, record.EventId);
        Assert.AreEqual(tenantId, record.TenantId);
        Assert.AreEqual("Order", record.AggregateType);
        Assert.AreEqual(aggregateId, record.AggregateId);
        Assert.AreEqual(5L, record.AggregateVersion);
        Assert.AreEqual("OrderShipped", record.EventType);
        Assert.AreEqual("""{"trackingNumber":"TRACK-001"}""", record.Data);
        Assert.AreEqual("""{"correlationId":"xyz"}""", record.Metadata);
        Assert.AreEqual(timestamp, record.Timestamp);
        Assert.AreEqual("user-456", record.UserId);
    }

    [TestMethod]
    public void Metadata_CanBeNull()
    {
        var record = CreateSampleRecord(metadata: null);

        Assert.IsNull(record.Metadata);
    }

    [TestMethod]
    public void Metadata_CanBePopulated()
    {
        var record = CreateSampleRecord(metadata: """{"correlationId":"abc"}""");

        Assert.IsNotNull(record.Metadata);
        Assert.AreEqual("""{"correlationId":"abc"}""", record.Metadata);
    }

    [TestMethod]
    public void UserId_CanBeNull_ForSystemGeneratedEvents()
    {
        var record = CreateSampleRecord(userId: null);

        Assert.IsNull(record.UserId);
    }

    [TestMethod]
    public void UserId_CanBePopulated_ForUserTriggeredEvents()
    {
        var record = CreateSampleRecord(userId: "user-123");

        Assert.AreEqual("user-123", record.UserId);
    }

    [TestMethod]
    public void TwoRecordsWithSameValues_AreEqual()
    {
        var eventId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new EventRecord(eventId, tenantId, "Order", aggregateId, 1L, "OrderCreated", "{}", null, timestamp, "user-1");
        var second = new EventRecord(eventId, tenantId, "Order", aggregateId, 1L, "OrderCreated", "{}", null, timestamp, "user-1");

        Assert.AreEqual(first, second);
    }

    [TestMethod]
    public void TwoRecordsWithDifferentEventIds_AreNotEqual()
    {
        var tenantId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new EventRecord(Guid.NewGuid(), tenantId, "Order", aggregateId, 1L, "OrderCreated", "{}", null, timestamp, "user-1");
        var second = new EventRecord(Guid.NewGuid(), tenantId, "Order", aggregateId, 1L, "OrderCreated", "{}", null, timestamp, "user-1");

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void TwoRecordsWithDifferentVersions_AreNotEqual()
    {
        var eventId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new EventRecord(eventId, tenantId, "Order", aggregateId, 1L, "OrderCreated", "{}", null, timestamp, "user-1");
        var second = new EventRecord(eventId, tenantId, "Order", aggregateId, 2L, "OrderCreated", "{}", null, timestamp, "user-1");

        Assert.AreNotEqual(first, second);
    }
}
