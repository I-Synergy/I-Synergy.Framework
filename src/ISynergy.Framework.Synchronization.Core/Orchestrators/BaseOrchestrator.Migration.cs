﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public abstract partial class BaseOrchestrator
    {

        /// <summary>
        /// Migrate from a setup to another setup
        /// </summary>
        internal async Task<SyncContext> InternalMigrationAsync(SyncContext context, SyncSet schema, SyncSetup oldSetup, SyncSetup newSetup,
                             DbConnection connection, DbTransaction transaction,
                             CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            // Create a new migration
            var migration = new Migration(newSetup, oldSetup);

            // get comparision results
            var migrationResults = migration.Compare();

            // Launch InterceptAsync on Migrating
            await InterceptAsync(new MigratingArgs(context, schema, oldSetup, newSetup, migrationResults, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            // Deprovision triggers stored procedures and tracking table if required
            foreach (var migrationTable in migrationResults.Tables)
            {
                // using a fake SyncTable based on oldSetup, since we don't need columns, but we need to have the filters
                var schemaTable = new SyncTable(migrationTable.SetupTable.TableName, migrationTable.SetupTable.SchemaName);

                // Create a temporary SyncSet for attaching to the schemaTable
                var tmpSchema = new SyncSet();

                // Add this table to schema
                tmpSchema.Tables.Add(schemaTable);

                tmpSchema.EnsureSchema();

                // copy filters from old setup
                foreach (var filter in oldSetup.Filters)
                    tmpSchema.Filters.Add(filter);

                // using a fake Synctable, since we don't need columns to deprovision
                var tableBuilder = GetTableBuilder(schemaTable, oldSetup);

                // Deprovision stored procedures
                if (migrationTable.StoredProcedures == MigrationAction.Drop || migrationTable.StoredProcedures == MigrationAction.Create)
                    await InternalDropStoredProceduresAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // Deprovision triggers
                if (migrationTable.Triggers == MigrationAction.Drop || migrationTable.Triggers == MigrationAction.Create)
                    await InternalDropTriggersAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // Deprovision tracking table
                if (migrationTable.TrackingTable == MigrationAction.Drop || migrationTable.TrackingTable == MigrationAction.Create)
                {
                    var exists = await InternalExistsTrackingTableAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        await InternalDropTrackingTableAsync(context, oldSetup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                // Removing cached commands
                var syncAdapter = GetSyncAdapter(schemaTable, oldSetup);
                syncAdapter.RemoveCommands();
            }

            // Provision table (create or alter), tracking tables, stored procedures and triggers
            // Need the real SyncSet since we need columns definition
            foreach (var migrationTable in migrationResults.Tables)
            {
                var syncTable = schema.Tables[migrationTable.SetupTable.TableName, migrationTable.SetupTable.SchemaName];
                var oldTable = oldSetup.Tables[migrationTable.SetupTable.TableName, migrationTable.SetupTable.SchemaName];

                if (syncTable is null)
                    continue;

                var tableBuilder = GetTableBuilder(syncTable, newSetup);

                // Re provision table
                if (migrationTable.Table == MigrationAction.Create)
                {
                    // Check if we need to create a schema there
                    var schemaExists = await InternalExistsSchemaAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!schemaExists)
                        await InternalCreateSchemaAsync(context, newSetup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    var exists = await InternalExistsTableAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!exists)
                        await InternalCreateTableAsync(context, newSetup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                // Re provision table
                if (migrationTable.Table == MigrationAction.Alter)
                {
                    var exists = await InternalExistsTableAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!exists)
                    {
                        await InternalCreateTableAsync(context, newSetup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    }
                    else if (oldTable is not null)
                    {
                        //get new columns to add
                        var newColumns = syncTable.Columns.Where(c => !oldTable.Columns.Any(oldC => string.Equals(oldC, c.ColumnName, SyncGlobalization.DataSourceStringComparison)));

                        if (newColumns is not null)
                        {
                            foreach (var newColumn in newColumns)
                            {
                                var columnExist = await InternalExistsColumnAsync(context, newColumn.ColumnName, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                                if (!columnExist)
                                    await InternalAddColumnAsync(context, newSetup, newColumn.ColumnName, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                            }
                        }
                    }
                }

                // Re provision tracking table
                if (migrationTable.TrackingTable == MigrationAction.Rename && oldTable is not null)
                {
                    var (_, oldTableName) = Provider.GetParsers(new SyncTable(oldTable.TableName, oldTable.SchemaName), oldSetup);

                    await InternalRenameTrackingTableAsync(context, newSetup, oldTableName, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }
                else if (migrationTable.TrackingTable == MigrationAction.Create)
                {
                    var exists = await InternalExistsTrackingTableAsync(context, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    if (exists)
                        await InternalDropTrackingTableAsync(context, newSetup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    await InternalCreateTrackingTableAsync(context, newSetup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                // Re provision stored procedures
                if (migrationTable.StoredProcedures == MigrationAction.Create)
                    await InternalCreateStoredProceduresAsync(context, true, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // Re provision triggers
                if (migrationTable.Triggers == MigrationAction.Create)
                    await InternalCreateTriggersAsync(context, true, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // InterceptAsync Migrated
            var args = new MigratedArgs(context, schema, newSetup, migrationResults, connection, transaction);
            await InterceptAsync(args, progress, cancellationToken).ConfigureAwait(false);

            return context;
        }


    }
}
