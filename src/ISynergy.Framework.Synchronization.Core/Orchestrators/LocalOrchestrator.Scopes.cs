using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Scopes;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;


namespace ISynergy.Framework.Synchronization.Core.Orchestrators
{
    public partial class LocalOrchestrator : BaseOrchestrator
    {

        public virtual async Task<ScopeInfo> GetClientScopeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeLoading, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Client, scopeBuilder,
                    runner.Connection, runner.Transaction, runner.CancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.Client, scopeBuilder,
                        runner.Connection, runner.Transaction, runner.CancellationToken, progress).ConfigureAwait(false);

                var localScope = await InternalGetScopeAsync<ScopeInfo>(GetContext(), DbScopeType.Client, ScopeName, scopeBuilder,
                    runner.Connection, runner.Transaction, runner.CancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return localScope;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Write a server scope 
        /// </summary> 
        public virtual async Task<ScopeInfo> SaveClientScopeAsync(ScopeInfo scopeInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.ScopeWriting, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Client, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(GetContext(), DbScopeType.Client, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Write scopes locally

                await InternalSaveScopeAsync(GetContext(), DbScopeType.Client, scopeInfo, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return scopeInfo;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

    }
}
