using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task LocalOrchestrator_Scope()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);


            var scopeTableCreating = 0;
            var scopeTableCreated = 0;
            var scopeLoading = 0;
            var scopeLoaded = 0;
            var scopeSaving = 0;
            var scopeSaved = 0;

            localOrchestrator.OnScopeSaving(ssa =>
            {
                Assert.IsNotNull(ssa.Command);
                scopeSaving++;
            });

            localOrchestrator.OnScopeSaved(ssa => scopeSaved++);

            localOrchestrator.OnScopeTableCreating(stca =>
            {
                Assert.IsNotNull(stca.Command);
                scopeTableCreating++;
            });

            localOrchestrator.OnScopeTableCreated(stca =>
            {
                scopeTableCreated++;
            });

            localOrchestrator.OnScopeLoading(args =>
            {
                Assert.IsNotNull(args.Command);
                Assert.AreEqual(SyncStage.ScopeLoading, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.AreEqual(scopeName, args.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                Assert.AreSame(args.Connection, args.Transaction.Connection);
                scopeLoading++;
            });

            localOrchestrator.OnScopeLoaded(args =>
            {
                Assert.AreEqual(SyncStage.ScopeLoading, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.ScopeInfo);
                Assert.AreEqual(scopeName, args.ScopeInfo.Name);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                scopeLoaded++;
            });

            var localScopeInfo = await localOrchestrator.GetClientScopeAsync();


            Assert.AreEqual(1, scopeTableCreating);
            Assert.AreEqual(1, scopeTableCreated);
            Assert.AreEqual(1, scopeLoading);
            Assert.AreEqual(1, scopeLoaded);
            Assert.AreEqual(1, scopeSaving);
            Assert.AreEqual(1, scopeSaved);

            scopeTableCreating = 0;
            scopeTableCreated = 0;
            scopeLoading = 0;
            scopeLoaded = 0;
            scopeSaving = 0;
            scopeSaved = 0;

            localScopeInfo.Version = "2.0";

            await localOrchestrator.SaveClientScopeAsync(localScopeInfo);

            Assert.AreEqual(0, scopeTableCreating);
            Assert.AreEqual(0, scopeTableCreated);
            Assert.AreEqual(0, scopeLoading);
            Assert.AreEqual(0, scopeLoaded);
            Assert.AreEqual(1, scopeSaving);
            Assert.AreEqual(1, scopeSaved);

            _databaseHelper.DropDatabase(dbName);
        }
        [Ignore]
        public async Task RemoteOrchestrator_Scope()
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

            var remoteOrchestrator = new RemoteOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var scopeTableCreating = 0;
            var scopeTableCreated = 0;
            var scopeLoading = 0;
            var scopeLoaded = 0;
            var scopeSaving = 0;
            var scopeSaved = 0;

            remoteOrchestrator.OnScopeSaving(ssa =>
            {
                Assert.IsNotNull(ssa.Command);
                scopeSaving++;
            });

            remoteOrchestrator.OnScopeSaved(ssa => scopeSaved++);

            remoteOrchestrator.OnScopeTableCreating(stca =>
            {
                Assert.IsNotNull(stca.Command);
                scopeTableCreating++;
            });

            remoteOrchestrator.OnScopeTableCreated(stca =>
            {
                scopeTableCreated++;
            });

            remoteOrchestrator.OnServerScopeLoading(args =>
            {
                Assert.IsNotNull(args.Command);
                Assert.AreEqual(SyncStage.ScopeLoading, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.AreEqual(scopeName, args.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                Assert.AreSame(args.Connection, args.Transaction.Connection);
                scopeLoading++;
            });

            remoteOrchestrator.OnServerScopeLoaded(args =>
            {
                Assert.AreEqual(SyncStage.ScopeLoading, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.ScopeInfo);
                Assert.AreEqual(scopeName, args.ScopeInfo.Name);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                scopeLoaded++;
            });

            var serverScopeInfo = await remoteOrchestrator.GetServerScopeAsync();

            serverScopeInfo.Version = "2.0";

            await remoteOrchestrator.SaveServerScopeAsync(serverScopeInfo);

            Assert.AreEqual(2, scopeTableCreating);
            Assert.AreEqual(2, scopeTableCreated);
            Assert.AreEqual(1, scopeLoading);
            Assert.AreEqual(1, scopeLoaded);
            Assert.AreEqual(3, scopeSaving);
            Assert.AreEqual(3, scopeSaved);

            _databaseHelper.DropDatabase(dbName);
        }
    }
}
