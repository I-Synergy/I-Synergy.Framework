using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task StoredProcedure_Create_One()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);
            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca => onDropping++);
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(1, onCreating);
            Assert.AreEqual(1, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_changes").ConfigureAwait(false);
                Assert.IsTrue(check);
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task StoredProcedure_Exists()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);

            var exists = await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);
            Assert.IsTrue(exists);
            exists = await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChangesWithFilters);
            Assert.IsFalse(exists);

            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task StoredProcedure_Create_All()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);
            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca => onDropping++);
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(10, onCreating);
            Assert.AreEqual(10, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_bulkdelete").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_bulkupdate").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_changes").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_delete").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_deletemetadata").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_initialize").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_reset").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_selectrow").ConfigureAwait(false);
                Assert.IsTrue(check);
                check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_update").ConfigureAwait(false);
                Assert.IsTrue(check);
                c.Close();
            }


            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task StoredProcedure_Create_All_Overwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);
            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca => onDropping++);
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(10, onCreating);
            Assert.AreEqual(10, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            isCreated = await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsFalse(isCreated);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            isCreated = await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"], true);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(10, onCreating);
            Assert.AreEqual(10, onCreated);
            Assert.AreEqual(10, onDropping);
            Assert.AreEqual(10, onDropped);

            _databaseHelper.DropDatabase(dbName);
        }



        [Ignore]
        public async Task StoredProcedure_Drop_One()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var isCreated = await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);
            Assert.IsTrue(isCreated);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);
            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca => onDropping++);
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isDropped = await localOrchestrator.DropStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);

            Assert.IsTrue(isDropped);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(1, onDropping);
            Assert.AreEqual(1, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_changes").ConfigureAwait(false);
                Assert.IsFalse(check);
                c.Close();
            }

            // try to delete a non existing one
            isDropped = await localOrchestrator.DropStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChangesWithFilters);

            Assert.IsFalse(isDropped);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task StoredProcedure_Drop_One_Cancel()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var isCreated = await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);
            Assert.IsTrue(isCreated);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);

            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca =>
            {
                tca.Cancel = true;
                onDropping++;
            });
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isDropped = await localOrchestrator.DropStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);

            Assert.IsFalse(isDropped);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(1, onDropping);
            Assert.AreEqual(0, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.ProcedureExistsAsync(c, null, "SalesLT.Product_changes").ConfigureAwait(false);
                Assert.IsTrue(check);
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task StoredProcedure_Create_One_Overwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);
            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca => onDropping++);
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(1, onCreating);
            Assert.AreEqual(1, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            isCreated = await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges);

            Assert.IsFalse(isCreated);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            isCreated = await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, true);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(1, onCreating);
            Assert.AreEqual(1, onCreated);
            Assert.AreEqual(1, onDropping);
            Assert.AreEqual(1, onDropped);


            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task StoredProcedure_Drop_All()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnStoredProcedureCreating(tca => onCreating++);
            localOrchestrator.OnStoredProcedureCreated(tca => onCreated++);
            localOrchestrator.OnStoredProcedureDropping(tca => onDropping++);
            localOrchestrator.OnStoredProcedureDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(10, onCreating);
            Assert.AreEqual(10, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);


            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            var isDropped = await localOrchestrator.DropStoredProceduresAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(10, onDropping);
            Assert.AreEqual(10, onDropped);


            _databaseHelper.DropDatabase(dbName);
        }

    }
}
