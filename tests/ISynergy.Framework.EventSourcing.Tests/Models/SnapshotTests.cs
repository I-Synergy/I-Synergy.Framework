using ISynergy.Framework.EventSourcing.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.Models;

[TestClass]
public class SnapshotTests
{
    [TestMethod]
    public void Constructor_SetsAllProperties()
    {
        var aggregateId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;
        var data = """{"customerName":"Alice","total":100}""";

        var snapshot = new Snapshot(
            AggregateId: aggregateId,
            TenantId: tenantId,
            AggregateType: "Order",
            Version: 10L,
            Data: data,
            Timestamp: timestamp);

        Assert.AreEqual(aggregateId, snapshot.AggregateId);
        Assert.AreEqual(tenantId, snapshot.TenantId);
        Assert.AreEqual("Order", snapshot.AggregateType);
        Assert.AreEqual(10L, snapshot.Version);
        Assert.AreEqual(data, snapshot.Data);
        Assert.AreEqual(timestamp, snapshot.Timestamp);
    }

    [TestMethod]
    public void TwoSnapshotsWithSameValues_AreEqual()
    {
        var aggregateId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new Snapshot(aggregateId, tenantId, "Order", 5L, "{}", timestamp);
        var second = new Snapshot(aggregateId, tenantId, "Order", 5L, "{}", timestamp);

        Assert.AreEqual(first, second);
    }

    [TestMethod]
    public void TwoSnapshotsWithDifferentAggregateIds_AreNotEqual()
    {
        var tenantId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new Snapshot(Guid.NewGuid(), tenantId, "Order", 5L, "{}", timestamp);
        var second = new Snapshot(Guid.NewGuid(), tenantId, "Order", 5L, "{}", timestamp);

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void TwoSnapshotsWithDifferentTenantIds_AreNotEqual()
    {
        var aggregateId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new Snapshot(aggregateId, Guid.NewGuid(), "Order", 5L, "{}", timestamp);
        var second = new Snapshot(aggregateId, Guid.NewGuid(), "Order", 5L, "{}", timestamp);

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void TwoSnapshotsWithDifferentVersions_AreNotEqual()
    {
        var aggregateId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new Snapshot(aggregateId, tenantId, "Order", 5L, "{}", timestamp);
        var second = new Snapshot(aggregateId, tenantId, "Order", 6L, "{}", timestamp);

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void TwoSnapshotsWithDifferentData_AreNotEqual()
    {
        var aggregateId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var timestamp = DateTimeOffset.UtcNow;

        var first = new Snapshot(aggregateId, tenantId, "Order", 5L, """{"name":"A"}""", timestamp);
        var second = new Snapshot(aggregateId, tenantId, "Order", 5L, """{"name":"B"}""", timestamp);

        Assert.AreNotEqual(first, second);
    }
}
