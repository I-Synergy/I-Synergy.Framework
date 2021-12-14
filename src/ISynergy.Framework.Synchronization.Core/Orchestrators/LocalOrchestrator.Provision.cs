using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
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
        /// Provision the local database based on the orchestrator setup, and the provision enumeration
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="overwrite">Overwrite existing objects</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual Task<SyncSet> ProvisionAsync(SyncSet schema = null, bool overwrite = false, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            var provision = SyncProvision.ClientScope | SyncProvision.Table |
                            SyncProvision.StoredProcedures | SyncProvision.Triggers | SyncProvision.TrackingTable;

            if (schema is null)
                schema = new SyncSet(Setup);

            return ProvisionAsync(schema, provision, overwrite, null, connection, transaction, cancellationToken, progress);
        }

        /// <summary>
        /// Provision the local database based on the orchestrator setup, and the provision enumeration
        /// </summary>
        /// <param name="provision">Provision enumeration to determine which components to apply</param>
        /// <param name="overwrite"></param>
        /// <param name="clientScopeInfo"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual Task<SyncSet> ProvisionAsync(SyncProvision provision, bool overwrite = false, ScopeInfo clientScopeInfo = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
            => ProvisionAsync(new SyncSet(Setup), provision, overwrite, clientScopeInfo, connection, transaction, cancellationToken, progress);


        /// <summary>
        /// Provision the local database based on the schema parameter, and the provision enumeration
        /// </summary>
        /// <param name="schema">Schema to be applied to the database managed by the orchestrator, through the provider.</param>
        /// <param name="provision">Provision enumeration to determine which components to apply</param>
        /// <param name="overwrite"></param>
        /// <param name="clientScopeInfo">client scope. Will be saved once provision is done</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns>Full schema with table and columns properties</returns>
        public virtual async Task<SyncSet> ProvisionAsync(SyncSet schema, SyncProvision provision, bool overwrite = false, ScopeInfo clientScopeInfo = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Provisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // Check incompatibility with the flags
                if (provision.HasFlag(SyncProvision.ServerHistoryScope) || provision.HasFlag(SyncProvision.ServerScope))
                    throw new InvalidProvisionForLocalOrchestratorException();

                // Get server scope if not supplied
                if (clientScopeInfo is null)
                {
                    var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                    var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Client, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        clientScopeInfo = await InternalGetScopeAsync<ScopeInfo>(GetContext(), DbScopeType.Client, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                schema = await InternalProvisionAsync(GetContext(), overwrite, schema, Setup, provision, clientScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return schema;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }


        /// <summary>
        /// Deprovision the local database
        /// </summary>
        public virtual Task DeprovisionAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            var provision = SyncProvision.ClientScope | SyncProvision.StoredProcedures | SyncProvision.Triggers | SyncProvision.TrackingTable;

            return DeprovisionAsync(provision, null, connection, transaction, cancellationToken, progress);
        }


        /// <summary>
        /// Deprovision the orchestrator database based on the provision enumeration
        /// </summary>
        /// <param name="provision">Provision enumeration to determine which components to deprovision</param>
        /// <param name="clientScopeInfo"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual async Task<bool> DeprovisionAsync(SyncProvision provision, ScopeInfo clientScopeInfo = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Create a temporary SyncSet for attaching to the schemaTable
            var tmpSchema = new SyncSet();

            // Add this table to schema
            foreach (var table in Setup.Tables)
                tmpSchema.Tables.Add(new SyncTable(table.TableName, table.SchemaName));

            tmpSchema.EnsureSchema();

            // copy filters from old setup
            foreach (var filter in Setup.Filters)
                tmpSchema.Filters.Add(filter);

            var isDeprovisioned = await DeprovisionAsync(tmpSchema, provision, clientScopeInfo, connection, transaction, cancellationToken, progress);

            return isDeprovisioned;

        }

        /// <summary>
        /// Deprovision the orchestrator database based on the schema argument, and the provision enumeration
        /// </summary>
        /// <param name="schema">Schema to be deprovisioned from the database managed by the orchestrator, through the provider.</param>
        /// <param name="provision">Provision enumeration to determine which components to deprovision</param>
        /// <param name="clientScopeInfo"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual async Task<bool> DeprovisionAsync(SyncSet schema, SyncProvision provision, ScopeInfo clientScopeInfo = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Deprovisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // Get server scope if not supplied
                if (clientScopeInfo is null)
                {
                    var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                    var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Client, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        clientScopeInfo = await InternalGetScopeAsync<ScopeInfo>(GetContext(), DbScopeType.Client, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                var isDeprovisioned = await InternalDeprovisionAsync(GetContext(), schema, Setup, provision, clientScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return isDeprovisioned;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

    }
}
