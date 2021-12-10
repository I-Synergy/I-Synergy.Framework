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
        [TestMethod()]
        public async Task LocalOrchestrator_ApplyChanges()
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
            var agent = new SyncAgent(_versionService, clientProvider, serverProvider, this.Tables, scopeName);

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

            using (var ctx = new DataContext((dbNameSrv, serverProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);

                await ctx.SaveChangesAsync();
            }

            var onDatabaseApplying = 0;
            var onDatabaseApplied = 0;
            var onBatchApplying = 0;
            var onBatchApplied = 0;
            var onApplying = 0;
            var onApplied = 0;

            localOrchestrator.OnDatabaseChangesApplying(dcs =>
            {
                onDatabaseApplying++;
            });

            localOrchestrator.OnDatabaseChangesApplied(dcs =>
            {
                Assert.IsNotNull(dcs.ChangesApplied);
                Assert.AreEqual(2, dcs.ChangesApplied.TableChangesApplied.Count);
                onDatabaseApplied++;
            });

            localOrchestrator.OnTableChangesBatchApplying(action =>
            {
                Assert.IsNotNull(action.Changes);
                Assert.IsNotNull(action.Command);
                onBatchApplying++;
            });

            localOrchestrator.OnTableChangesBatchApplied(action =>
            {
                Assert.AreEqual(1, action.TableChangesApplied.Applied);
                onBatchApplied++;
            });

            localOrchestrator.OnTableChangesApplying(action =>
            {
                Assert.IsNotNull(action.Table);
                onApplying++;
            });

            localOrchestrator.OnTableChangesApplied(action =>
            {
                Assert.AreEqual(1, action.TableChangesApplied.Applied);
                onApplied++;
            });

            // Making a first sync, will initialize everything we need
            var s2 = await agent.SynchronizeAsync();

            Assert.AreEqual(2, onBatchApplying);
            Assert.AreEqual(2, onBatchApplied);
            Assert.AreEqual(1, onDatabaseApplying);
            Assert.AreEqual(1, onDatabaseApplied);
            Assert.AreEqual(4, onApplying); // Deletes + Modified state = Table count * 2
            Assert.AreEqual(2, onApplied); // Two tables applied

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }

        [Ignore]
        public async Task RemoteOrchestrator_ApplyChanges()
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
            var agent = new SyncAgent(_versionService, clientProvider, serverProvider, this.Tables, scopeName);

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

            var onDatabaseApplying = 0;
            var onDatabaseApplied = 0;
            var onApplying = 0;
            var onApplied = 0;

            remoteOrchestrator.OnDatabaseChangesApplying(dcs =>
            {
                onDatabaseApplying++;
            });

            remoteOrchestrator.OnDatabaseChangesApplied(dcs =>
            {
                Assert.IsNotNull(dcs.ChangesApplied);
                Assert.AreEqual(2, dcs.ChangesApplied.TableChangesApplied.Count);
                onDatabaseApplied++;
            });

            remoteOrchestrator.OnTableChangesBatchApplying(action =>
            {
                Assert.IsNotNull(action.Changes);
                Assert.IsNotNull(action.Command);
                onApplying++;
            });

            remoteOrchestrator.OnTableChangesBatchApplied(action =>
            {
                Assert.AreEqual(1, action.TableChangesApplied.Applied);
                onApplied++;
            });

            // Making a first sync, will initialize everything we need
            var s2 = await agent.SynchronizeAsync();

            Assert.AreEqual(2, onApplying);
            Assert.AreEqual(2, onApplied);
            Assert.AreEqual(1, onDatabaseApplying);
            Assert.AreEqual(1, onDatabaseApplied);

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }
    }
}