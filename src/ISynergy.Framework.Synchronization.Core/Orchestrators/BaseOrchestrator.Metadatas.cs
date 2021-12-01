﻿using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public abstract partial class BaseOrchestrator
    {

        /// <summary>
        /// Delete metadatas items from tracking tables
        /// </summary>
        /// <param name="timeStampStart">Timestamp start. Used to limit the delete metadatas rows from now to this timestamp</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="progress">Progress args</param>
        public virtual Task<DatabaseMetadatasCleaned> DeleteMetadatasAsync(long? timeStampStart, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.MetadataCleaning, async (ctx, connection, transaction) =>
        {
            if (!timeStampStart.HasValue)
                return null;

            // Create a dummy schema to be able to call the DeprovisionAsync method on the provider
            // No need columns or primary keys to be able to deprovision a table
            SyncSet schema = new SyncSet(this.Setup);

            var databaseMetadatasCleaned = await this.InternalDeleteMetadatasAsync(ctx, schema, this.Setup, timeStampStart.Value, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return databaseMetadatasCleaned;

        }, connection, transaction, cancellationToken);


        internal virtual async Task<DatabaseMetadatasCleaned> InternalDeleteMetadatasAsync(SyncContext context, SyncSet schema, SyncSetup setup, long timestampLimit,
                    DbConnection connection, DbTransaction transaction,
                    CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            await this.InterceptAsync(new MetadataCleaningArgs(context, this.Setup, timestampLimit, connection, transaction), cancellationToken).ConfigureAwait(false);

            DatabaseMetadatasCleaned databaseMetadatasCleaned = new DatabaseMetadatasCleaned { TimestampLimit = timestampLimit };

            foreach (var syncTable in schema.Tables)
            {
                // Create sync adapter
                var syncAdapter = this.GetSyncAdapter(syncTable, setup);

                var command = await syncAdapter.GetCommandAsync(DbCommandType.DeleteMetadata, connection, transaction);

                if (command != null)
                {

                    // Set the special parameters for delete metadata
                    DbSyncAdapter.SetParameterValue(command, "sync_row_timestamp", timestampLimit);

                    var rowsCleanedCount = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    // Check if we have a return value instead
                    var syncRowCountParam = DbSyncAdapter.GetParameter(command, "sync_row_count");

                    if (syncRowCountParam != null)
                        rowsCleanedCount = (int)syncRowCountParam.Value;

                    // Only add a new table metadata stats object, if we have, at least, purged 1 or more rows
                    if (rowsCleanedCount > 0)
                    {
                        var tableMetadatasCleaned = new TableMetadatasCleaned(syncTable.TableName, syncTable.SchemaName)
                        {
                            RowsCleanedCount = rowsCleanedCount,
                            TimestampLimit = timestampLimit
                        };

                        databaseMetadatasCleaned.Tables.Add(tableMetadatasCleaned);
                    }

                }
            }

            await this.InterceptAsync(new MetadataCleanedArgs(context, databaseMetadatasCleaned, connection), cancellationToken).ConfigureAwait(false);
            return databaseMetadatasCleaned;
        }



        /// <summary>
        /// Update a metadata row
        /// </summary>
        internal async Task<bool> InternalUpdateMetadatasAsync(SyncContext context, DbSyncAdapter syncAdapter, SyncRow row, Guid? senderScopeId, bool forceWrite, DbConnection connection, DbTransaction transaction)
        {
            var command = await syncAdapter.GetCommandAsync(DbCommandType.UpdateMetadata, connection, transaction);

            if (command == null) return false;

            // Set the parameters value from row
            syncAdapter.SetColumnParametersValues(command, row);

            // Set the special parameters for update
            syncAdapter.AddScopeParametersValues(command, senderScopeId, 0, row.RowState == DataRowState.Deleted, forceWrite);

            var metadataUpdatedRowsCount = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            // Check if we have a return value instead
            var syncRowCountParam = DbSyncAdapter.GetParameter(command, "sync_row_count");

            if (syncRowCountParam != null)
                metadataUpdatedRowsCount = (int)syncRowCountParam.Value;

            return metadataUpdatedRowsCount > 0;
        }


    }
}
