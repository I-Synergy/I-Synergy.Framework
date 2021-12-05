using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class BaseOrchestratorTests
    {
        [Ignore]
        public async Task BaseOrchestrator_Provision_ShouldCreate_Triggers()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            setup.TrackingTablesSuffix = "sync";
            setup.TrackingTablesPrefix = "trck";

            setup.TriggersPrefix = "trg_";
            setup.TriggersSuffix = "_trg";


            // trackign table name is composed with prefix and suffix from setup
            var triggerDelete = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_delete_trigger";
            var triggerInsert = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_insert_trigger";
            var triggerUpdate = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_update_trigger";

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            // Needs the tracking table to be able to create triggers
            var provision = SyncProvision.TrackingTable | SyncProvision.Triggers;

            await localOrchestrator.ProvisionAsync(provision);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trigDel = await SqlManagementUtils.GetTriggerAsync(c, null, triggerDelete, "SalesLT");
                Assert.AreEqual(triggerDelete, trigDel.Rows[0]["Name"].ToString());

                var trigIns = await SqlManagementUtils.GetTriggerAsync(c, null, triggerInsert, "SalesLT");
                Assert.AreEqual(triggerInsert, trigIns.Rows[0]["Name"].ToString());

                var trigUdate = await SqlManagementUtils.GetTriggerAsync(c, null, triggerUpdate, "SalesLT");
                Assert.AreEqual(triggerUpdate, trigUdate.Rows[0]["Name"].ToString());

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Trigger_ShouldCreate()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            setup.TriggersPrefix = "trg_";
            setup.TriggersSuffix = "_trg";

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            var triggerInsert = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_insert_trigger";
            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert, false);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trigIns = await SqlManagementUtils.GetTriggerAsync(c, null, triggerInsert, "SalesLT");
                Assert.AreEqual(triggerInsert, trigIns.Rows[0]["Name"].ToString());

                c.Close();
            }

            var triggerUpdate = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_update_trigger";
            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Update, false);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trig = await SqlManagementUtils.GetTriggerAsync(c, null, triggerUpdate, "SalesLT");
                Assert.AreEqual(triggerUpdate, trig.Rows[0]["Name"].ToString());

                c.Close();
            }

            var triggerDelete = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_delete_trigger";
            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Delete, false);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trig = await SqlManagementUtils.GetTriggerAsync(c, null, triggerDelete, "SalesLT");
                Assert.AreEqual(triggerDelete, trig.Rows[0]["Name"].ToString());

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Trigger_ShouldOverwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                TriggersPrefix = "trg_",
                TriggersSuffix = "_trg"
            };

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            var triggerInsert = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_insert_trigger";
            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert, false);

            var assertOverWritten = false;
            localOrchestrator.On(new Action<TriggerCreatingArgs>(args =>
            {
               assertOverWritten = true;
            }));

            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert, true);

            Assert.IsTrue(assertOverWritten);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Trigger_ShouldNotOverwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                TriggersPrefix = "trg_",
                TriggersSuffix = "_trg"
            };

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            var triggerInsert = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_insert_trigger";
            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert, false);


            var assertOverWritten = false;
            localOrchestrator.On(new Action<TriggerCreatingArgs>(args =>
            {
                assertOverWritten = true;
            }));

            await localOrchestrator.CreateTriggerAsync(setup.Tables["Product", "SalesLT"], DbTriggerType.Insert, false);

            Assert.IsFalse(assertOverWritten);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Trigger_Exists()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                TriggersPrefix = "trg_",
                TriggersSuffix = "_trg"
            };

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            var productTable = setup.Tables["Product", "SalesLT"];

            await localOrchestrator.CreateTriggerAsync(productTable, DbTriggerType.Insert, false);

            var insertExists = await localOrchestrator.ExistTriggerAsync(productTable, DbTriggerType.Insert);
            var updateExists = await localOrchestrator.ExistTriggerAsync(productTable, DbTriggerType.Update);

            Assert.IsTrue(insertExists);
            Assert.IsFalse(updateExists);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Triggers_ShouldCreate()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            setup.TriggersPrefix = "trg_";
            setup.TriggersSuffix = "_trg";

            var localOrchestrator = new LocalOrchestrator(sqlProvider, options, setup, scopeName);

            var triggerInsert = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_insert_trigger";
            var triggerUpdate = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_update_trigger";
            var triggerDelete = $"{setup.TriggersPrefix}Product{setup.TriggersSuffix}_delete_trigger";

            await localOrchestrator.CreateTriggersAsync(setup.Tables["Product", "SalesLT"]);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trigIns = await SqlManagementUtils.GetTriggerAsync(c, null, triggerInsert, "SalesLT");
                Assert.AreEqual(triggerInsert, trigIns.Rows[0]["Name"].ToString());

                c.Close();
            }

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trig = await SqlManagementUtils.GetTriggerAsync(c, null, triggerUpdate, "SalesLT");
                Assert.AreEqual(triggerUpdate, trig.Rows[0]["Name"].ToString());

                c.Close();
            }

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var trig = await SqlManagementUtils.GetTriggerAsync(c, null, triggerDelete, "SalesLT");
                Assert.AreEqual(triggerDelete, trig.Rows[0]["Name"].ToString());

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }



    }
}
