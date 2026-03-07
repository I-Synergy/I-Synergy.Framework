using ISynergy.Framework.AspNetCore.MultiTenancy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Tests;

[TestClass]
public class TenantContextTests
{
    // Reset TenantContext state before each test so tests are independent.
    [TestInitialize]
    public void ResetContext() => TenantContext.Set(Guid.Empty, string.Empty);

    // ------------------------------------------------------------------ //
    // Default state
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void TenantId_WhenNothingSet_ReturnsGuidEmpty()
    {
        Assert.AreEqual(Guid.Empty, TenantContext.TenantId);
    }

    [TestMethod]
    public void UserName_WhenNothingSet_ReturnsEmptyString()
    {
        Assert.AreEqual(string.Empty, TenantContext.UserName);
    }

    // ------------------------------------------------------------------ //
    // Set()
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Set_WithTenantIdAndUserName_StoresBothValues()
    {
        var tenantId = Guid.NewGuid();
        const string userName = "alice";

        TenantContext.Set(tenantId, userName);

        Assert.AreEqual(tenantId, TenantContext.TenantId);
        Assert.AreEqual(userName, TenantContext.UserName);
    }

    [TestMethod]
    public void Set_WithTenantIdOnly_StoresEmptyUserName()
    {
        var tenantId = Guid.NewGuid();

        TenantContext.Set(tenantId);

        Assert.AreEqual(tenantId, TenantContext.TenantId);
        Assert.AreEqual(string.Empty, TenantContext.UserName);
    }

    [TestMethod]
    public void Set_CalledTwice_OverwritesPreviousValues()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();

        TenantContext.Set(first, "user1");
        TenantContext.Set(second, "user2");

        Assert.AreEqual(second, TenantContext.TenantId);
        Assert.AreEqual("user2", TenantContext.UserName);
    }

    // ------------------------------------------------------------------ //
    // Use()
    // ------------------------------------------------------------------ //

    [TestMethod]
    public void Use_AppliesValuesForScopeAndRestoresOnDispose()
    {
        var original = Guid.NewGuid();
        var scoped = Guid.NewGuid();
        TenantContext.Set(original, "original-user");

        using (TenantContext.Use(scoped, "scoped-user"))
        {
            Assert.AreEqual(scoped, TenantContext.TenantId);
            Assert.AreEqual("scoped-user", TenantContext.UserName);
        }

        Assert.AreEqual(original, TenantContext.TenantId);
        Assert.AreEqual("original-user", TenantContext.UserName);
    }

    [TestMethod]
    public void Use_WithDefaultUserName_SetsSystem()
    {
        var tenantId = Guid.NewGuid();

        using (TenantContext.Use(tenantId))
        {
            Assert.AreEqual("System", TenantContext.UserName);
        }
    }

    [TestMethod]
    public void Use_Nested_RestoresEachLayerCorrectly()
    {
        var outer = Guid.NewGuid();
        var inner = Guid.NewGuid();
        TenantContext.Set(Guid.Empty, string.Empty);

        using (TenantContext.Use(outer, "outer"))
        {
            Assert.AreEqual(outer, TenantContext.TenantId);

            using (TenantContext.Use(inner, "inner"))
            {
                Assert.AreEqual(inner, TenantContext.TenantId);
                Assert.AreEqual("inner", TenantContext.UserName);
            }

            Assert.AreEqual(outer, TenantContext.TenantId);
            Assert.AreEqual("outer", TenantContext.UserName);
        }

        Assert.AreEqual(Guid.Empty, TenantContext.TenantId);
        Assert.AreEqual(string.Empty, TenantContext.UserName);
    }

    [TestMethod]
    public void Use_ReturnsDisposable()
    {
        var tenantId = Guid.NewGuid();
        var scope = TenantContext.Use(tenantId, "user");

        Assert.IsNotNull(scope);
        Assert.IsInstanceOfType<IDisposable>(scope);

        scope.Dispose();
    }

    // ------------------------------------------------------------------ //
    // AsyncLocal behaviour
    // ------------------------------------------------------------------ //

    [TestMethod]
    public async Task TenantContext_FlowsCorrectlyAcrossAsyncContinuations()
    {
        var tenantId = Guid.NewGuid();
        TenantContext.Set(tenantId, "async-user");

        await Task.Yield();

        Assert.AreEqual(tenantId, TenantContext.TenantId);
        Assert.AreEqual("async-user", TenantContext.UserName);
    }

    [TestMethod]
    public async Task TenantContext_ValuesDoNotLeakBetweenParallelTasks()
    {
        // Each task sets its own tenant; they must not overwrite each other's reads.
        var results = new Guid[5];
        var barrier = new SemaphoreSlim(0, 5);

        var tasks = Enumerable.Range(0, 5).Select(async i =>
        {
            var id = Guid.NewGuid();
            TenantContext.Set(id);

            // Allow all tasks to set their values before any reads.
            barrier.Release();
            await barrier.WaitAsync();

            results[i] = TenantContext.TenantId;
        }).ToArray();

        await Task.WhenAll(tasks);

        // Every task should have read back its own tenant, not a neighbour's.
        // We verify uniqueness: if AsyncLocal works correctly all 5 values differ.
        var distinct = results.Distinct().Count();
        Assert.AreEqual(5, distinct, "AsyncLocal values leaked between parallel tasks.");
    }

    [TestMethod]
    public async Task Use_RestoredCorrectlyAfterAwaitInsideScope()
    {
        var outer = Guid.NewGuid();
        var scoped = Guid.NewGuid();
        TenantContext.Set(outer, "outer");

        using (TenantContext.Use(scoped, "scoped"))
        {
            await Task.Delay(1);
            Assert.AreEqual(scoped, TenantContext.TenantId);
        }

        Assert.AreEqual(outer, TenantContext.TenantId);
    }

    [TestMethod]
    public async Task TenantContext_SetInParentTask_VisibleInChildTask()
    {
        var tenantId = Guid.NewGuid();
        TenantContext.Set(tenantId, "parent-user");

        var captured = await Task.Run(() => TenantContext.TenantId);

        Assert.AreEqual(tenantId, captured);
    }
}
