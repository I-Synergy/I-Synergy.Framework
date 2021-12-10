using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task TrackingTable_Create_One()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });
            setup.TrackingTablesPrefix = "t_";
            setup.TrackingTablesSuffix = "_t";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = false;
            var onCreated = false;

            localOrchestrator.OnTrackingTableCreating(ttca =>
            {
                var addingID = $" ALTER TABLE {ttca.TrackingTableName.Schema().Quoted()} ADD internal_id int identity(1,1)";
                ttca.Command.CommandText += addingID;
                onCreating = true;
            });

            localOrchestrator.OnTrackingTableCreated(ttca =>
            {
                onCreated = true;
            });

            await localOrchestrator.CreateTrackingTableAsync(setup.Tables[0]);

            Assert.IsTrue(onCreating);
            Assert.IsTrue(onCreated);


            // Check we have a new column in tracking table
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var cols = await SqlManagementUtils.GetColumnsForTableAsync(c, null, "t_Product_t", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(7, cols.Rows.Count);
                Assert.IsNotNull(cols.Rows.FirstOrDefault(r => r["name"].ToString() == "internal_id"));
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task TrackingTable_Exists()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product", "SalesLT.ProductCategory" });
            setup.TrackingTablesPrefix = "t_";
            setup.TrackingTablesSuffix = "_t";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            await localOrchestrator.CreateTrackingTableAsync(setup.Tables[0]);

            var exists = await localOrchestrator.ExistTrackingTableAsync(setup.Tables[0]);
            Assert.IsTrue(exists);
            exists = await localOrchestrator.ExistTrackingTableAsync(setup.Tables[1]);
            Assert.IsFalse(exists);

            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task TrackingTable_Create_All()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.ProductCategory", "SalesLT.ProductModel", "SalesLT.Product", "Posts" });
            setup.TrackingTablesPrefix = "t_";
            setup.TrackingTablesSuffix = "_t";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTrackingTableCreating(ttca => onCreating++);
            localOrchestrator.OnTrackingTableCreated(ttca => onCreated++);
            localOrchestrator.OnTrackingTableDropping(ttca => onDropping++);
            localOrchestrator.OnTrackingTableDropped(ttca => onDropped++);

            await localOrchestrator.CreateTrackingTablesAsync();

            Assert.AreEqual(4, onCreating);
            Assert.AreEqual(4, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            await localOrchestrator.CreateTrackingTablesAsync();

            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            await localOrchestrator.CreateTrackingTablesAsync(true);

            Assert.AreEqual(4, onCreating);
            Assert.AreEqual(4, onCreated);
            Assert.AreEqual(4, onDropping);
            Assert.AreEqual(4, onDropped);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task TrackingTable_Drop_One()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });
            setup.TrackingTablesPrefix = "t_";
            setup.TrackingTablesSuffix = "_t";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onDropping = false;
            var onDropped = false;

            localOrchestrator.OnTrackingTableDropping(ttca =>
            {
                onDropping = true;
            });

            localOrchestrator.OnTrackingTableDropped(ttca =>
            {
                onDropped = true;
            });

            await localOrchestrator.CreateTrackingTableAsync(setup.Tables[0]);
            await localOrchestrator.DropTrackingTableAsync(setup.Tables[0]);

            Assert.IsTrue(onDropping);
            Assert.IsTrue(onDropped);


            // Check we have a new column in tracking table
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var table = await SqlManagementUtils.GetTableAsync(c, null, "t_Product_t", "SalesLT").ConfigureAwait(false);

                Assert.AreEqual(0, table.Rows.Count);

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task TrackingTable_Drop_One_Cancel()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });
            setup.TrackingTablesPrefix = "t_";
            setup.TrackingTablesSuffix = "_t";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onDropping = false;
            var onDropped = false;

            localOrchestrator.OnTrackingTableDropping(ttca =>
            {
                ttca.Cancel = true;
                onDropping = true;
            });

            localOrchestrator.OnTrackingTableDropped(ttca =>
            {
                onDropped = true;
            });

            await localOrchestrator.CreateTrackingTableAsync(setup.Tables[0]);
            await localOrchestrator.DropTrackingTableAsync(setup.Tables[0]);

            Assert.IsTrue(onDropping);
            Assert.IsFalse(onDropped);

            // Check we have a new column in tracking table
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var table = await SqlManagementUtils.GetTableAsync(c, null, "t_Product_t", "SalesLT").ConfigureAwait(false);

                Assert.IsTrue(table.Rows.Count > 0);

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task TrackingTable_Drop_All()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.ProductCategory", "SalesLT.ProductModel", "SalesLT.Product", "Posts" });
            setup.TrackingTablesPrefix = "t_";
            setup.TrackingTablesSuffix = "_t";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTrackingTableDropping(ttca => onDropping++);
            localOrchestrator.OnTrackingTableDropped(ttca => onDropped++);

            await localOrchestrator.CreateTrackingTablesAsync();
            await localOrchestrator.DropTrackingTablesAsync();


            Assert.AreEqual(4, onDropping);
            Assert.AreEqual(4, onDropped);

            _databaseHelper.DropDatabase(dbName);
        }

    }
}
