using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Parameters;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public partial class RemoteOrchestrator : BaseOrchestrator
    {

        /// <summary>
        /// Get a snapshot
        /// </summary>
        public virtual async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected DatabaseChangesSelected)>
            GetSnapshotAsync(SyncSet schema = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            try
            {
                // Get context or create a new one
                var ctx = GetContext();
                var changesSelected = new DatabaseChangesSelected();

                BatchInfo serverBatchInfo = null;
                if (string.IsNullOrEmpty(Options.SnapshotsDirectory))
                    return (0, null, changesSelected);

                //Direction set to Download
                ctx.SyncWay = SyncWay.Download;

                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                // Get Schema from remote provider if no schema passed from args
                if (schema is null)
                {
                    var serverScopeInfo = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    schema = serverScopeInfo.Schema;
                }

                // When we get the changes from server, we create the batches if it's requested by the client
                // the batch decision comes from batchsize from client
                var (rootDirectory, nameDirectory) = await InternalGetSnapshotDirectoryAsync(ctx, cancellationToken, progress).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(rootDirectory))
                {
                    var directoryFullPath = Path.Combine(rootDirectory, nameDirectory);

                    // if no snapshot present, just return null value.
                    if (Directory.Exists(directoryFullPath))
                    {
                        // Serialize on disk.
                        var jsonConverter = new Serialization.JsonConverter<BatchInfo>();

                        var summaryFileName = Path.Combine(directoryFullPath, "summary.json");

                        using (var fs = new FileStream(summaryFileName, FileMode.Open, FileAccess.Read))
                        {
                            serverBatchInfo = await jsonConverter.DeserializeAsync(fs).ConfigureAwait(false);
                        }

                        // Create the schema changeset
                        var changesSet = new SyncSet();

                        // Create a Schema set without readonly columns, attached to memory changes
                        foreach (var table in schema.Tables)
                        {
                            DbSyncAdapter.CreateChangesTable(schema.Tables[table.TableName, table.SchemaName], changesSet);

                            // Get all stats about this table
                            var bptis = serverBatchInfo.BatchPartsInfo.SelectMany(bpi => bpi.Tables.Where(t =>
                            {
                                var sc = SyncGlobalization.DataSourceStringComparison;

                                var sn = t.SchemaName is null ? string.Empty : t.SchemaName;
                                var otherSn = table.SchemaName is null ? string.Empty : table.SchemaName;

                                return table.TableName.Equals(t.TableName, sc) && sn.Equals(otherSn, sc);

                            }));

                            if (bptis is not null)
                            {
                                // Statistics
                                var tableChangesSelected = new TableChangesSelected(table.TableName, table.SchemaName)
                                {
                                    // we are applying a snapshot where it can't have any deletes, obviously
                                    Upserts = bptis.Sum(bpti => bpti.RowsCount)
                                };

                                if (tableChangesSelected.Upserts > 0)
                                    changesSelected.TableChangesSelected.Add(tableChangesSelected);
                            }


                        }
                        serverBatchInfo.SanitizedSchema = changesSet;
                    }
                }
                if (serverBatchInfo is null)
                    return (0, null, changesSelected);

                return (serverBatchInfo.Timestamp, serverBatchInfo, changesSelected);
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }



        /// <summary>
        /// Create a snapshot, based on the Setup object. 
        /// </summary>
        /// <param name="syncParameters">if not parameters are found in the SyncContext instance, will use thes sync parameters instead</param>
        /// <param name="localSerializerFactory"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns>Instance containing all information regarding the snapshot</returns>
        public virtual async Task<BatchInfo> CreateSnapshotAsync(SyncParameters syncParameters = null,
            ILocalSerializerFactory localSerializerFactory = default, DbConnection connection = default, DbTransaction transaction = default,
            CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.SnapshotCreating, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                if (string.IsNullOrEmpty(Options.SnapshotsDirectory) || Options.BatchSize <= 0)
                    throw new SnapshotMissingMandatariesOptionsException();

                // check parameters
                // If context has no parameters specified, and user specifies a parameter collection we switch them
                if ((syncContext.Parameters is null || syncContext.Parameters.Count <= 0) && syncParameters is not null && syncParameters.Count > 0)
                    syncContext.Parameters = syncParameters;

                // 1) Get Schema from remote provider
                var schema = await InternalGetSchemaAsync(syncContext, Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // 2) Ensure databases are ready
                //    Even if we are using only stored procedures, we need tracking tables and triggers
                //    for tracking the rows inserted / updated after the snapshot
                var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

                // 3) Provision everything
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(syncContext, DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(syncContext, DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(syncContext, DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                schema = await InternalProvisionAsync(syncContext, false, schema, Setup, provision, serverScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // 4) Getting the most accurate timestamp
                var remoteClientTimestamp = await InternalGetLocalTimestampAsync(syncContext, runner.Connection, runner.Transaction, cancellationToken, progress);


                await runner.CommitAsync().ConfigureAwait(false);

                // 5) Create the snapshot with
                localSerializerFactory = localSerializerFactory is null ? new LocalJsonSerializerFactory() : localSerializerFactory;

                var batchInfo = await InternalCreateSnapshotAsync(GetContext(), schema, Setup, localSerializerFactory, remoteClientTimestamp, cancellationToken, progress).ConfigureAwait(false);

                return batchInfo;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        internal virtual async Task<BatchInfo> InternalCreateSnapshotAsync(SyncContext context, SyncSet schema, SyncSetup setup,
                ILocalSerializerFactory localSerializerFactory, long remoteClientTimestamp,
                CancellationToken cancellationToken, IProgress<ProgressArgs> progress = null)
        {

            await InterceptAsync(new SnapshotCreatingArgs(GetContext(), schema, Options.SnapshotsDirectory, Options.BatchSize, remoteClientTimestamp, Provider.CreateConnection(), null), progress, cancellationToken).ConfigureAwait(false);

            //// Call interceptor
            //await InterceptAsync(new DatabaseChangesSelectingArgs(context, null, null, null), progress, cancellationToken).ConfigureAwait(false);

            // create local directory
            if (!Directory.Exists(Options.SnapshotsDirectory))
                Directory.CreateDirectory(Options.SnapshotsDirectory);

            var (rootDirectory, nameDirectory) = await InternalGetSnapshotDirectoryAsync(context, cancellationToken, progress).ConfigureAwait(false);

            if (string.IsNullOrEmpty(rootDirectory))
                return null;

            // create local directory with scope inside
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            // Delete directory if already exists
            var directoryFullPath = Path.Combine(rootDirectory, nameDirectory);

            if (Directory.Exists(directoryFullPath))
                Directory.Delete(directoryFullPath, true);

            Directory.CreateDirectory(directoryFullPath);

            // Create stats object to store changes count
            var changes = new DatabaseChangesSelected();
            var batchInfo = new BatchInfo(schema, rootDirectory, nameDirectory);

            var schemaTables = schema.Tables.SortByDependencies(tab => tab.GetRelations().Select(r => r.GetParentTable()));

            var lstAllBatchPartInfos = new ConcurrentBag<BatchPartInfo>();

            await schemaTables.ForEachAsync(async table =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                try
                {
                    var serializer = localSerializerFactory.GetLocalSerializer();

                    //list of batchpart for that synctable
                    var batchPartInfos = new List<BatchPartInfo>();

                    var batchIndex = 0;
                    // Get Select initialize changes command
                    await using var runner = await this.GetConnectionAsync(SyncStage.SnapshotCreating, null, null, cancellationToken, progress).ConfigureAwait(false);
                    var selectIncrementalChangesCommand = await GetSelectChangesCommandAsync(context, table, setup, true, runner.Connection, runner.Transaction).ConfigureAwait(false);

                    if (selectIncrementalChangesCommand is null)
                        return;

                    // generate a new path
                    var (fullPath, fileName) = batchInfo.GetNewBatchPartInfoPath(table, batchIndex, serializer.Extension);

                    var schemaChangesTable = DbSyncAdapter.CreateChangesTable(table);

                    // open the file and write table header
                    await serializer.OpenFileAsync(fullPath, schemaChangesTable).ConfigureAwait(false);

                    // Statistics
                    var tableChangesSelected = new TableChangesSelected(table.TableName, table.SchemaName);

                    var rowsCountInBatch = 0;
                    // We are going to batch select, if needed by the provider

                    // Set parameters
                    SetSelectChangesCommonParameters(context, table, null, true, null, selectIncrementalChangesCommand);

                    // launch interceptor if any
                    var args = await InterceptAsync(new TableChangesSelectingArgs(context, table, selectIncrementalChangesCommand, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                    if (!args.Cancel && args.Command is not null)
                    {
                        await InterceptAsync(new DbCommandArgs(context, args.Command, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                        // Get the reader
                        using var dataReader = await selectIncrementalChangesCommand.ExecuteReaderAsync().ConfigureAwait(false);

                        while (await dataReader.ReadAsync())
                        {
                            // Create a row from dataReader
                            var syncRow = CreateSyncRowFromReader2(dataReader, schemaChangesTable);
                            rowsCountInBatch++;

                            // Set the correct state to be applied
                            if (syncRow.RowState == DataRowState.Deleted)
                                tableChangesSelected.Deletes++;
                            else
                                tableChangesSelected.Upserts++;

                            await serializer.WriteRowToFileAsync(syncRow, schemaChangesTable).ConfigureAwait(false);

                            var currentBatchSize = await serializer.GetCurrentFileSizeAsync().ConfigureAwait(false);

                            // Next line if we don't reach the batch size yet.
                            if (currentBatchSize <= Options.BatchSize)
                                continue;

                            var bpi = new BatchPartInfo { FileName = fileName };

                            // Create the info on the batch part
                            BatchPartTableInfo tableInfo = new BatchPartTableInfo
                            {
                                TableName = tableChangesSelected.TableName,
                                SchemaName = tableChangesSelected.SchemaName,
                                RowsCount = rowsCountInBatch

                            };

                            bpi.Tables = new BatchPartTableInfo[] { tableInfo };
                            bpi.RowsCount = rowsCountInBatch;
                            bpi.IsLastBatch = false;
                            bpi.Index = batchIndex;
                            batchPartInfos.Add(bpi);

                            // Add to all bpi concurrent bag
                            lstAllBatchPartInfos.Add(bpi);

                            Debug.WriteLine($"Added BPI for table {tableChangesSelected.TableName}. lstAllBatchPartInfos.Count:{lstAllBatchPartInfos.Count}");

                            // Close file
                            await serializer.CloseFileAsync(fullPath, schemaChangesTable).ConfigureAwait(false);

                            // increment batch index
                            batchIndex++;
                            // Reinit rowscount in batch
                            rowsCountInBatch = 0;

                            // generate a new path
                            (fullPath, fileName) = batchInfo.GetNewBatchPartInfoPath(schemaChangesTable, batchIndex, serializer.Extension);

                            // open a new file and write table header
                            await serializer.OpenFileAsync(fullPath, schemaChangesTable).ConfigureAwait(false);

                            // Raise progress
                            await InterceptAsync(new TableChangesSelectedArgs(context, null, tableChangesSelected, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                        }

                        dataReader.Close();

                    }

                    var bpi2 = new BatchPartInfo { FileName = fileName };

                    // Create the info on the batch part
                    BatchPartTableInfo tableInfo2 = new BatchPartTableInfo
                    {
                        TableName = tableChangesSelected.TableName,
                        SchemaName = tableChangesSelected.SchemaName,
                        RowsCount = rowsCountInBatch
                    };
                    bpi2.Tables = new BatchPartTableInfo[] { tableInfo2 };
                    bpi2.RowsCount = rowsCountInBatch;
                    bpi2.IsLastBatch = true;
                    bpi2.Index = batchIndex;
                    batchPartInfos.Add(bpi2);

                    // Add to all bpi concurrent bag
                    lstAllBatchPartInfos.Add(bpi2);

                    Debug.WriteLine($"Added last BPI for table {tableChangesSelected.TableName}. lstAllBatchPartInfos.Count:{lstAllBatchPartInfos.Count}");

                    batchIndex++;

                    // Close file
                    await serializer.CloseFileAsync(fullPath, schemaChangesTable).ConfigureAwait(false);

                    // Raise progress
                    var tableChangesSelectedArgs = await InterceptAsync(new TableChangesSelectedArgs(context, batchPartInfos, tableChangesSelected, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                    changes.TableChangesSelected.Add(tableChangesSelected);

                }
                catch (Exception ex)
                {
                    throw GetSyncError(ex);
                }
            });

            // Check the last index as the last batch
            batchInfo.EnsureLastBatch();
            batchInfo.Timestamp = remoteClientTimestamp;

            // delete all empty batchparts (empty tables)
            foreach (var bpi in lstAllBatchPartInfos)
            {
                if (bpi.RowsCount <= 0)
                {
                    var fullPathToDelete = Path.Combine(directoryFullPath, bpi.FileName);
                    File.Delete(fullPathToDelete);
                }
            }

            // Generate a good index order to be compliant with previous versions
            var tmpLstBatchPartInfos = new List<BatchPartInfo>();
            foreach (var table in schemaTables)
            {
                // get all bpi where count > 0 and ordered by index
                foreach (var bpi in lstAllBatchPartInfos.Where(bpi => bpi.RowsCount > 0 && bpi.Tables[0].EqualsByName(new BatchPartTableInfo(table.TableName, table.SchemaName))).OrderBy(bpi => bpi.Index).ToArray())
                {
                    batchInfo.BatchPartsInfo.Add(bpi);
                    batchInfo.RowsCount += bpi.RowsCount;

                    tmpLstBatchPartInfos.Add(bpi);
                }
            }

            var newBatchIndex = 0;
            foreach (var bpi in tmpLstBatchPartInfos)
            {
                bpi.Index = newBatchIndex;
                newBatchIndex++;
                bpi.IsLastBatch = newBatchIndex == tmpLstBatchPartInfos.Count;
            }

            // Serialize on disk.
            var jsonConverter = new Serialization.JsonConverter<BatchInfo>();

            var summaryFileName = Path.Combine(directoryFullPath, "summary.json");

            using (var f = new FileStream(summaryFileName, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                var bytes = await jsonConverter.SerializeAsync(batchInfo).ConfigureAwait(false);
                f.Write(bytes, 0, bytes.Length);
            }

            // Raise database changes selected
            //await InterceptAsync(new DatabaseChangesSelectedArgs(context, remoteClientTimestamp, batchInfo, changes), progress, cancellationToken).ConfigureAwait(false);

            await InterceptAsync(new SnapshotCreatedArgs(GetContext(), batchInfo, Provider.CreateConnection(), null), progress, cancellationToken).ConfigureAwait(false);

            return batchInfo;
        }
    }
}
