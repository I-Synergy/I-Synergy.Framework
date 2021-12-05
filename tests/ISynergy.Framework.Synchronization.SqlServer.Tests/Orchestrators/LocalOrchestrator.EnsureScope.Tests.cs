using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class LocalOrchestratorTests
    {
        [Ignore]
        public async Task LocalOrchestrator_EnsureScope_ShouldNot_Fail_If_NoTables_In_Setup()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);

            var options = new SyncOptions();
            var setup = new SyncSetup();
            var provider = new SqlSyncProvider(cs);

            var localOrchestrator = new LocalOrchestrator(provider, options, setup);

            var scope = await localOrchestrator.GetClientScopeAsync();

            Assert.IsNotNull(scope);

            _databaseHelper.DropDatabase(dbName);

        }

        [Ignore]
        public async Task LocalOrchestrator_EnsureScope_NewScope()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();
            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(this.Tables);

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            var localScopeInfo = await localOrchestrator.GetClientScopeAsync();

            Assert.IsNotNull(localScopeInfo);
            Assert.AreEqual(scopeName, localScopeInfo.Name);
            Assert.IsTrue(localScopeInfo.IsNewScope);
            Assert.AreNotEqual(Guid.Empty, localScopeInfo.Id);
            Assert.IsNull(localScopeInfo.LastServerSyncTimestamp);
            Assert.IsNull(localScopeInfo.LastSync);
            Assert.AreEqual(0, localScopeInfo.LastSyncDuration);
            Assert.IsNull(localScopeInfo.LastSyncTimestamp);
            Assert.IsNull(localScopeInfo.Schema);
            Assert.AreEqual(SyncVersion.Current, new Version(localScopeInfo.Version));

            // Check context
            SyncContext syncContext = localOrchestrator.GetContext();
            Assert.AreEqual(scopeName, syncContext.ScopeName);
            Assert.AreNotEqual(Guid.Empty, syncContext.SessionId);
            Assert.IsNull(syncContext.Parameters);
            Assert.AreEqual(SyncStage.ScopeLoading, syncContext.SyncStage);
            Assert.AreEqual(SyncType.Normal, syncContext.SyncType);
            Assert.AreEqual(SyncWay.None, syncContext.SyncWay);


            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task LocalOrchestrator_CancellationToken_ShouldInterrupt_EnsureScope_OnConnectionOpened()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(this.Tables);

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);
            using var cts = new CancellationTokenSource();

            localOrchestrator.OnConnectionOpen(args => cts.Cancel());

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await localOrchestrator.GetClientScopeAsync(default, default, cts.Token));

            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("OperationCanceledException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task LocalOrchestrator_CancellationToken_ShouldInterrupt_EnsureScope_OnTransactionCommit()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(this.Tables);

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);
            using var cts = new CancellationTokenSource();

            localOrchestrator.OnTransactionCommit(args => cts.Cancel());
            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await localOrchestrator.GetClientScopeAsync(default, default, cts.Token));
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("OperationCanceledException", se.TypeName);
            
            _databaseHelper.DropDatabase(dbName);
        }
    }
}
