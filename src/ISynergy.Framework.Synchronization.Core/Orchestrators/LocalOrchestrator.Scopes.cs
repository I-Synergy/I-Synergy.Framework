using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Scopes;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;


namespace ISynergy.Framework.Synchronization.Core
{
    public partial class LocalOrchestrator : BaseOrchestrator
    {

        /// <summary> 
        /// Get the local configuration, ensures the local scope is created
        /// </summary>
        /// <returns>current context, the local scope info created or get from the database and the configuration from the client if changed </returns>
        public virtual Task<ScopeInfo> GetClientScopeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
            => RunInTransactionAsync(SyncStage.ScopeLoading, async (ctx, connection, transaction) =>
            {
                var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                var localScope = await this.InternalGetScopeAsync<ScopeInfo>(ctx, DbScopeType.Client, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                return localScope;

            }, connection, transaction, cancellationToken);

        /// <summary>
        /// Write a server scope 
        /// </summary> 
        public virtual Task<ScopeInfo> SaveClientScopeAsync(ScopeInfo scopeInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.ScopeWriting, async (ctx, connection, transaction) =>
        {
            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // Write scopes locally

            await this.InternalSaveScopeAsync(ctx, DbScopeType.Client, scopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return scopeInfo;

        }, connection, transaction, cancellationToken);

    }
}
