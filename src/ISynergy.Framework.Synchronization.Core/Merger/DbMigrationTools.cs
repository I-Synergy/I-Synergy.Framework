using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Merger
{
    public class DbMigrationTools
    {
        private IProvider _provider;
        private readonly SyncOptions _newOptions;
        private SyncSetup _newSetup;
        private readonly string _currentScopeInfoTableName;

        public DbMigrationTools() { }

        public DbMigrationTools(IProvider provider, SyncOptions options, SyncSetup setup, string currentScopeInfoTableName = null)
        {
            _provider = provider;
            _newOptions = options;
            _newSetup = setup;
            _currentScopeInfoTableName = currentScopeInfoTableName;
        }


        public Task MigrateAsync(SyncContext context)
        {

            return Task.CompletedTask;
            //DbTransaction transaction = null;

            //using (var connection = provider.CreateConnection())
            //{
            //    // Encapsulate in a try catch for a better exception handling
            //    // Especially whe called from web proxy
            //    try
            //    {
            //        await connection.OpenAsync().ConfigureAwait(false);

            //        // Let provider knows a connection is opened
            //        provider.OnConnectionOpened(connection);

            //        await provider.InterceptAsync(new ConnectionOpenArgs(context, connection)).ConfigureAwait(false);

            //        // Create a transaction
            //        using (transaction = connection.BeginTransaction(provider.IsolationLevel))
            //        {

            //            await provider.InterceptAsync(new TransactionOpenArgs(context, connection, transaction)).ConfigureAwait(false);


            //            // actual scope info table name
            //            var scopeInfoTableName = string.IsNullOrEmpty(currentScopeInfoTableName) ? newOptions.ScopeInfoTableName : currentScopeInfoTableName;

            //            // create a temp sync context
            //            ScopeInfo localScope = null;

            //            var scopeBuilder = provider.GetScopeBuilder();
            //            var scopeInfoBuilder = scopeBuilder.CreateScopeInfoBuilder(scopeInfoTableName, connection, transaction);

            //            // if current scope table name does not exists, it's probably first sync. so return
            //            if (scopeInfoBuilder.NeedToCreateClientScopeInfoTable())
            //                return;

            //            // Get scope
            //            (context, localScope) = await provider.GetClientScopeAsync(
            //                                context, newOptions.ScopeName,
            //                                connection, transaction, CancellationToken.None).ConfigureAwait(false);

            //            // Get current schema saved in local database
            //            if (localScope is null || string.IsNullOrEmpty(localScope.Schema))
            //                return;

            //            var currentSchema = JsonConvert.DeserializeObject<SyncSet>(localScope.Schema);

            //            // Create new schema based on new setup
            //            var newSchema = provider.ReadSchema(newSetup, connection, transaction);


            //            // Get Tables that are NOT in new schema anymore
            //            var deletedSyncTables = currentSchema.Tables.Where(currentTable => !newSchema.Tables.Any(newTable => newTable == currentTable));

            //            foreach (var dSyncTable in deletedSyncTables)
            //            {
            //                // get builder
            //                var delBuilder = provider.GetTableBuilder(dSyncTable);

            //                // Delete all stored procedures
            //                delBuilder.DropProcedures(connection, transaction);

            //                // Delete all triggers
            //                delBuilder.DropTriggers(connection, transaction);

            //                // Delete tracking table
            //                delBuilder.DropTrackingTable(connection, transaction);
            //            }

            //            // Get Tables that are completely new
            //            var addSyncTables = newSchema.Tables.Where(newTable => !currentSchema.Tables.Any(currentTable => newTable == currentTable));

            //            foreach (var aSyncTable in addSyncTables)
            //            {
            //                // get builder
            //                var addBuilder = provider.GetTableBuilder(aSyncTable);

            //                // Create table if not exists
            //                addBuilder.CreateTable(connection, transaction);

            //                // Create tracking table
            //                addBuilder.CreateTrackingTable(connection, transaction);

            //                // Create triggers
            //                addBuilder.CreateTriggers(connection, transaction);

            //                // Create stored procedures
            //                addBuilder.CreateStoredProcedures(connection, transaction);
            //            }

            //            var editSyncTables = newSchema.Tables.Where(newTable => currentSchema.Tables.Any(currentTable => newTable == currentTable));

            //            foreach (var eSyncTable in editSyncTables)
            //            {
            //                var cSyncTable = currentSchema.Tables.First(t => t == eSyncTable);

            //                var migrationTable = new DbMigrationTable(provider, cSyncTable, eSyncTable, true);
            //                migrationTable.Compare();
            //                //migrationTable.Apply(connection, transaction);
            //            }

            //            await provider.InterceptAsync(new TransactionCommitArgs(null, connection, transaction)).ConfigureAwait(false);
            //            transaction.Commit();
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //        var syncException = new SyncException(ex, context.SyncStage);

            //        // try to let the provider enrich the exception
            //        provider.EnsureSyncException(syncException);
            //        syncException.Side = SyncExceptionSide.ClientSide;
            //        throw syncException;
            //    }
            //    finally
            //    {
            //        if (transaction is not null)
            //            transaction.Dispose();

            //        if (connection is not null && connection.State == ConnectionState.Open)
            //            connection.Close();

            //        await provider.InterceptAsync(new ConnectionCloseArgs(context, connection, transaction)).ConfigureAwait(false);

            //        // Let provider knows a connection is closed
            //        provider.OnConnectionClosed(connection);
            //    }

            //}

        }



        public void Apply(DbConnection connection, DbTransaction transaction = null)
        {
            //var alreadyOpened = connection.State != ConnectionState.Closed;

            //if (!alreadyOpened)
            //    connection.Open();

            //// get builder
            //var newBuilder = provider.GetTableBuilder(newTable);
            //var currentBuilder = provider.GetTableBuilder(currentTable);

            //if (NeedRecreateTrackingTable)
            //{
            //    // drop triggers, stored procedures, tvp, tracking table
            //    currentBuilder.DropTriggers(connection, transaction);
            //    currentBuilder.DropProcedures(connection, transaction);
            //    currentBuilder.DropTrackingTable(connection, transaction);

            //    newBuilder.CreateTrackingTable(connection, transaction);
            //    newBuilder.CreateStoredProcedures(connection, transaction);
            //    newBuilder.CreateTriggers(connection, transaction);

            //    NeedRecreateStoredProcedures = false;
            //    NeedRecreateTriggers = false;
            //    NeedRecreateTrackingTable = false;
            //    NeedRenameTrackingTable = false;
            //}

            //if (NeedRecreateStoredProcedures)
            //{
            //    currentBuilder.DropProcedures(connection, transaction);
            //    newBuilder.CreateStoredProcedures(connection, transaction);

            //    NeedRecreateStoredProcedures = false;
            //}

            //if (NeedRecreateTriggers)
            //{
            //    currentBuilder.DropTriggers(connection, transaction);
            //    newBuilder.CreateTriggers(connection, transaction);

            //    NeedRecreateTriggers = false;
            //}

            //if (!alreadyOpened)
            //    connection.Close();

        }
    }
}
