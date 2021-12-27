using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Orchestrators
{
    public partial class RemoteOrchestrator : BaseOrchestrator
    {

        /// <summary>
        /// Get the server scope histories
        /// </summary>
        public virtual async Task<List<ServerHistoryScopeInfo>> GetServerHistoryScopesAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeLoading, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                List<ServerHistoryScopeInfo> serverHistoryScopes = null;

                // Get Scope Builder
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Get scope if exists
                serverHistoryScopes = await InternalGetAllScopesAsync<ServerHistoryScopeInfo>(GetContext(), DbScopeType.ServerHistory, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return serverHistoryScopes;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Get the local configuration, ensures the local scope is created
        /// </summary>
        /// <returns>Server scope info, containing all scopes names, version, setup and related schema infos</returns>
        public virtual async Task<ServerScopeInfo> GetServerScopeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeLoading, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                ServerScopeInfo serverScopeInfo = null;

                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(GetContext(), DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // if serverscopeinfo is a new, because we never run any sync before, grab schema and affect setup
                if (serverScopeInfo.Setup is null && serverScopeInfo.Schema is null)
                {
                    // 1) Get Schema from remote provider
                    var schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    // 2) Provision
                    var provision = SyncProvision.TrackingTable | SyncProvision.StoredProcedures | SyncProvision.Triggers;
                    await InternalProvisionAsync(GetContext(), false, schema, Setup, provision, serverScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }
                else if (!serverScopeInfo.Setup.EqualsByProperties(Setup))
                {
                    // Compare serverscope setup with current
                    SyncSet schema;
                    // 1) Get Schema from remote provider
                    schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Migrate the old setup (serverScopeInfo.Setup) to the new setup (Setup) based on the new schema 
                    await InternalMigrationAsync(GetContext(), schema, serverScopeInfo.Setup, Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    serverScopeInfo.Setup = Setup;
                    serverScopeInfo.Schema = schema;

                    // Write scopes locally
                    await InternalSaveScopeAsync(GetContext(), DbScopeType.Server, serverScopeInfo, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
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
        /// Update or Insert a server scope row
        /// </summary>
        public virtual async Task<ServerScopeInfo> SaveServerScopeAsync(ServerScopeInfo scopeInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeWriting, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Write scopes locally
                var scopeInfoUpdated = await InternalSaveScopeAsync(GetContext(), DbScopeType.Server, scopeInfo, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return scopeInfoUpdated;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Update or Insert a server scope row
        /// </summary>
        public virtual async Task<ServerHistoryScopeInfo> SaveServerHistoryScopeAsync(ServerHistoryScopeInfo scopeInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeWriting, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.ServerHistory, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Write scopes locally
                var scopeInfoUpdated = await InternalSaveScopeAsync(GetContext(), DbScopeType.ServerHistory, scopeInfo, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return scopeInfoUpdated;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }
    }
}