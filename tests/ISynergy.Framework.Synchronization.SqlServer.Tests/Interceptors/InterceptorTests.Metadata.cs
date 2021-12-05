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
        public async Task LocalOrchestrator_MetadataCleaning()
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

            var cleaning = 0;
            var cleaned = 0;

            localOrchestrator.OnMetadataCleaning(args =>
            {
                cleaning++;
            });

            localOrchestrator.OnMetadataCleaned(args =>
            {
                cleaned++;
                Assert.AreEqual(0, args.DatabaseMetadatasCleaned.RowsCleanedCount);
                Assert.AreEqual(0, args.DatabaseMetadatasCleaned.Tables.Count);

            });

            // Making a first sync, will call cleaning, but nothing is cleaned
            var s2 = await agent.SynchronizeAsync();

            Assert.AreEqual(1, cleaning);
            Assert.AreEqual(1, cleaned);

            // Reset interceptors
            localOrchestrator.OnMetadataCleaning(null);
            localOrchestrator.OnMetadataCleaned(null);
            cleaning = 0;
            cleaned = 0;


            localOrchestrator.OnMetadataCleaning(args =>
            {
                cleaning++;
            });

            localOrchestrator.OnMetadataCleaned(args =>
            {
                cleaned++;
            });

            // Making a second empty sync.
            var s3 = await agent.SynchronizeAsync();

            // If there is no changes on any tables, no metadata cleaning is called
            Assert.AreEqual(0, cleaning);
            Assert.AreEqual(0, cleaned);


            // Server side : Create a product category 
            productCategoryName = _databaseHelper.GetRandomName();
            productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

            using (var ctx = new DataContext((dbNameSrv, serverProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                await ctx.SaveChangesAsync();
            }

            // Reset interceptors
            localOrchestrator.OnMetadataCleaning(null);
            localOrchestrator.OnMetadataCleaned(null);
            cleaning = 0;
            cleaned = 0;


            localOrchestrator.OnMetadataCleaning(args =>
            {
                cleaning++;
            });

            localOrchestrator.OnMetadataCleaned(args =>
            {
                cleaned++;
                Assert.AreEqual(1, args.DatabaseMetadatasCleaned.RowsCleanedCount);
                Assert.AreEqual(1, args.DatabaseMetadatasCleaned.Tables.Count);
                Assert.AreEqual("SalesLT", args.DatabaseMetadatasCleaned.Tables[0].SchemaName);
                Assert.AreEqual("ProductCategory", args.DatabaseMetadatasCleaned.Tables[0].TableName);
                Assert.AreEqual(1, args.DatabaseMetadatasCleaned.Tables[0].RowsCleanedCount);

            });
            var s4 = await agent.SynchronizeAsync();

            Assert.AreEqual(1, cleaning);
            Assert.AreEqual(1, cleaned);


            // Server side : Create a product category and a product
            // Create a productcategory item
            // Create a new product on server
            productId = Guid.NewGuid();
            productName = _databaseHelper.GetRandomName();
            productNumber = productName.ToUpperInvariant().Substring(0, 10);

            productCategoryName = _databaseHelper.GetRandomName();
            productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

            using (var ctx = new DataContext((dbNameSrv, serverProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);

                await ctx.SaveChangesAsync();
            }

            // Reset interceptors
            localOrchestrator.OnMetadataCleaning(null);
            localOrchestrator.OnMetadataCleaned(null);
            cleaning = 0;
            cleaned = 0;


            localOrchestrator.OnMetadataCleaning(args =>
            {
                cleaning++;
            });

            localOrchestrator.OnMetadataCleaned(args =>
            {
                cleaned++;
                Assert.AreEqual(0, args.DatabaseMetadatasCleaned.RowsCleanedCount);
            });

            var s5 = await agent.SynchronizeAsync();

            // cleaning is always called on N-1 rows, so nothing here should be called
            Assert.AreEqual(1, cleaning);
            Assert.AreEqual(1, cleaned);


            _databaseHelper.DropDatabase(dbNameSrv);
            _databaseHelper.DropDatabase(dbNameCli);
        }
    }
}
