﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
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
        public RemoteOrchestrator(CoreProvider provider, SyncOptions options, SyncSetup setup, string scopeName = SyncOptions.DefaultScopeName)
           : base(provider, options, setup, scopeName)
        {
            if (!this.Provider.CanBeServerProvider)
                throw new UnsupportedServerProviderException(this.Provider.GetProviderTypeName());
        }

        /// <summary>
        /// Ensure the schema is readed from the server, based on the Setup instance.
        /// Creates all required tables (server_scope tables) and provision all tables (tracking, stored proc, triggers and so on...)
        /// Then return the schema readed
        /// </summary>
        /// <returns>current context, the local scope info created or get from the database and the configuration from the client if changed </returns>
        internal virtual Task<ServerScopeInfo> EnsureSchemaAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.SchemaReading, async (ctx, connection, transaction) =>
        {
            // starting with scope loading
            ctx.SyncStage = SyncStage.ScopeLoading;

            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            var serverScopeInfo = await this.InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            SyncSet schema;
            // Let's compare this serverScopeInfo with the current Setup
            // If schema is null :
            // - Read the schema from database based on this.Setup
            // - Provision the database with this schema
            // - Write the scope with this.Setup and schema
            // If schema is not null :
            // - Compare saved setup with current setup
            // - If not equals:
            // - Read schema from database based on this.Setup
            if (serverScopeInfo.Schema == null)
            {
                // So far, we don't have already a database provisionned
                ctx.SyncStage = SyncStage.Provisioning;

                // 1) Get Schema from remote provider
                schema = await this.InternalGetSchemaAsync(ctx, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // 2) Provision
                var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;
                schema = await InternalProvisionAsync(ctx, false, schema, this.Setup, provision, serverScopeInfo, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }
            else
            {
                // Setup stored on local or remote is different from the one provided.
                // So, we can migrate
                if (!serverScopeInfo.Setup.EqualsByProperties(this.Setup))
                {
                    // 1) Get Schema from remote provider
                    schema = await this.InternalGetSchemaAsync(ctx, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Migrate the old setup (serverScopeInfo.Setup) to the new setup (this.Setup) based on the new schema 
                    await this.InternalMigrationAsync(ctx, schema, serverScopeInfo.Setup, this.Setup, connection, transaction, cancellationToken, progress);

                    serverScopeInfo.Setup = this.Setup;
                    serverScopeInfo.Schema = schema;

                    // Write scopes locally
                    await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                // Get the schema saved on server
                schema = serverScopeInfo.Schema;
            }
            return serverScopeInfo;

        }, connection, transaction, cancellationToken);


        /// <summary>
        /// Migrate an old setup configuration to a new one. This method is usefull if you are changing your SyncSetup when a database has been already configured previously
        /// </summary>
        public virtual Task<ServerScopeInfo> MigrationAsync(SyncSetup oldSetup, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.Migrating, async (ctx, connection, transaction) =>
        {
            SyncSet schema;

            // Get Schema from remote provider
            schema = await this.InternalGetSchemaAsync(ctx, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // Migrate the db structure
            await this.InternalMigrationAsync(ctx, schema, oldSetup, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            var remoteScope = await this.InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            remoteScope.Setup = this.Setup;
            remoteScope.Schema = schema;

            // Write scopes locally
            await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, remoteScope, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return remoteScope;

        }, connection, transaction, cancellationToken);


        /// <summary>
        /// Apply changes on remote provider
        /// </summary>
        internal virtual async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, ConflictResolutionPolicy ServerPolicy, DatabaseChangesApplied ClientChangesApplied, DatabaseChangesSelected ServerChangesSelected)>
            ApplyThenGetChangesAsync(ScopeInfo clientScope, BatchInfo clientBatchInfo,
            CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            // Get context or create a new one
            var ctx = this.GetContext();

            long remoteClientTimestamp = 0L;
            DatabaseChangesSelected serverChangesSelected = null;
            DatabaseChangesApplied clientChangesApplied = null;
            BatchInfo serverBatchInfo = null;
            SyncSet schema = null;

            using var connection = this.Provider.CreateConnection();

            try
            {
                ctx.SyncStage = SyncStage.ChangesApplying;

                //Direction set to Upload
                ctx.SyncWay = SyncWay.Upload;

                // Open connection
                await this.OpenConnectionAsync(connection, cancellationToken).ConfigureAwait(false);

                DbTransaction transaction;

                // Create two transactions
                // First one to commit changes
                // Second one to get changes now that everything is commited
                using (transaction = connection.BeginTransaction(this.Provider.IsolationLevel))
                {
                    await this.InterceptAsync(new TransactionOpenedArgs(ctx, connection, transaction), cancellationToken).ConfigureAwait(false);

                    // Maybe here, get the schema from server, issue from client scope name
                    // Maybe then compare the schema version from client scope with schema version issued from server
                    // Maybe if different, raise an error ?
                    // Get scope if exists

                    // Getting server scope assumes we have already created the schema on server

                    var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

                    var serverScopeInfo = await this.InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, clientScope.Name, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Should we ?
                    if (serverScopeInfo.Schema == null)
                        throw new MissingRemoteOrchestratorSchemaException();

                    // Deserialiaze schema
                    schema = serverScopeInfo.Schema;

                    // Create message containing everything we need to apply on server side
                    var applyChanges = new MessageApplyChanges(Guid.Empty, clientScope.Id, false, clientScope.LastServerSyncTimestamp, schema, this.Setup, this.Options.ConflictResolutionPolicy,
                                    this.Options.DisableConstraintsOnApplyChanges, this.Options.UseBulkOperations, this.Options.CleanMetadatas, this.Options.CleanFolder, false, clientBatchInfo, this.Options.SerializerFactory);

                    // Call provider to apply changes
                    (ctx, clientChangesApplied) = await this.InternalApplyChangesAsync(ctx, applyChanges, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    await this.InterceptAsync(new TransactionCommitArgs(ctx, connection, transaction), cancellationToken).ConfigureAwait(false);

                    // commit first transaction
                    transaction.Commit();
                }

                ctx.SyncStage = SyncStage.ChangesSelecting;
                ctx.ProgressPercentage = 0.55;


                using (transaction = connection.BeginTransaction(this.Provider.IsolationLevel))
                {
                    await this.InterceptAsync(new TransactionOpenedArgs(ctx, connection, transaction), cancellationToken).ConfigureAwait(false);

                    //Direction set to Download
                    ctx.SyncWay = SyncWay.Download;

                    // JUST Before get changes, get the timestamp, to be sure to 
                    // get rows inserted / updated elsewhere since the sync is not over
                    remoteClientTimestamp = await this.InternalGetLocalTimestampAsync(ctx, connection, transaction, cancellationToken, progress);

                    // Get if we need to get all rows from the datasource
                    var fromScratch = clientScope.IsNewScope || ctx.SyncType == SyncType.Reinitialize || ctx.SyncType == SyncType.ReinitializeWithUpload;

                    var message = new MessageGetChangesBatch(clientScope.Id, Guid.Empty, fromScratch, clientScope.LastServerSyncTimestamp, schema, this.Setup, this.Options.BatchSize, this.Options.BatchDirectory, this.Options.SerializerFactory);

                    // Call interceptor
                    await this.InterceptAsync(new DatabaseChangesSelectingArgs(ctx, message, connection, transaction), cancellationToken).ConfigureAwait(false);

                    // When we get the chnages from server, we create the batches if it's requested by the client
                    // the batch decision comes from batchsize from client
                    (ctx, serverBatchInfo, serverChangesSelected) =
                        await this.InternalGetChangesAsync(ctx, message, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    // generate the new scope item
                    this.CompleteTime = DateTime.UtcNow;

                    var scopeHistory = new ServerHistoryScopeInfo
                    {
                        Id = clientScope.Id,
                        Name = clientScope.Name,
                        LastSyncTimestamp = remoteClientTimestamp,
                        LastSync = this.CompleteTime,
                        LastSyncDuration = this.CompleteTime.Value.Subtract(this.StartTime.Value).Ticks,
                    };

                    // Write scopes locally
                    var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

                    await this.InternalSaveScopeAsync(ctx, DbScopeType.ServerHistory, scopeHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Commit second transaction for getting changes
                    await this.InterceptAsync(new TransactionCommitArgs(ctx, connection, transaction), cancellationToken).ConfigureAwait(false);

                    transaction.Commit();
                }

                // Event progress & interceptor
                await this.CloseConnectionAsync(connection, cancellationToken).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
            finally
            {
                await this.CloseConnectionAsync(connection, cancellationToken).ConfigureAwait(false);
            }
            return (remoteClientTimestamp, serverBatchInfo, this.Options.ConflictResolutionPolicy, clientChangesApplied, serverChangesSelected);

        }

        /// <summary>
        /// Get changes from remote database
        /// </summary>
        public virtual Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected ServerChangesSelected)> GetChangesAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.ChangesSelecting, async (ctx, connection, transaction) =>
        {
            // Output
            long remoteClientTimestamp = 0L;
            BatchInfo serverBatchInfo = null;
            DatabaseChangesSelected serverChangesSelected = null;

            if (!string.Equals(clientScope.Name, this.ScopeName, SyncGlobalization.DataSourceStringComparison))
                throw new InvalidScopeInfoException();

            // Before getting changes, be sure we have a remote schema available
            var serverScope = await this.EnsureSchemaAsync(connection, transaction, cancellationToken, progress);

            // Should we ?
            if (serverScope.Schema == null)
                throw new MissingRemoteOrchestratorSchemaException();


            //Direction set to Download
            ctx.SyncWay = SyncWay.Download;

            // JUST Before get changes, get the timestamp, to be sure to 
            // get rows inserted / updated elsewhere since the sync is not over
            remoteClientTimestamp = await this.InternalGetLocalTimestampAsync(ctx, connection, transaction, cancellationToken, progress);

            // Get if we need to get all rows from the datasource
            var fromScratch = clientScope.IsNewScope || ctx.SyncType == SyncType.Reinitialize || ctx.SyncType == SyncType.ReinitializeWithUpload;

            var message = new MessageGetChangesBatch(clientScope.Id, Guid.Empty, fromScratch, clientScope.LastServerSyncTimestamp, serverScope.Schema, this.Setup, this.Options.BatchSize, this.Options.BatchDirectory, this.Options.SerializerFactory);

            // When we get the chnages from server, we create the batches if it's requested by the client
            // the batch decision comes from batchsize from client
            (ctx, serverBatchInfo, serverChangesSelected) =
                await this.InternalGetChangesAsync(ctx, message, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return (remoteClientTimestamp, serverBatchInfo, serverChangesSelected);
        }, connection, transaction, cancellationToken);

        /// <summary>
        /// Get estimated changes from remote database to be applied on client
        /// </summary>
        public virtual Task<(long RemoteClientTimestamp, DatabaseChangesSelected ServerChangesSelected)> GetEstimatedChangesCountAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
          => RunInTransactionAsync(SyncStage.ChangesSelecting, async (ctx, connection, transaction) =>
          {
              // Output
              long remoteClientTimestamp = 0L;
              DatabaseChangesSelected serverChangesSelected = null;

              if (!string.Equals(clientScope.Name, this.ScopeName, SyncGlobalization.DataSourceStringComparison))
                  throw new InvalidScopeInfoException();

              var serverScope = await this.EnsureSchemaAsync(connection, transaction, cancellationToken, progress);

              // Should we ?
              if (serverScope.Schema == null)
                  throw new MissingRemoteOrchestratorSchemaException();

              //Direction set to Download
              ctx.SyncWay = SyncWay.Download;

              // JUST Before get changes, get the timestamp, to be sure to 
              // get rows inserted / updated elsewhere since the sync is not over
              remoteClientTimestamp = await this.InternalGetLocalTimestampAsync(ctx, connection, transaction, cancellationToken, progress);

              // Get if we need to get all rows from the datasource
              var fromScratch = clientScope.IsNewScope || ctx.SyncType == SyncType.Reinitialize || ctx.SyncType == SyncType.ReinitializeWithUpload;

              // it's an estimation, so force In Memory (BatchSize == 0)
              var message = new MessageGetChangesBatch(clientScope.Id, Guid.Empty, fromScratch, clientScope.LastServerSyncTimestamp, serverScope.Schema, this.Setup, 0, this.Options.BatchDirectory, this.Options.SerializerFactory);

              // When we get the chnages from server, we create the batches if it's requested by the client
              // the batch decision comes from batchsize from client
              (ctx, serverChangesSelected) =
                  await this.InternalGetEstimatedChangesCountAsync(ctx, message, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              return (remoteClientTimestamp, serverChangesSelected);

          }, connection, transaction, cancellationToken);

        /// <summary>
        /// Check if we can reach the underlying provider database
        /// </summary>
        public Task<(string DatabaseName, string Version)> GetHelloAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
          => RunInTransactionAsync(SyncStage.None, async (ctx, connection, transaction) =>
          {
              string databaseName = null;
              string version = null;

              (ctx, databaseName, version) = await this.GetHelloAsync(ctx, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              return (databaseName, version);

          }, connection, transaction, cancellationToken);

        /// <summary>
        /// Delete all metadatas from tracking tables, based on min timestamp from history client table
        /// </summary>
        public Task<DatabaseMetadatasCleaned> DeleteMetadatasAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
            => RunInTransactionAsync(SyncStage.None, async (ctx, connection, transaction) =>
            {
                // Get the min timestamp, where we can without any problem, delete metadatas
                var histories = await this.GetServerHistoryScopesAsync(connection, transaction, cancellationToken, progress);

                if (histories == null || histories.Count == 0)
                    return new DatabaseMetadatasCleaned();

                var minTimestamp = histories.Min(shsi => shsi.LastSyncTimestamp);

                if (minTimestamp == 0)
                    return new DatabaseMetadatasCleaned();

                return await this.DeleteMetadatasAsync(minTimestamp, connection, transaction, cancellationToken, progress);
            }, connection, transaction, cancellationToken);

        /// <summary>
        /// Delete metadatas items from tracking tables
        /// </summary>
        /// <param name="timeStampStart">Timestamp start. Used to limit the delete metadatas rows from now to this timestamp</param>
        public override Task<DatabaseMetadatasCleaned> DeleteMetadatasAsync(long? timeStampStart, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
          => RunInTransactionAsync(SyncStage.MetadataCleaning, async (ctx, connection, transaction) =>
          {
              if (!timeStampStart.HasValue)
                  return null;

              await this.InterceptAsync(new MetadataCleaningArgs(ctx, this.Setup, timeStampStart.Value, connection, transaction), cancellationToken).ConfigureAwait(false);

              // Create a dummy schema to be able to call the DeprovisionAsync method on the provider
              // No need columns or primary keys to be able to deprovision a table
              SyncSet schema = new SyncSet(this.Setup);

              var databaseMetadatasCleaned = await this.InternalDeleteMetadatasAsync(ctx, schema, this.Setup, timeStampStart.Value, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              // Update server scope table
              var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

              var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              if (!exists)
                  await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              var serverScopeInfo = await this.InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              serverScopeInfo.LastCleanupTimestamp = databaseMetadatasCleaned.TimestampLimit;

              await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

              await this.InterceptAsync(new MetadataCleanedArgs(ctx, databaseMetadatasCleaned, connection), cancellationToken).ConfigureAwait(false);

              return databaseMetadatasCleaned;

          }, connection, transaction, cancellationToken);

    }
}