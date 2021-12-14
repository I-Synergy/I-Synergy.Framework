using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public partial class RemoteOrchestrator : BaseOrchestrator
    {

        /// <summary>
        /// Gets the sync side of this Orchestrator. RemoteOrchestrator is always used on server side
        /// </summary>
        public override SyncSide Side => SyncSide.ServerSide;


        /// <summary>
        /// Create a remote orchestrator, used to orchestrates the whole sync on the server side
        /// </summary>
        public RemoteOrchestrator(
            IVersionService versionService,
            CoreProvider provider, 
            SyncOptions options, 
            SyncSetup setup, 
            string scopeName = SyncOptions.DefaultScopeName)
           : base(versionService, provider, options, setup, scopeName)
        {
            if (!Provider.CanBeServerProvider)
                throw new UnsupportedServerProviderException(Provider.GetProviderTypeName());
        }

        /// <summary>
        /// Ensure the schema is readed from the server, based on the Setup instance.
        /// Creates all required tables (server_scope tables) and provision all tables (tracking, stored proc, triggers and so on...)
        /// Then return the schema readed
        /// </summary>
        /// <returns>current context, the local scope info created or get from the database and the configuration from the client if changed </returns>
        internal virtual async Task<ServerScopeInfo> EnsureSchemaAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeLoading, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(GetContext(), DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                SyncSet schema;
                // Let's compare this serverScopeInfo with the current Setup
                // If schema is null :
                // - Read the schema from database based on Setup
                // - Provision the database with this schema
                // - Write the scope with Setup and schema
                // If schema is not null :
                // - Compare saved setup with current setup
                // - If not equals:
                // - Read schema from database based on Setup
                if (serverScopeInfo.Schema is null)
                {
                    // So far, we don't have already a database provisionned
                    GetContext().SyncStage = SyncStage.Provisioning;

                    // 1) Get Schema from remote provider
                    schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    // 2) Provision
                    var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;
                    schema = await InternalProvisionAsync(GetContext(), false, schema, Setup, provision, serverScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }
                else
                {
                    // Setup stored on local or remote is different from the one provided.
                    // So, we can migrate
                    if (!serverScopeInfo.Setup.EqualsByProperties(Setup))
                    {
                        // 1) Get Schema from remote provider
                        schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                        // Migrate the old setup (serverScopeInfo.Setup) to the new setup (Setup) based on the new schema 
                        await InternalMigrationAsync(GetContext(), schema, serverScopeInfo.Setup, Setup, runner.Connection, runner.Transaction, cancellationToken, progress);

                        serverScopeInfo.Setup = Setup;
                        serverScopeInfo.Schema = schema;

                        // Write scopes locally
                        await InternalSaveScopeAsync(GetContext(), DbScopeType.Server, serverScopeInfo, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                    }

                    // Get the schema saved on server
                    schema = serverScopeInfo.Schema;
                }

                await runner.CommitAsync().ConfigureAwait(false);

                return serverScopeInfo;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }


        /// <summary>
        /// Migrate an old setup configuration to a new one. This method is usefull if you are changing your SyncSetup when a database has been already configured previously
        /// </summary>
        public virtual async Task<ServerScopeInfo> MigrationAsync(SyncSetup oldSetup, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Migrating, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                SyncSet schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Migrate the db structure
                await InternalMigrationAsync(GetContext(), schema, oldSetup, Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var remoteScope = await InternalGetScopeAsync<ServerScopeInfo>(GetContext(), DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                remoteScope.Setup = Setup;
                remoteScope.Schema = schema;

                // Write scopes locally
                await InternalSaveScopeAsync(GetContext(), DbScopeType.Server, remoteScope, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return remoteScope;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }


        /// <summary>
        /// Apply changes on remote provider
        /// </summary>
        internal virtual async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, ConflictResolutionPolicy ServerPolicy, DatabaseChangesApplied ClientChangesApplied, DatabaseChangesSelected ServerChangesSelected)>
            ApplyThenGetChangesAsync(ScopeInfo clientScope, BatchInfo clientBatchInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                if (!StartTime.HasValue)
                    StartTime = DateTime.UtcNow;

                // Get context or create a new one
                var ctx = GetContext();

                long remoteClientTimestamp = 0L;
                DatabaseChangesSelected serverChangesSelected = null;
                DatabaseChangesApplied clientChangesApplied = null;
                BatchInfo serverBatchInfo = null;
                SyncSet schema = null;


                //Direction set to Upload
                ctx.SyncWay = SyncWay.Upload;

                // Create two transactions
                // First one to commit changes
                // Second one to get changes now that everything is commited
                await using (var runner = await this.GetConnectionAsync(SyncStage.ChangesApplying, connection, transaction, cancellationToken, progress).ConfigureAwait(false))
                {
                    // Maybe here, get the schema from server, issue from client scope name
                    // Maybe then compare the schema version from client scope with schema version issued from server
                    // Maybe if different, raise an error ?
                    // Get scope if exists

                    // Getting server scope assumes we have already created the schema on server
                    var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                    var serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, clientScope.Name, scopeBuilder,
                                            runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Should we ?
                    if (serverScopeInfo.Schema is null)
                        throw new MissingRemoteOrchestratorSchemaException();

                    // Deserialiaze schema
                    schema = serverScopeInfo.Schema;

                    // Create message containing everything we need to apply on server side
                    var applyChanges = new MessageApplyChanges(Guid.Empty, clientScope.Id, false, clientScope.LastServerSyncTimestamp, schema, Setup, Options.ConflictResolutionPolicy,
                                    Options.DisableConstraintsOnApplyChanges, Options.CleanMetadatas, Options.CleanFolder, false, clientBatchInfo, Options.LocalSerializerFactory);

                    // Call provider to apply changes
                    (ctx, clientChangesApplied) = await InternalApplyChangesAsync(ctx, applyChanges, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    await InterceptAsync(new TransactionCommitArgs(ctx, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                    // commit first transaction
                    await runner.CommitAsync().ConfigureAwait(false);
                }

                await using (var runner = await this.GetConnectionAsync(SyncStage.ChangesSelecting, connection, transaction, cancellationToken, progress).ConfigureAwait(false))
                {
                    ctx.ProgressPercentage = 0.55;

                    //Direction set to Download
                    ctx.SyncWay = SyncWay.Download;

                    // JUST Before get changes, get the timestamp, to be sure to 
                    // get rows inserted / updated elsewhere since the sync is not over
                    remoteClientTimestamp = await InternalGetLocalTimestampAsync(ctx, runner.Connection, runner.Transaction, cancellationToken, progress);

                    // Get if we need to get all rows from the datasource
                    var fromScratch = clientScope.IsNewScope || ctx.SyncType == SyncType.Reinitialize || ctx.SyncType == SyncType.ReinitializeWithUpload;

                    var message = new MessageGetChangesBatch(clientScope.Id, Guid.Empty, fromScratch, clientScope.LastServerSyncTimestamp, schema, Setup, Options.BatchSize, Options.BatchDirectory, Provider.SupportsMultipleActiveResultSets, Options.LocalSerializerFactory);

                    // When we get the chnages from server, we create the batches if it's requested by the client
                    // the batch decision comes from batchsize from client
                    (ctx, serverBatchInfo, serverChangesSelected) =
                        await InternalGetChangesAsync(ctx, message, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    // generate the new scope item
                    CompleteTime = DateTime.UtcNow;

                    var scopeHistory = new ServerHistoryScopeInfo
                    {
                        Id = clientScope.Id,
                        Name = clientScope.Name,
                        LastSyncTimestamp = remoteClientTimestamp,
                        LastSync = CompleteTime,
                        LastSyncDuration = CompleteTime.Value.Subtract(StartTime.Value).Ticks,
                    };

                    // Write scopes locally
                    var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                    await InternalSaveScopeAsync(ctx, DbScopeType.ServerHistory, scopeHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Commit second transaction for getting changes
                    await InterceptAsync(new TransactionCommitArgs(ctx, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                    await runner.CommitAsync().ConfigureAwait(false); ;
                }
                return (remoteClientTimestamp, serverBatchInfo, Options.ConflictResolutionPolicy, clientChangesApplied, serverChangesSelected);
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Get changes from remote database
        /// </summary>
        public virtual async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected ServerChangesSelected)>
            GetChangesAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                // Get context or create a new one
                var ctx = GetContext();

                if (!string.Equals(clientScope.Name, ScopeName, SyncGlobalization.DataSourceStringComparison))
                    throw new InvalidScopeInfoException();

                // Before getting changes, be sure we have a remote schema available
                var serverScope = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress);

                // Should we ?
                if (serverScope.Schema is null)
                    throw new MissingRemoteOrchestratorSchemaException();

                await using var runner = await this.GetConnectionAsync(SyncStage.ChangesSelecting, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                //Direction set to Download
                ctx.SyncWay = SyncWay.Download;

                // Output
                // JUST Before get changes, get the timestamp, to be sure to 
                // get rows inserted / updated elsewhere since the sync is not over
                var remoteClientTimestamp = await InternalGetLocalTimestampAsync(ctx, runner.Connection, runner.Transaction, cancellationToken, progress);

                // Get if we need to get all rows from the datasource
                var fromScratch = clientScope.IsNewScope || ctx.SyncType == SyncType.Reinitialize || ctx.SyncType == SyncType.ReinitializeWithUpload;

                var message = new MessageGetChangesBatch(clientScope.Id, Guid.Empty, fromScratch, clientScope.LastServerSyncTimestamp,
                    serverScope.Schema, Setup, Options.BatchSize, Options.BatchDirectory, Provider.SupportsMultipleActiveResultSets, Options.LocalSerializerFactory);

                BatchInfo serverBatchInfo;
                DatabaseChangesSelected serverChangesSelected;
                // When we get the chnages from server, we create the batches if it's requested by the client
                // the batch decision comes from batchsize from client
                (ctx, serverBatchInfo, serverChangesSelected) =
                    await InternalGetChangesAsync(ctx, message, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return (remoteClientTimestamp, serverBatchInfo, serverChangesSelected);
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        /// <summary>
        /// Get estimated changes from remote database to be applied on client
        /// </summary>
        public virtual async Task<(long RemoteClientTimestamp, DatabaseChangesSelected ServerChangesSelected)>
                    GetEstimatedChangesCountAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {

                if (!string.Equals(clientScope.Name, ScopeName, SyncGlobalization.DataSourceStringComparison))
                    throw new InvalidScopeInfoException();

                var serverScope = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress);

                // Should we ?
                if (serverScope.Schema is null)
                    throw new MissingRemoteOrchestratorSchemaException();

                await using var runner = await this.GetConnectionAsync(SyncStage.ChangesSelecting, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var ctx = GetContext();
                ctx.SyncStage = SyncStage.ChangesSelecting;
                //Direction set to Download
                ctx.SyncWay = SyncWay.Download;

                // Output
                // JUST Before get changes, get the timestamp, to be sure to 
                // get rows inserted / updated elsewhere since the sync is not over
                var remoteClientTimestamp = await InternalGetLocalTimestampAsync(ctx, runner.Connection, runner.Transaction, cancellationToken, progress);

                // Get if we need to get all rows from the datasource
                var fromScratch = clientScope.IsNewScope || ctx.SyncType == SyncType.Reinitialize || ctx.SyncType == SyncType.ReinitializeWithUpload;

                // it's an estimation, so force In Memory (BatchSize == 0)
                var message = new MessageGetChangesBatch(clientScope.Id, Guid.Empty, fromScratch, clientScope.LastServerSyncTimestamp, serverScope.Schema,
                    Setup, 0, Options.BatchDirectory, Provider.SupportsMultipleActiveResultSets, Options.LocalSerializerFactory);

                DatabaseChangesSelected serverChangesSelected;
                // When we get the chnages from server, we create the batches if it's requested by the client
                // the batch decision comes from batchsize from client
                (ctx, serverChangesSelected) =
                    await InternalGetEstimatedChangesCountAsync(ctx, message, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                return (remoteClientTimestamp, serverChangesSelected);
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Delete all metadatas from tracking tables, based on min timestamp from history client table
        /// </summary>
        public async Task<DatabaseMetadatasCleaned> DeleteMetadatasAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Get the min timestamp, where we can without any problem, delete metadatas
            var histories = await GetServerHistoryScopesAsync(connection, transaction, cancellationToken, progress);

            if (histories is null || histories.Count == 0)
                return new DatabaseMetadatasCleaned();

            var minTimestamp = histories.Min(shsi => shsi.LastSyncTimestamp);

            if (minTimestamp == 0)
                return new DatabaseMetadatasCleaned();

            return await DeleteMetadatasAsync(minTimestamp, connection, transaction, cancellationToken, progress);
        }

        /// <summary>
        /// Delete metadatas items from tracking tables
        /// </summary>
        /// <param name="timeStampStart">Timestamp start. Used to limit the delete metadatas rows from now to this timestamp</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public override async Task<DatabaseMetadatasCleaned> DeleteMetadatasAsync(long? timeStampStart, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                if (!timeStampStart.HasValue)
                    return null;

                await using var runner = await this.GetConnectionAsync(SyncStage.MetadataCleaning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var ctx = GetContext();

                await InterceptAsync(new MetadataCleaningArgs(ctx, Setup, timeStampStart.Value, runner.Connection, runner.Transaction), progress, cancellationToken).ConfigureAwait(false);

                // Create a dummy schema to be able to call the DeprovisionAsync method on the provider
                // No need columns or primary keys to be able to deprovision a table
                SyncSet schema = new SyncSet(Setup);

                var databaseMetadatasCleaned = await InternalDeleteMetadatasAsync(ctx, schema, Setup, timeStampStart.Value, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Update server scope table
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                serverScopeInfo.LastCleanupTimestamp = databaseMetadatasCleaned.TimestampLimit;

                await InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await InterceptAsync(new MetadataCleanedArgs(ctx, databaseMetadatasCleaned, runner.Connection), progress, cancellationToken).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return databaseMetadatasCleaned;

            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

    }
}