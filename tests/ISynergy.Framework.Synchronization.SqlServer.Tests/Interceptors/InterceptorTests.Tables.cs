using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task Table_Create_One()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = false;
            var onCreated = false;

            localOrchestrator.OnTableCreating(ttca =>
            {
                var addingID = Environment.NewLine + $"ALTER TABLE {ttca.TableName.Schema().Quoted()} ADD internal_id int identity(1,1)";
                ttca.Command.CommandText += addingID;
                onCreating = true;
            });

            localOrchestrator.OnTableCreated(ttca =>
            {
                onCreated = true;
            });

            var isCreated = await localOrchestrator.CreateTableAsync(table);

            Assert.IsTrue(isCreated);
            Assert.IsTrue(onCreating);
            Assert.IsTrue(onCreated);


            // Check we have a new column in tracking table
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var cols = await SqlManagementUtils.GetColumnsForTableAsync(c, null, "Product", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(4, cols.Rows.Count);
                Assert.IsNotNull(cols.Rows.FirstOrDefault(r => r["name"].ToString() == "internal_id"));
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Table_Exists()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product", "SalesLT.ProductCategory" });

            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            await localOrchestrator.CreateTableAsync(table);

            var exists = await localOrchestrator.ExistTableAsync(setup.Tables[0]).ConfigureAwait(false);
            Assert.IsTrue(exists);
            exists = await localOrchestrator.ExistTableAsync(setup.Tables[1]).ConfigureAwait(false);
            Assert.IsFalse(exists);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Table_Create_One_Overwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            // Overwrite existing table with this new one
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            // Call create a first time to have an existing table
            var isCreated = await localOrchestrator.CreateTableAsync(table);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = false;
            var onCreated = false;
            var onDropping = false;
            var onDropped = false;

            localOrchestrator.OnTableCreating(ttca => onCreating = true);
            localOrchestrator.OnTableCreated(ttca => onCreated = true);
            localOrchestrator.OnTableDropping(ttca => onDropping = true);
            localOrchestrator.OnTableDropped(ttca => onDropped = true);

            isCreated = await localOrchestrator.CreateTableAsync(table);

            Assert.IsFalse(isCreated);
            Assert.IsFalse(onDropping);
            Assert.IsFalse(onDropped);
            Assert.IsFalse(onCreating);
            Assert.IsFalse(onCreated);

            onCreating = false;
            onCreated = false;
            onDropping = false;
            onDropped = false;

            isCreated = await localOrchestrator.CreateTableAsync(table, true);

            Assert.IsTrue(isCreated);
            Assert.IsTrue(onDropping);
            Assert.IsTrue(onDropped);
            Assert.IsTrue(onCreating);
            Assert.IsTrue(onCreated);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Table_Create_All()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.ProductCategory", "SalesLT.ProductModel", "SalesLT.Product", "Posts" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var schema = await localOrchestrator.GetSchemaAsync();

            // new empty db
            dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            cs = _databaseHelper.GetConnectionString(dbName);
            sqlProvider = new SqlSyncProvider(cs);

            localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);


            var onCreating = 0;
            var onCreated = 0;
            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTableCreating(ttca => onCreating++);
            localOrchestrator.OnTableCreated(ttca => onCreated++);
            localOrchestrator.OnTableDropping(ttca => onDropping++);
            localOrchestrator.OnTableDropped(ttca => onDropped++);

            await localOrchestrator.CreateTablesAsync(schema);

            Assert.AreEqual(4, onCreating);
            Assert.AreEqual(4, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            onCreating = 0;
            onCreated = 0;
            onDropping = 0;
            onDropped = 0;

            await localOrchestrator.CreateTablesAsync(schema);

            Assert.AreEqual(0, onCreating);
            Assert.AreEqual(0, onCreated);
            Assert.AreEqual(0, onDropping);
            Assert.AreEqual(0, onDropped);

            await localOrchestrator.CreateTablesAsync(schema, true);

            Assert.AreEqual(4, onCreating);
            Assert.AreEqual(4, onCreated);
            Assert.AreEqual(4, onDropping);
            Assert.AreEqual(4, onDropped);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Table_Drop_One()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            // Overwrite existing table with this new one
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            // Call create a first time to have an existing table
            var isCreated = await localOrchestrator.CreateTableAsync(table);

            Assert.IsTrue(isCreated);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = false;
            var onCreated = false;
            var onDropping = false;
            var onDropped = false;

            localOrchestrator.OnTableCreating(ttca => onCreating = true);
            localOrchestrator.OnTableCreated(ttca => onCreated = true);
            localOrchestrator.OnTableDropping(ttca => onDropping = true);
            localOrchestrator.OnTableDropped(ttca => onDropped = true);

            var isDropped = await localOrchestrator.DropTableAsync(setup.Tables[0]);

            Assert.IsTrue(isDropped);
            Assert.IsTrue(onDropping);
            Assert.IsTrue(onDropped);
            Assert.IsFalse(onCreating);
            Assert.IsFalse(onCreated);

            // Check we have the correct table ovewritten
            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var stable = await SqlManagementUtils.GetTableAsync(c, null, "Product", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(0, stable.Rows.Count);
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Table_Drop_One_Cancel()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            // Overwrite existing table with this new one
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            // Call create a first time to have an existing table
            var isCreated = await localOrchestrator.CreateTableAsync(table);

            Assert.IsTrue(isCreated);

            // Ensuring we have a clean new instance
            localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onCreating = false;
            var onCreated = false;
            var onDropping = false;
            var onDropped = false;

            localOrchestrator.OnTableCreating(ttca => onCreating = true);
            localOrchestrator.OnTableCreated(ttca => onCreated = true);
            localOrchestrator.OnTableDropped(ttca => onDropped = true);

            localOrchestrator.OnTableDropping(ttca =>
            {
                ttca.Cancel = true;
                onDropping = true;
            });

            var isDropped = await localOrchestrator.DropTableAsync(setup.Tables[0]);

            Assert.IsTrue(onDropping);

            Assert.IsFalse(isDropped);
            Assert.IsFalse(onDropped);
            Assert.IsFalse(onCreating);
            Assert.IsFalse(onCreated);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);
                var stable = await SqlManagementUtils.GetTableAsync(c, null, "Product", "SalesLT").ConfigureAwait(false);
                Assert.AreEqual(1, stable.Rows.Count);
                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task Table_Drop_All()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(this.Tables);

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onDropping = 0;
            var onDropped = 0;

            localOrchestrator.OnTableDropping(ttca => onDropping++);
            localOrchestrator.OnTableDropped(ttca => onDropped++);

            await localOrchestrator.DropTablesAsync();

            Assert.AreEqual(this.Tables.Length, onDropping);
            Assert.AreEqual(this.Tables.Length, onDropped);

            _databaseHelper.DropDatabase(dbName);
        }
    }
}
