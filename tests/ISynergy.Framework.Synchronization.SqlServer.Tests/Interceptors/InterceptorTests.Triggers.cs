using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task Trigger_Create_One()
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

            options.Logger = LoggerFactory.Create(builder => { builder.SetMinimumLevel(LogLevel.Debug); }).CreateLogger(nameof(InterceptorsTests)); ;

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTriggerCreating(tca => onCreating++);
            localOrchestrator.OnTriggerCreated(tca => onCreated++);
            localOrchestrator.OnTriggerDropping(tca => onDropping++);
            localOrchestrator.OnTriggerDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(1, onCreating);
            Assert.AreEqual(1, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.GetTriggerAsync(c, null, "Product_insert_trigger", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(1, check.Rows.Count);
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Trigger_Exists()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);

            var exists = await localOrchestrator.ExistTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);
            Assert.IsTrue(exists);
            exists = await localOrchestrator.ExistTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Update);
            Assert.IsFalse(exists);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Trigger_Create_All()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTriggerCreating(tca => onCreating++);
            localOrchestrator.OnTriggerCreated(tca => onCreated++);
            localOrchestrator.OnTriggerDropping(tca => onDropping++);
            localOrchestrator.OnTriggerDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateTriggersAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(3, onCreating);
            Assert.AreEqual(3, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.GetTriggerAsync(c, null, "Product_insert_trigger", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(1, check.Rows.Count);
                check = await SqlManagementUtils.GetTriggerAsync(c, null, "Product_update_trigger", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(1, check.Rows.Count);
                check = await SqlManagementUtils.GetTriggerAsync(c, null, "Product_delete_trigger", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(1, check.Rows.Count);
                c.Close();
            }


            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Trigger_Drop_One()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);
            Assert.IsTrue(isCreated);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTriggerCreating(tca => onCreating++);
            localOrchestrator.OnTriggerCreated(tca => onCreated++);
            localOrchestrator.OnTriggerDropping(tca => onDropping++);
            localOrchestrator.OnTriggerDropped(tca => onDropped++);

            var isDropped = await localOrchestrator.DropTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);

            Assert.IsTrue(isDropped);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(1, onDropping);
            Assert.AreEqual(1, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.GetTriggerAsync(c, null, "Product_insert_trigger", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(0, check.Rows.Count);
                c.Close();
            }

            // try to delete a non existing one
            isDropped = await localOrchestrator.DropTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Update);

            Assert.IsFalse(isDropped);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Trigger_Drop_One_Cancel()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);
            Assert.IsTrue(isCreated);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTriggerCreating(tca => onCreating++);

            localOrchestrator.OnTriggerCreated(tca => onCreated++);
            localOrchestrator.OnTriggerDropping(tca =>
            {
                tca.Cancel = true;
                onDropping++;
            });
            localOrchestrator.OnTriggerDropped(tca => onDropped++);

            var isDropped = await localOrchestrator.DropTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);

            Assert.IsFalse(isDropped);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(1, onDropping);
            Assert.AreEqual(0, onDropped);

            // Check 
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var check = await SqlManagementUtils.GetTriggerAsync(c, null, "Product_insert_trigger", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(1, check.Rows.Count);
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Trigger_Create_One_Overwrite()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTriggerCreating(tca => onCreating++);
            localOrchestrator.OnTriggerCreated(tca => onCreated++);
            localOrchestrator.OnTriggerDropping(tca => onDropping++);
            localOrchestrator.OnTriggerDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(1, onCreating);
            Assert.AreEqual(1, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert);

            Assert.IsFalse(isCreated);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            isCreated = await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert, true);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(1, onCreating);
            Assert.AreEqual(1, onCreated);
            Assert.AreEqual(1, onDropping);
            Assert.AreEqual(1, onDropped);


            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Trigger_Drop_All()
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

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTriggerCreating(tca => onCreating++);
            localOrchestrator.OnTriggerCreated(tca => onCreated++);
            localOrchestrator.OnTriggerDropping(tca => onDropping++);
            localOrchestrator.OnTriggerDropped(tca => onDropped++);

            var isCreated = await localOrchestrator.CreateTriggersAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(3, onCreating);
            Assert.AreEqual(3, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);


            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            var isDropped = await localOrchestrator.DropTriggersAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(isCreated);
            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(3, onDropping);
            Assert.AreEqual(3, onDropped);


            _databaseHelper.DropDatabase(dbName);
        }
    }
}
