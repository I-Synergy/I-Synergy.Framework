using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Parameter;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Set;
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


namespace ISynergy.Framework.Synchronization.Core.Orchestrators
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
                if ((_syncContext.Parameters is null || _syncContext.Parameters.Count <= 0) && syncParameters is not null && syncParameters.Count > 0)
                    _syncContext.Parameters = syncParameters;

                // 1) Get Schema from remote provider
                var schema = await InternalGetSchemaAsync(_syncContext, Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // 2) Ensure databases are ready
                //    Even if we are using only stored procedures, we need tracking tables and triggers
                //    for tracking the rows inserted / updated after the snapshot
                var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;

                // 3) Provision everything
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(_syncContext, DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(_syncContext, DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(_syncContext, DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                schema = await InternalProvisionAsync(_syncContext, false, schema, Setup, provision, serverScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // 4) Getting the most accurate timestamp
                var remoteClientTimestamp = await InternalGetLocalTimestampAsync(_syncContext, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // 5) Create the snapshot with
                localSerializerFactory = localSerializerFactory is null ? new LocalJsonSerializerFactory() : localSerializerFactory;

                var batchInfo = await InternalCreateSnapshotAsync(GetContext(), schema, Setup, localSerializerFactory, remoteClientTimestamp,
                    runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return batchInfo;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        internal virtual async Task<BatchInfo> InternalCreateSnapshotAsync(SyncContext context, SyncSet schema, SyncSetup setup,
              ILocalSerializerFactory localSerializerFactory, long remoteClientTimestamp, DbConnection connection, DbTransaction transaction,
              CancellationToken cancellationToken, IProgress<ProgressArgs> progress = null)
        {
            await InterceptAsync(new SnapshotCreatingArgs(GetContext(), schema, Options.SnapshotsDirectory, Options.BatchSize, remoteClientTimestamp, Provider.CreateConnection(), null), progress, cancellationToken).ConfigureAwait(false);

            if (!Directory.Exists(Options.SnapshotsDirectory))
                Directory.CreateDirectory(Options.SnapshotsDirectory);

            var (rootDirectory, nameDirectory) = await InternalGetSnapshotDirectoryAsync(context, cancellationToken, progress).ConfigureAwait(false);

            // create local directory with scope inside
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            // Delete directory if already exists
            var directoryFullPath = Path.Combine(rootDirectory, nameDirectory);

            if (Directory.Exists(directoryFullPath))
                Directory.Delete(directoryFullPath, true);

            var message = new MessageGetChangesBatch(Guid.Empty, Guid.Empty, true, null, schema, Setup, Options.BatchSize,
                rootDirectory, nameDirectory, Provider.SupportsMultipleActiveResultSets, Options.LocalSerializerFactory);

            BatchInfo serverBatchInfo;
            DatabaseChangesSelected serverChangesSelected;

            (context, serverBatchInfo, serverChangesSelected) =
                    await InternalGetChangesAsync(context, message, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // since we explicitely defined remote client timestamp to null, to get all rows, just reaffect here
            serverBatchInfo.Timestamp = remoteClientTimestamp;

            // Serialize on disk.
            var jsonConverter = new Serialization.JsonConverter<BatchInfo>();

            var summaryFileName = Path.Combine(directoryFullPath, "summary.json");

            using (var f = new FileStream(summaryFileName, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                var bytes = await jsonConverter.SerializeAsync(serverBatchInfo).ConfigureAwait(false);
                f.Write(bytes, 0, bytes.Length);
            }

            await InterceptAsync(new SnapshotCreatedArgs(GetContext(), serverBatchInfo, Provider.CreateConnection(), null), progress, cancellationToken).ConfigureAwait(false);

            return serverBatchInfo;
        }
    }
}
