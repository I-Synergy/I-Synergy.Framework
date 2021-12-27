using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task LocalOrchestrator_GetChanges()
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
            var agent = new SyncAgent(_versionService, clientProvider, serverProvider, Tables, scopeName);

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

            var onDatabaseSelecting = 0;
            var onDatabaseSelected = 0;
            var onSelecting = 0;
            var onSelected = 0;

            localOrchestrator.OnDatabaseChangesSelecting(dcs =>
            {
                onDatabaseSelecting++;
            });

            localOrchestrator.OnDatabaseChangesSelected(dcs =>
            {
                Assert.IsNotNull(dcs.BatchInfo);
                Assert.AreEqual(2, dcs.ChangesSelected.TableChangesSelected.Count);
                onDatabaseSelected++;
            });

            localOrchestrator.OnTableChangesSelecting(action =>
            {
                Assert.IsNotNull(action.Command);
                onSelecting++;
            });

            localOrchestrator.OnTableChangesSelected(action =>
            {
                Assert.IsNotNull(action.BatchPartInfos);
                onSelected++;
            });

            // Get changes to be populated to the server
            var changes = await localOrchestrator.GetChangesAsync();

            Assert.AreEqual(this.Tables.Length, onSelecting);
            Assert.AreEqual(this.Tables.Length, onSelected);
            Assert.AreEqual(1, onDatabaseSelected);
            Assert.AreEqual(1, onDatabaseSelecting);

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }

        [Ignore]
        public async Task LocalOrchestrator_GetBacthChanges()
        {
            var dbNameSrv = _databaseHelper.GetRandomName("tcp_lo_srv");
            await _databaseHelper.CreateDatabaseAsync(dbNameSrv, true);

            var dbNameCli = _databaseHelper.GetRandomName("tcp_lo_cli");
            await _databaseHelper.CreateDatabaseAsync(dbNameCli, true);

            var csServer = _databaseHelper.GetConnectionString(dbNameSrv);
            var serverProvider = new SqlSyncProvider(csServer);

            var csClient = _databaseHelper.GetConnectionString(dbNameCli);
            var clientProvider = new SqlSyncProvider(csClient);

            await new DataContext((dbNameSrv, serverProvider), true, true).Database.EnsureCreatedAsync();
            await new DataContext((dbNameCli, clientProvider), true, false).Database.EnsureCreatedAsync();

            var scopeName = "scopesnap1";
            var syncOptions = new SyncOptions
            {
                BatchSize = 100
            };

            var setup = new SyncSetup();

            // Make a first sync to be sure everything is in place
            var agent = new SyncAgent(_versionService, clientProvider, serverProvider, syncOptions, this.Tables, scopeName);

            // Making a first sync, will initialize everything we need
            var s = await agent.SynchronizeAsync();

            // Get the orchestrators
            var localOrchestrator = agent.LocalOrchestrator;
            var remoteOrchestrator = agent.RemoteOrchestrator;

            // Client side : Create a product category and a product
            // Create a productcategory item
            // Create a new product on server
            using (var ctx = new DataContext((dbNameCli, clientProvider)))
            {
                for (int i = 0; i <= 10000; i++)
                {
                    var productId = Guid.NewGuid();
                    var productName = _databaseHelper.GetRandomName();
                    var productNumber = productName.ToUpperInvariant().Substring(0, 10);

                    var productCategoryName = _databaseHelper.GetRandomName();
                    var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

                    if (!ctx.ProductCategory.Any(p => p.ProductCategoryId == productCategoryId))
                    {
                        var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                        ctx.Add(pc);
                    }

                    var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                    ctx.Add(product);

                }
                await ctx.SaveChangesAsync();

            }
            var onDatabaseSelecting = 0;
            var onDatabaseSelected = 0;
            var onSelecting = 0;
            var onSelected = 0;

            localOrchestrator.OnDatabaseChangesSelecting(dcs =>
            {
                onDatabaseSelecting++;
            });

            localOrchestrator.OnDatabaseChangesSelected(dcs =>
            {
                Assert.IsNotNull(dcs.BatchInfo);
                Assert.AreEqual(2, dcs.ChangesSelected.TableChangesSelected.Count);
                onDatabaseSelected++;
            });

            localOrchestrator.OnTableChangesSelecting(action =>
            {
                Assert.IsNotNull(action.Command);
                onSelecting++;
            });

            localOrchestrator.OnTableChangesSelected(action =>
            {
                Assert.IsNotNull(action.BatchPartInfos);
                onSelected++;
            });

            // Get changes to be populated to the server
            var changes = await localOrchestrator.GetChangesAsync();

            Assert.AreEqual(this.Tables.Length, onSelecting);
            Assert.AreEqual(16, onSelected);
            Assert.AreEqual(1, onDatabaseSelected);
            Assert.AreEqual(1, onDatabaseSelecting);

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }



        [Ignore]
        public async Task RemoteOrchestrator_GetChanges()
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

            remoteOrchestrator.OnTableChangesSelecting(action =>
            {
                Assert.IsNotNull(action.Command);
                onSelecting++;
            });

            remoteOrchestrator.OnTableChangesSelected(action =>
            {
                Assert.IsNotNull(action.BatchPartInfos);
                onSelected++;
            });
            remoteOrchestrator.OnDatabaseChangesSelecting(dcs =>
            {
                onDatabaseSelecting++;
            });

            remoteOrchestrator.OnDatabaseChangesSelected(dcs =>
            {
                Assert.IsNotNull(dcs.BatchInfo);
                Assert.AreEqual(2, dcs.ChangesSelected.TableChangesSelected.Count);
                onDatabaseSelected++;
            });

            var clientScope = await localOrchestrator.GetClientScopeAsync();

            // Get changes to be populated to be sent to the client
            var changes = await remoteOrchestrator.GetChangesAsync(clientScope);

            Assert.AreEqual(this.Tables.Length, onSelecting);
            Assert.AreEqual(this.Tables.Length, onSelected);
            Assert.AreEqual(1, onDatabaseSelected);
            Assert.AreEqual(1, onDatabaseSelecting);

            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }
    }
}
