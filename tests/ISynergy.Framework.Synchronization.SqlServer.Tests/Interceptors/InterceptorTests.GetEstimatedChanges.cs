using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task LocalOrchestrator_GetEstimatedChanges()
        {
            var dbNameSrv = _databaseHelper.GetRandomName("tcp_lo_srv");
            await _databaseHelper.CreateDatabaseAsync(dbNameSrv, true);

            var dbNameCli = _databaseHelper.GetRandomName("tcp_lo_cli");
            await _databaseHelper.CreateDatabaseAsync(dbNameCli, true);

            var csServer = _databaseHelper.GetConnectionString(dbNameSrv);
            var serverProvider = new SqlSyncProvider(csServer);

            var csClient = _databaseHelper.GetConnectionString(dbNameCli);
            var clientProvider = new SqlSyncProvider(csClient);

            await new DataContext((dbNameSrv, serverProvider), true, false).Database.EnsureCreatedAsync();
            await new DataContext((dbNameCli, clientProvider), true, false).Database.EnsureCreatedAsync();

            var scopeName = "scopesnap1";
            var syncOptions = new SyncOptions();
            var setup = new SyncSetup();

            // Make a first sync to be sure everything is in place
            var agent = new SyncAgent(clientProvider, serverProvider, this.Tables, scopeName);

            // Making a first sync, will initialize everything we need
            var s = await agent.SynchronizeAsync();

            // Get the orchestrators
            var localOrchestrator = agent.LocalOrchestrator;
            var remoteOrchestrator = agent.RemoteOrchestrator;

            // Client side : Create a product category and a product
            // Create a productcategory item
            // Create a new product on server
            var productId = Guid.NewGuid();
            var productName = _databaseHelper.GetRandomName();
            var productNumber = productName.ToUpperInvariant().Substring(0, 10);

            var productCategoryName = _databaseHelper.GetRandomName();
            var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

            using (var ctx = new DataContext((dbNameCli, clientProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);

                await ctx.SaveChangesAsync();
            }

            var onSelecting = 0;
            var onSelected = 0;
            var onDatabaseSelecting = 0;
            var onDatabaseSelected = 0;

            localOrchestrator.OnTableChangesSelecting(action =>
            {
                Assert.IsNotNull(action.Command);
                onSelecting++;
            });

            localOrchestrator.OnTableChangesSelected(action =>
            {
                Assert.IsNull(action.Changes);
                onSelected++;
            });
            localOrchestrator.OnDatabaseChangesSelecting(dcs =>
            {
                onDatabaseSelecting++;
            });

            localOrchestrator.OnDatabaseChangesSelected(dcs =>
            {
                Assert.IsNull(dcs.BatchInfo);
                Assert.AreEqual(2, dcs.ChangesSelected.TableChangesSelected.Count);
                onDatabaseSelected++;
            });

            // Get changes to be populated to the server
            var changes = await localOrchestrator.GetEstimatedChangesCountAsync();

            Assert.AreEqual(this.Tables.Length, onSelecting);
            Assert.AreEqual(this.Tables.Length, onSelected);
            Assert.AreEqual(1, onDatabaseSelected);
            Assert.AreEqual(1, onDatabaseSelecting);

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }

        [Ignore]
        public async Task RemoteOrchestrator_GetEstimatedChanges()
        {
            var dbNameSrv = _databaseHelper.GetRandomName("tcp_lo_srv");
            await _databaseHelper.CreateDatabaseAsync(dbNameSrv, true);

            var dbNameCli = _databaseHelper.GetRandomName("tcp_lo_cli");
            await _databaseHelper.CreateDatabaseAsync(dbNameCli, true);

            var csServer = _databaseHelper.GetConnectionString(dbNameSrv);
            var serverProvider = new SqlSyncProvider(csServer);

            var csClient = _databaseHelper.GetConnectionString(dbNameCli);
            var clientProvider = new SqlSyncProvider(csClient);

            await new DataContext((dbNameSrv, serverProvider), true, false).Database.EnsureCreatedAsync();
            await new DataContext((dbNameCli, clientProvider), true, false).Database.EnsureCreatedAsync();

            var scopeName = "scopesnap1";
            var syncOptions = new SyncOptions();
            var setup = new SyncSetup();

            // Make a first sync to be sure everything is in place
            var agent = new SyncAgent(clientProvider, serverProvider, this.Tables, scopeName);

            // Making a first sync, will initialize everything we need
            var s = await agent.SynchronizeAsync();

            // Get the orchestrators
            var localOrchestrator = agent.LocalOrchestrator;
            var remoteOrchestrator = agent.RemoteOrchestrator;

            // Server side : Create a product category and a product
            // Create a productcategory item
            // Create a new product on server
            var productId = Guid.NewGuid();
            var productName = _databaseHelper.GetRandomName();
            var productNumber = productName.ToUpperInvariant().Substring(0, 10);

            var productCategoryName = _databaseHelper.GetRandomName();
            var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

            using (var ctx = new DataContext((dbNameSrv, serverProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);

                await ctx.SaveChangesAsync();
            }

            var onSelecting = 0;
            var onSelected = 0;
            var onDatabaseSelecting = 0;
            var onDatabaseSelected = 0;

            remoteOrchestrator.OnDatabaseChangesSelecting(dcs =>
            {
                onDatabaseSelecting++;
            });

            remoteOrchestrator.OnDatabaseChangesSelected(dcs =>
            {
                Assert.IsNull(dcs.BatchInfo);
                Assert.AreEqual(2, dcs.ChangesSelected.TableChangesSelected.Count);
                onDatabaseSelected++;
            });

            remoteOrchestrator.OnTableChangesSelecting(action =>
            {
                Assert.IsNotNull(action.Command);
                onSelecting++;
            });

            remoteOrchestrator.OnTableChangesSelected(action =>
            {
                Assert.IsNull(action.Changes);
                onSelected++;
            });

            var clientScope = await localOrchestrator.GetClientScopeAsync();

            // Get changes to be populated to be sent to the client
            var changes = await remoteOrchestrator.GetEstimatedChangesCountAsync(clientScope);

            Assert.AreEqual(this.Tables.Length, onSelecting);
            Assert.AreEqual(this.Tables.Length, onSelected);
            Assert.AreEqual(1, onDatabaseSelected);
            Assert.AreEqual(1, onDatabaseSelecting);

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }

    }
}
