using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class LocalOrchestratorTests
    {
        /// <summary>
        /// RemoteOrchestrator.GetChanges() should return rows inserted on server, depending on the client scope sent
        /// </summary>
        [TestMethod]
        public async Task LocalOrchestrator_GetChanges_ShouldReturnNewRowsInserted()
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
            await agent.SynchronizeAsync();

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

            using (var ctx = new DataContext((dbNameCli, clientProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);

                await ctx.SaveChangesAsync();
            }

            // Get changes to be populated to the server
            var changes = await localOrchestrator.GetChangesAsync();

            Assert.IsNotNull(changes.ClientBatchInfo);
            Assert.IsNotNull(changes.ClientChangesSelected);
            Assert.AreEqual(2, changes.ClientChangesSelected.TableChangesSelected.Count);
            Assert.IsTrue(changes.ClientChangesSelected.TableChangesSelected.Select(tcs => tcs.TableName).ToList().Contains("Product"));
            Assert.IsTrue(changes.ClientChangesSelected.TableChangesSelected.Select(tcs => tcs.TableName).ToList().Contains("ProductCategory"));

            var productTable = changes.ClientBatchInfo.InMemoryData.Tables["Product", "SalesLT"];
            var productRowName = productTable.Rows[0]["Name"];

            Assert.AreEqual(productName, productRowName);

            var productCategoryTable = changes.ClientBatchInfo.InMemoryData.Tables["ProductCategory", "SalesLT"];
            var productCategoryRowName = productCategoryTable.Rows[0]["Name"];

            Assert.AreEqual(productCategoryName, productCategoryRowName);

        }

    }
}
