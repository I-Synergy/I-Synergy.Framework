﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Scopes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public partial class RemoteOrchestrator : BaseOrchestrator
    {

        /// <summary>
        /// Get the server scope histories
        /// </summary>
        public virtual Task<List<ServerHistoryScopeInfo>> GetServerHistoryScopesAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
           => RunInTransactionAsync(SyncStage.ScopeLoading, async (ctx, connection, transaction) =>
           {
               List<ServerHistoryScopeInfo> serverHistoryScopes = null;

               // Get Scope Builder
               var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

               var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

               if (!exists)
                   await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

               // Get scope if exists
               serverHistoryScopes = await this.InternalGetAllScopesAsync<ServerHistoryScopeInfo>(ctx, DbScopeType.ServerHistory, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

               return serverHistoryScopes;

           }, connection, transaction, cancellationToken);


        /// <summary>
        /// Get the local configuration, ensures the local scope is created
        /// </summary>
        /// <returns>Server scope info, containing all scopes names, version, setup and related schema infos</returns>
        public virtual Task<ServerScopeInfo> GetServerScopeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.ScopeLoading, async (ctx, connection, transaction) =>
        {
            ServerScopeInfo serverScopeInfo = null;

            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            serverScopeInfo = await this.InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // if serverscopeinfo is a new, because we never run any sync before, grab schema and affect setup
            if (serverScopeInfo.Setup == null && serverScopeInfo.Schema == null)
            {
                // 1) Get Schema from remote provider
                var schema = await this.InternalGetSchemaAsync(ctx, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // 2) Provision
                var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;
                schema = await InternalProvisionAsync(ctx, false, schema, this.Setup, provision, serverScopeInfo, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                return serverScopeInfo;
            }

            // Compare serverscope setup with current
            if (!serverScopeInfo.Setup.EqualsByProperties(this.Setup))
            {
                SyncSet schema;
                // 1) Get Schema from remote provider
                schema = await this.InternalGetSchemaAsync(ctx, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // Migrate the old setup (serverScopeInfo.Setup) to the new setup (this.Setup) based on the new schema 
                await this.InternalMigrationAsync(ctx, schema, serverScopeInfo.Setup, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                serverScopeInfo.Setup = this.Setup;
                serverScopeInfo.Schema = schema;

                // Write scopes locally
                await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            return serverScopeInfo;

        }, connection, transaction, cancellationToken);



        /// <summary>
        /// Update or Insert a server scope row
        /// </summary>
        public virtual Task<ServerScopeInfo> SaveServerScopeAsync(ServerScopeInfo scopeInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.ScopeWriting, async (ctx, connection, transaction) =>
        {
            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // Write scopes locally
            var scopeInfoUpdated = await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, scopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return scopeInfoUpdated;

        }, connection, transaction, cancellationToken);

        /// <summary>
        /// Update or Insert a server scope row
        /// </summary>
        public virtual Task<ServerHistoryScopeInfo> SaveServerHistoryScopeAsync(ServerHistoryScopeInfo scopeInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.ScopeWriting, async (ctx, connection, transaction) =>
        {
            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // Write scopes locally
            var scopeInfoUpdated = await this.InternalSaveScopeAsync(ctx, DbScopeType.ServerHistory, scopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return scopeInfoUpdated;

        }, connection, transaction, cancellationToken);

    }
}