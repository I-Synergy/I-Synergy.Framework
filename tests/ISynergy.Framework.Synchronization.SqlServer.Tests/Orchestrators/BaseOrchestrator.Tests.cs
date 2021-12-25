using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Base;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class BaseOrchestratorTests : BaseTest
    {
        [Ignore]
        public void BaseOrchestrator_Constructor()
        {
            var provider = new SqlSyncProvider();
            var options = new SyncOptions();
            var setup = new SyncSetup();
            var orchestrator = new LocalOrchestrator(_versionService, provider, options, setup);

            Assert.IsNotNull(orchestrator.Options);
            Assert.AreSame(options, orchestrator.Options);

            Assert.IsNotNull(orchestrator.Provider);
            Assert.AreSame(provider, orchestrator.Provider);

            Assert.IsNotNull(orchestrator.Setup);
            Assert.AreSame(setup, orchestrator.Setup);

            Assert.IsNotNull(provider.Orchestrator);
            Assert.AreSame(provider.Orchestrator, orchestrator);

        }

        [Ignore]
        public void BaseOrchestrator_ShouldFail_When_Args_AreNull()
        {
            var provider = new SqlSyncProvider();
            var options = new SyncOptions();
            var setup = new SyncSetup();

            Assert.ThrowsException<ArgumentNullException>(() => new LocalOrchestrator(_versionService, null, null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new LocalOrchestrator(_versionService, provider, null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new LocalOrchestrator(_versionService, provider, options, null));
            Assert.ThrowsException<ArgumentNullException>(() => new LocalOrchestrator(_versionService, null, options, setup));
            Assert.ThrowsException<ArgumentNullException>(() => new LocalOrchestrator(_versionService, null, null, setup));
        }

        [Ignore]
        public void BaseOrchestrator_GetContext_ShouldBeInitialized()
        {
            var options = new SyncOptions();
            var setup = new SyncSetup();
            var provider = new SqlSyncProvider();

            var localOrchestrator = new LocalOrchestrator(_versionService, provider, options, setup, "scope1");

            var ctx = localOrchestrator.GetContext();

            Assert.AreEqual(SyncStage.None, ctx.SyncStage);
            Assert.AreEqual(localOrchestrator.ScopeName, ctx.ScopeName);
            Assert.AreEqual(SyncType.Normal, ctx.SyncType);
            Assert.AreEqual(SyncWay.None, ctx.SyncWay);
            Assert.IsNull(ctx.Parameters);
        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_ShouldFail_If_SetupIsEmpty()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);

            var options = new SyncOptions();
            var setup = new SyncSetup();
            var provider = new SqlSyncProvider(cs);

            var orchestrator = new RemoteOrchestrator(_versionService, provider, options, setup);

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await orchestrator.GetSchemaAsync());

            Assert.AreEqual(SyncStage.SchemaReading, se.SyncStage);
            Assert.AreEqual(SyncSide.ServerSide, se.Side);
            Assert.AreEqual("MissingTablesException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);

        }

        internal static void AssertConnectionAndTransaction(BaseOrchestrator orchestrator, SyncStage stage)
        {
            orchestrator.OnConnectionOpen(args =>
            {
                Assert.IsInstanceOfType(args, typeof(ConnectionOpenedArgs));
                Assert.AreEqual(stage, args.Context.SyncStage);
                Assert.AreEqual(orchestrator.ScopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
            });
            orchestrator.OnTransactionOpen(args =>
            {
                Assert.IsInstanceOfType(args, typeof(TransactionOpenedArgs));
                Assert.AreEqual(stage, args.Context.SyncStage);
                Assert.AreEqual(orchestrator.ScopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                Assert.AreSame(args.Connection, args.Transaction.Connection);
            });
            orchestrator.OnTransactionCommit(args =>
            {
                Assert.IsInstanceOfType(args, typeof(TransactionCommitArgs));
                Assert.AreEqual(stage, args.Context.SyncStage);
                Assert.AreEqual(orchestrator.ScopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                Assert.AreSame(args.Connection, args.Transaction.Connection);
            });
            orchestrator.OnConnectionClose(args =>
            {
                Assert.IsInstanceOfType(args, typeof(ConnectionClosedArgs));
                Assert.AreEqual(stage, args.Context.SyncStage);
                Assert.AreEqual(orchestrator.ScopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Closed, args.Connection.State);
            });

        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_ShouldReturnSchema()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();
            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(this.Tables);

            var onSchemaRead = false;
            var onSchemaReading = false;

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            localOrchestrator.OnSchemaLoading(args =>
            {
                onSchemaReading = true;
            });

            localOrchestrator.OnSchemaLoaded(args =>
            {
                Assert.IsInstanceOfType(args, typeof(SchemaLoadedArgs));
                Assert.AreEqual(SyncStage.SchemaReading, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                Assert.AreEqual(16, args.Schema.Tables.Count);
                onSchemaRead = true;

            });

            AssertConnectionAndTransaction(localOrchestrator, SyncStage.SchemaReading);

            var schema = await localOrchestrator.GetSchemaAsync();

            Assert.IsNotNull(schema);
            Assert.AreEqual(SyncStage.SchemaReading, localOrchestrator.GetContext().SyncStage);
            Assert.AreEqual(16, schema.Tables.Count);
            Assert.IsTrue(onSchemaRead);
            Assert.IsTrue(onSchemaReading);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_CancellationToken_ShouldInterrupt()
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
            using var cts = new CancellationTokenSource();

            localOrchestrator.OnConnectionOpen(args =>
            {
                Assert.AreEqual(SyncStage.SchemaReading, args.Context.SyncStage);
                Assert.IsInstanceOfType(args, typeof(ConnectionOpenedArgs));
                Assert.IsNotNull(args.Connection);
                Assert.IsNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                cts.Cancel();
            });

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
            {
                var schema = await localOrchestrator.GetSchemaAsync(cancellationToken:cts.Token);
            });

            Assert.AreEqual(SyncStage.SchemaReading, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("OperationCanceledException", se.TypeName);


            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_SetupColumnsDefined_ShouldReturn_SchemaWithSetupColumnsOnly()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            // Create a bad setup with a non existing table

            var tables = new string[] { "Customer", "Address", "CustomerAddress" };
            var setup = new SyncSetup(tables);
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "FirstName", "LastName", "CompanyName" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var schema = await localOrchestrator.GetSchemaAsync();

            Assert.AreEqual(SyncStage.SchemaReading, localOrchestrator.GetContext().SyncStage);
            Assert.AreEqual(3, schema.Tables.Count);

            // Only 4 columns shoud be part of Customer table
            Assert.AreEqual(4, schema.Tables["Customer"].Columns.Count);

            Assert.AreEqual(9, schema.Tables["Address"].Columns.Count);
            Assert.AreEqual(5, schema.Tables["CustomerAddress"].Columns.Count);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_NoPrimaryKeysColumn_ShouldFail()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            // Create a bad setup with a non existing table

            var tables = new string[] { "Customer", "Address", "CustomerAddress" };
            var setup = new SyncSetup(tables);
            setup.Tables["Customer"].Columns.AddRange(new string[] { "FirstName", "LastName", "CompanyName" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
            {
                var schema = await localOrchestrator.GetSchemaAsync();
            });

            Assert.AreEqual(SyncStage.SchemaReading, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingPrimaryKeyColumnException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_NonExistingColumns_ShouldFail()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            // Create a bad setup with a non existing table

            var tables = new string[] { "Customer", "Address", "CustomerAddress" };
            var setup = new SyncSetup(tables);
            setup.Tables["Customer"].Columns.AddRange(new string[] { "FirstName", "LastName", "CompanyName", "BADCOLUMN" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
            {
                var schema = await localOrchestrator.GetSchemaAsync();
            });

            Assert.AreEqual(SyncStage.SchemaReading, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingColumnException", se.TypeName);


            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_GetSchema_NonExistingTables_ShouldFail()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            // Create a bad setup with a non existing table

            var tables = new string[]
            {
                "SalesLT.ProductCategory", "SalesLT.ProductModel", "SalesLT.Product", "Employee", "Customer", "Address", "CustomerAddress", "EmployeeAddress",
                "SalesLT.SalesOrderHeader", "SalesLT.SalesOrderDetail", "Posts", "Tags", "PostTag",
                "PricesList", "PricesListCategory", "PricesListDetail", "WRONGTABLE"
            };
            var setup = new SyncSetup(tables);

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
            {
                var schema = await localOrchestrator.GetSchemaAsync();
            });

            Assert.AreEqual(SyncStage.SchemaReading, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingTableException", se.TypeName);


            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task BaseOrchestrator_Provision_ShouldFail_If_SetupIsEmpty()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();
            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup();

            var orchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await orchestrator.ProvisionAsync(provision));

            Assert.AreEqual(SyncStage.Provisioning, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingTablesException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Provision_SchemaCreated_If_SetupHasTables()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();
            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

            var schema = await localOrchestrator.ProvisionAsync(provision);

            var context = localOrchestrator.GetContext();

            Assert.AreEqual(SyncStage.Provisioning, context.SyncStage);
            Assert.AreEqual(1, schema.Tables.Count);
            Assert.AreEqual("SalesLT.Product", schema.Tables[0].GetFullName());
            Assert.AreEqual(17, schema.Tables[0].Columns.Count);

            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task BaseOrchestrator_Provision_SchemaNotCreated_If_SetupHasTables_AndDbIsEmpty()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await localOrchestrator.ProvisionAsync(provision));

            Assert.AreEqual(SyncStage.Provisioning, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingTableException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Provision_SchemaCreated_If_SchemaHasColumnsDefinition()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup();

            var schema = new SyncSet();
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            schema.Tables.Add(table);

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

            await localOrchestrator.ProvisionAsync(schema, provision);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var tbl = await SqlManagementUtils.GetTableAsync(c, null, "Product", "SalesLT");

                var tblName = tbl.Rows[0]["TableName"].ToString();
                var schName = tbl.Rows[0]["SchemaName"].ToString();

                Assert.AreEqual(table.TableName, tblName);
                Assert.AreEqual(table.SchemaName, schName);

                var cols = await SqlManagementUtils.GetColumnsForTableAsync(c, null, "Product", "SalesLT");

                Assert.AreEqual(3, cols.Rows.Count);

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_Provision_ShouldCreate_TrackingTable()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup { TrackingTablesSuffix = "sync", TrackingTablesPrefix = "trck" };

            var schema = new SyncSet();
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            //schema.TrackingTablesSuffix = "sync";
            //schema.TrackingTablesPrefix = "trck";

            schema.Tables.Add(table);

            // trackign table name is composed with prefix and suffix from setup
            var trackingTableName = $"{setup.TrackingTablesPrefix}{table.TableName}{setup.TrackingTablesSuffix}";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.TrackingTable;

            await localOrchestrator.ProvisionAsync(schema, provision);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var tbl = await SqlManagementUtils.GetTableAsync(c, null, trackingTableName, "SalesLT");

                var tblName = tbl.Rows[0]["TableName"].ToString();
                var schName = tbl.Rows[0]["SchemaName"].ToString();

                Assert.AreEqual(trackingTableName, tblName);
                Assert.AreEqual(table.SchemaName, schName);

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }

      
        [Ignore]
        public async Task BaseOrchestrator_Provision_ShouldCreate_StoredProcedures()
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

            setup.StoredProceduresPrefix = "s";
            setup.StoredProceduresSuffix = "proc";

            // trackign table name is composed with prefix and suffix from setup
            var bulkDelete = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_bulkdelete";
            var bulkUpdate = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_bulkupdate";
            var changes = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_changes";
            var delete = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_delete";
            var deletemetadata = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_deletemetadata";
            var initialize = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_initialize";
            var reset = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_reset";
            var selectrow = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_selectrow";
            var update = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_update";

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            // Needs the tracking table to be able to create stored procedures
            var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures;

            await localOrchestrator.ProvisionAsync(provision);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, bulkDelete));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, bulkUpdate));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, changes));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, delete));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, deletemetadata));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, initialize));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, reset));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, selectrow));
                Assert.IsTrue(await SqlManagementUtils.ProcedureExistsAsync(c, null, update));

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task BaseOrchestrator_Provision_ShouldCreate_Table()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup();

            var schema = new SyncSet();
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));
            table.PrimaryKeys.Add("ID");

            schema.Tables.Add(table);

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table;

            await localOrchestrator.ProvisionAsync(schema, provision);

            using (var c = new SqlConnection(cs))
            {
                await c.OpenAsync().ConfigureAwait(false);

                var tbl = await SqlManagementUtils.GetTableAsync(c, null, "Product", "SalesLT");

                var tblName = tbl.Rows[0]["TableName"].ToString();
                var schName = tbl.Rows[0]["SchemaName"].ToString();

                Assert.AreEqual(table.TableName, tblName);
                Assert.AreEqual(table.SchemaName, schName);

                c.Close();
            }

            _databaseHelper.DropDatabase(dbName);
        }



        [Ignore]
        public async Task BaseOrchestrator_Provision_SchemaFail_If_SchemaHasColumnsDefinitionButNoPrimaryKey()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup();

            var schema = new SyncSet();
            var table = new SyncTable("Product", "SalesLT");
            var colID = new SyncColumn("ID", typeof(Guid));
            var colName = new SyncColumn("Name", typeof(string));

            table.Columns.Add(colID);
            table.Columns.Add(colName);
            table.Columns.Add("Number", typeof(int));

            schema.Tables.Add(table);

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;


            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await localOrchestrator.ProvisionAsync(schema, provision));

            Assert.AreEqual(SyncStage.Provisioning, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingPrimaryKeyException", se.TypeName);



            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task BaseOrchestrator_Provision_ShouldFails_If_SetupTable_DoesNotExist()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();
            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.badTable" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var provision = SyncProvision.Table | SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

            var se = await Assert.ThrowsExceptionAsync<SyncException>(async () => await localOrchestrator.ProvisionAsync(provision));

            Assert.AreEqual(SyncStage.Provisioning, se.SyncStage);
            Assert.AreEqual(SyncSide.ClientSide, se.Side);
            Assert.AreEqual("MissingTableException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);
        }



    }
}
