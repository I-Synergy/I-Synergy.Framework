using ISynergy.Framework.EventSourcing.EntityFramework.Services;
using ISynergy.Framework.EventSourcing.EntityFramework.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

[TestClass]
public class EventStoreTests
{
    private TestTenantService _tenantService = null!;
    private EventSourcingDbContext _context = null!;
    private EventStore _store = null!;
    private DefaultEventTypeResolver _resolver = null!;
    private JsonReflectionEventSerializer _serializer = null!;

    [TestInitialize]
    public void Setup()
    {
        _tenantService = new TestTenantService();
        _context = EventSourcingDbContextFactory.Create(_tenantService);
        _resolver = new DefaultEventTypeResolver();
        _serializer = new JsonReflectionEventSerializer();
        _store = new EventStore(_context, _tenantService, _resolver, _serializer);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    // ── AppendEventAsync ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task AppendEventAsync_FirstEvent_ReturnsVersionOne()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();
        var @event = new OrderPlaced(aggregateId, "Alice", 99.99m);

        var version = await _store.AppendEventAsync(
            tenantId, "OrderAggregate", aggregateId, expectedVersion: 0, @event);

        Assert.AreEqual(1L, version);
    }

    [TestMethod]
    public async Task AppendEventAsync_SecondEvent_ReturnsVersionTwo()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 99.99m));

        var version = await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 1,
            new OrderShipped(aggregateId, "TRACK-001"));

        Assert.AreEqual(2L, version);
    }

    [TestMethod]
    public async Task AppendEventAsync_WrongExpectedVersion_ThrowsInvalidOperationException()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 99.99m));

        // expectedVersion = 0 again instead of 1 — concurrency conflict
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
                new OrderShipped(aggregateId, "TRACK-001")));
    }

    [TestMethod]
    public async Task AppendEventAsync_PersistsCorrectEventRecord()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();
        var @event = new OrderPlaced(aggregateId, "Bob", 200m);

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0, @event);

        var records = await _store.GetEventsForAggregateAsync("OrderAggregate", aggregateId);
        Assert.AreEqual(1, records.Count);

        var record = records[0];
        Assert.AreEqual(tenantId, record.TenantId);
        Assert.AreEqual(aggregateId, record.AggregateId);
        Assert.AreEqual(1L, record.AggregateVersion);
        Assert.AreEqual("OrderAggregate", record.AggregateType);
        Assert.IsNotNull(record.Data);
        Assert.AreEqual("test-user", record.UserId);
    }

    // ── GetEventsForAggregateAsync ────────────────────────────────────────────

    [TestMethod]
    public async Task GetEventsForAggregateAsync_ReturnsEventsInVersionOrder()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 50m));
        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 1,
            new OrderShipped(aggregateId, "T1"));
        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 2,
            new OrderCancelled(aggregateId, "Wrong item"));

        var records = await _store.GetEventsForAggregateAsync("OrderAggregate", aggregateId);

        Assert.AreEqual(3, records.Count);
        Assert.AreEqual(1L, records[0].AggregateVersion);
        Assert.AreEqual(2L, records[1].AggregateVersion);
        Assert.AreEqual(3L, records[2].AggregateVersion);
    }

    [TestMethod]
    public async Task GetEventsForAggregateAsync_WithFromVersion_ReturnsSubset()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 50m));
        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 1,
            new OrderShipped(aggregateId, "T1"));

        var records = await _store.GetEventsForAggregateAsync("OrderAggregate", aggregateId, fromVersion: 2);

        Assert.AreEqual(1, records.Count);
        Assert.AreEqual(2L, records[0].AggregateVersion);
    }

    [TestMethod]
    public async Task GetEventsForAggregateAsync_ScopedToCurrentTenant()
    {
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();

        // Write as tenant A
        _tenantService.SetTenant(tenantAId);
        await _store.AppendEventAsync(tenantAId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 50m));

        // Read as tenant B — should see nothing
        _tenantService.SetTenant(tenantBId);
        var records = await _store.GetEventsForAggregateAsync("OrderAggregate", aggregateId);

        Assert.AreEqual(0, records.Count);
    }

    [TestMethod]
    public async Task GetEventsForAggregateAsync_ReturnsEmpty_WhenNoEventsExist()
    {
        var records = await _store.GetEventsForAggregateAsync("OrderAggregate", Guid.NewGuid());

        Assert.AreEqual(0, records.Count);
    }

    // ── GetEventsByTypeAsync ──────────────────────────────────────────────────

    [TestMethod]
    public async Task GetEventsByTypeAsync_ReturnsMatchingEventType()
    {
        var tenantId = _tenantService.TenantId;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var placedTypeName = _resolver.GetTypeName(typeof(OrderPlaced));

        await _store.AppendEventAsync(tenantId, "OrderAggregate", id1, 0, new OrderPlaced(id1, "Alice", 50m));
        await _store.AppendEventAsync(tenantId, "OrderAggregate", id1, 1, new OrderShipped(id1, "T1"));
        await _store.AppendEventAsync(tenantId, "OrderAggregate", id2, 0, new OrderPlaced(id2, "Bob", 100m));

        var records = await _store.GetEventsByTypeAsync(placedTypeName);

        Assert.AreEqual(2, records.Count);
        Assert.IsTrue(records.All(r => r.EventType == placedTypeName));
    }

    [TestMethod]
    public async Task GetEventsByTypeAsync_WithDateRange_FiltersCorrectly()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();
        var placedTypeName = _resolver.GetTypeName(typeof(OrderPlaced));

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 50m));

        var future = DateTimeOffset.UtcNow.AddHours(1);
        var records = await _store.GetEventsByTypeAsync(placedTypeName, to: DateTimeOffset.UtcNow.AddSeconds(-1));

        Assert.AreEqual(0, records.Count, "Event after 'to' cutoff should be excluded.");
    }

    // ── GetEventsSinceAsync ───────────────────────────────────────────────────

    [TestMethod]
    public async Task GetEventsSinceAsync_ReturnsEventsAfterTimestamp()
    {
        var tenantId = _tenantService.TenantId;
        var aggregateId = Guid.NewGuid();
        var before = DateTimeOffset.UtcNow;

        await _store.AppendEventAsync(tenantId, "OrderAggregate", aggregateId, 0,
            new OrderPlaced(aggregateId, "Alice", 50m));

        var records = await _store.GetEventsSinceAsync(before);

        Assert.IsTrue(records.Count >= 1);
    }

    // ── Snapshot ──────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetSnapshotAsync_ReturnsNull_WhenNoSnapshotExists()
    {
        var snapshot = await _store.GetSnapshotAsync(Guid.NewGuid());

        Assert.IsNull(snapshot);
    }

    [TestMethod]
    public async Task SaveSnapshotAsync_AndGet_ReturnsSavedSnapshot()
    {
        var aggregateId = Guid.NewGuid();
        var snapshot = new Models.Snapshot(
            aggregateId, _tenantService.TenantId, "OrderAggregate", 5L,
            """{"customerName":"Alice","total":100}""", DateTimeOffset.UtcNow);

        await _store.SaveSnapshotAsync(snapshot);
        var loaded = await _store.GetSnapshotAsync(aggregateId);

        Assert.IsNotNull(loaded);
        Assert.AreEqual(aggregateId, loaded.AggregateId);
        Assert.AreEqual(5L, loaded.Version);
    }

    [TestMethod]
    public async Task SaveSnapshotAsync_Upsert_ReplacesExistingSnapshot()
    {
        var aggregateId = Guid.NewGuid();
        var tenantId = _tenantService.TenantId;

        var first = new Models.Snapshot(aggregateId, tenantId, "OrderAggregate", 3L, "{}", DateTimeOffset.UtcNow);
        await _store.SaveSnapshotAsync(first);

        var second = new Models.Snapshot(aggregateId, tenantId, "OrderAggregate", 7L, "{\"updated\":true}", DateTimeOffset.UtcNow);
        await _store.SaveSnapshotAsync(second);

        var loaded = await _store.GetSnapshotAsync(aggregateId);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(7L, loaded.Version);
    }

    [TestMethod]
    public async Task GetSnapshotAsync_ScopedToCurrentTenant()
    {
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();

        _tenantService.SetTenant(tenantAId);
        await _store.SaveSnapshotAsync(
            new Models.Snapshot(aggregateId, tenantAId, "OrderAggregate", 5L, "{}", DateTimeOffset.UtcNow));

        _tenantService.SetTenant(tenantBId);
        var snapshot = await _store.GetSnapshotAsync(aggregateId);

        Assert.IsNull(snapshot, "Snapshot from tenant A should not be visible to tenant B.");
    }
}
