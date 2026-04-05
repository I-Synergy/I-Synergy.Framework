using ISynergy.Framework.EventSourcing.Abstractions.Aggregates;
using ISynergy.Framework.EventSourcing.EntityFramework.Services;
using ISynergy.Framework.EventSourcing.EntityFramework.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.EventSourcing.EntityFramework.Services;

[TestClass]
public class AggregateRepositoryTests
{
    private TestTenantService _tenantService = null!;
    private EventSourcingDbContext _context = null!;
    private EventStore _store = null!;
    private DefaultEventTypeResolver _resolver = null!;
    private JsonReflectionEventSerializer _serializer = null!;
    private IAggregateRepository<OrderAggregate, Guid> _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _tenantService = new TestTenantService();
        _context = EventSourcingDbContextFactory.Create(_tenantService);
        _resolver = new DefaultEventTypeResolver();
        _serializer = new JsonReflectionEventSerializer();
        _store = new EventStore(_context, _tenantService, _resolver, _serializer);
        _repository = new AggregateRepository<OrderAggregate, Guid>(_store, _tenantService, _resolver, _serializer);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    // ── LoadAsync ─────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task LoadAsync_ReturnsNull_WhenNoEventsExist()
    {
        var result = await _repository.LoadAsync(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task LoadAsync_ReturnsHydratedAggregate_AfterSave()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderAggregate();
        order.Place(orderId, "Alice", 150m);

        await _repository.SaveAsync(order);

        var loaded = await _repository.LoadAsync(orderId);

        Assert.IsNotNull(loaded);
        Assert.AreEqual(orderId, loaded.Id);
        Assert.AreEqual("Alice", loaded.CustomerName);
        Assert.AreEqual(150m, loaded.Total);
        Assert.IsFalse(loaded.IsCancelled);
    }

    [TestMethod]
    public async Task LoadAsync_ReappliesMultipleEvents_InOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderAggregate();
        order.Place(orderId, "Bob", 200m);
        order.Ship("TRACK-XYZ");

        await _repository.SaveAsync(order);

        var loaded = await _repository.LoadAsync(orderId);

        Assert.IsNotNull(loaded);
        Assert.AreEqual("TRACK-XYZ", loaded.TrackingNumber);
    }

    [TestMethod]
    public async Task LoadAsync_ReflectsLatestState_AfterMultipleSaves()
    {
        var orderId = Guid.NewGuid();

        // First save: place
        var order = new OrderAggregate();
        order.Place(orderId, "Carol", 75m);
        await _repository.SaveAsync(order);

        // Second save: ship (reload then apply)
        var reloaded = await _repository.LoadAsync(orderId);
        reloaded!.Ship("TRACK-001");
        await _repository.SaveAsync(reloaded);

        // Third load: should see both events applied
        var final = await _repository.LoadAsync(orderId);

        Assert.IsNotNull(final);
        Assert.AreEqual("Carol", final.CustomerName);
        Assert.AreEqual("TRACK-001", final.TrackingNumber);
    }

    // ── SaveAsync ─────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task SaveAsync_DoesNothing_WhenNoUncommittedEvents()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderAggregate();
        order.Place(orderId, "Dave", 300m);
        await _repository.SaveAsync(order);

        // No new events raised
        await _repository.SaveAsync(order);

        var records = await _store.GetEventsForAggregateAsync("OrderAggregate", orderId);
        Assert.AreEqual(1, records.Count, "No additional events should have been appended.");
    }

    [TestMethod]
    public async Task SaveAsync_ClearsUncommittedEvents_AfterPersisting()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderAggregate();
        order.Place(orderId, "Eve", 500m);

        await _repository.SaveAsync(order);

        Assert.AreEqual(0, order.GetUncommittedEvents().Count);
    }

    [TestMethod]
    public async Task SaveAsync_SetsCorrectVersion_AfterPersisting()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderAggregate();
        order.Place(orderId, "Frank", 25m);
        order.Ship("T99");

        await _repository.SaveAsync(order);

        // Version should equal the number of events replayed via LoadFromHistory, but at
        // the time of saving the aggregate was freshly created (not loaded), so Version = 0.
        // After saving, the DB has 2 events. Reload to verify version is set by LoadFromHistory.
        var loaded = await _repository.LoadAsync(orderId);
        Assert.AreEqual(2L, loaded!.Version);
    }

    // ── Concurrency ───────────────────────────────────────────────────────────

    [TestMethod]
    public async Task SaveAsync_ThrowsOnConcurrencyConflict_WhenSameVersionSavedTwice()
    {
        var orderId = Guid.NewGuid();

        // Two instances both start from the same loaded state (Version = 1 after load).
        var v1 = new OrderAggregate();
        v1.Place(orderId, "Grace", 80m);
        await _repository.SaveAsync(v1);

        var instanceA = await _repository.LoadAsync(orderId);
        var instanceB = await _repository.LoadAsync(orderId);

        instanceA!.Ship("TRACK-A");
        instanceB!.Ship("TRACK-B");

        // First save succeeds
        await _repository.SaveAsync(instanceA);

        // Second save must fail — both have Version = 1 but DB is now at Version = 2
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.SaveAsync(instanceB));
    }

    // ── Snapshot hydration ────────────────────────────────────────────────────

    [TestMethod]
    public async Task LoadAsync_WithSnapshotOnly_ReturnsAggregateAtSnapshotVersion()
    {
        // Arrange: save a snapshot directly (no hot events — simulates a fully archived aggregate).
        var aggregateId = Guid.NewGuid();
        var tenantId = _tenantService.TenantId;

        await _store.SaveSnapshotAsync(new Models.Snapshot(
            aggregateId, tenantId, "OrderAggregate", 5L,
            "{}", DateTimeOffset.UtcNow));

        // Act
        var loaded = await _repository.LoadAsync(aggregateId);

        // Assert: aggregate returned (not null) with version set from snapshot.
        Assert.IsNotNull(loaded);
        Assert.AreEqual(5L, loaded.Version);
    }

    [TestMethod]
    public async Task LoadAsync_WithSnapshotAndDeltaEvents_ReplaysDeltaOnTopOfSnapshot()
    {
        // Arrange: snapshot at version 1 + one additional (delta) event at version 2.
        var aggregateId = Guid.NewGuid();
        var tenantId = _tenantService.TenantId;
        const long snapshotVersion = 1L;

        // Save snapshot at version 1 (simulating archive boundary).
        await _store.SaveSnapshotAsync(new Models.Snapshot(
            aggregateId, tenantId, "OrderAggregate", snapshotVersion,
            "{}", DateTimeOffset.UtcNow));

        // Append a delta event with expectedVersion = snapshotVersion (hot-tier only).
        await _store.AppendEventAsync(
            tenantId, "OrderAggregate", aggregateId, expectedVersion: snapshotVersion,
            new OrderShipped(aggregateId, "TRACK-DELTA"));

        // Act
        var loaded = await _repository.LoadAsync(aggregateId);

        // Assert: version is snapshot version + 1 delta event.
        Assert.IsNotNull(loaded);
        Assert.AreEqual(snapshotVersion + 1, loaded.Version);
        Assert.AreEqual("TRACK-DELTA", loaded.TrackingNumber);
    }
}
