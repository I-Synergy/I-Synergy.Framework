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
    public partial class RemoteOrchestrator : BaseOrchestrator
    {

        /// <summary>
        /// Provision the remote database 
        /// </summary>
        /// <param name="overwrite">Overwrite existing objects</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual Task<SyncSet> ProvisionAsync(bool overwrite = false, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            var provision = SyncProvision.ServerScope | SyncProvision.ServerHistoryScope |
                            SyncProvision.StoredProcedures | SyncProvision.Triggers | SyncProvision.TrackingTable;

            return ProvisionAsync(provision, overwrite, null, connection, transaction, cancellationToken, progress);
        }

        /// <summary>
        /// Provision the remote database based on the Setup parameter, and the provision enumeration
        /// </summary>
        /// <param name="provision">Provision enumeration to determine which components to apply</param>
        /// <param name="overwrite"></param>
        /// <param name="serverScopeInfo">server scope. Will be saved once provision is done</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns>Full schema with table and columns properties</returns>
        public virtual async Task<SyncSet> ProvisionAsync(SyncProvision provision, bool overwrite = false, ServerScopeInfo serverScopeInfo = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Provisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // Check incompatibility with the flags
                if (provision.HasFlag(SyncProvision.ClientScope))
                    throw new InvalidProvisionForRemoteOrchestratorException();

                // Get server scope if not supplied
                if (serverScopeInfo is null)
                {
                    var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                    var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                    if (exists)
                        serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(GetContext(), DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                var schema = new SyncSet(Setup);
                schema = await InternalProvisionAsync(GetContext(), overwrite, schema, Setup, provision, serverScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);
                return schema;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Deprovision the remote database 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual Task<bool> DeprovisionAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            var provision = SyncProvision.ServerScope | SyncProvision.ServerHistoryScope |
                            SyncProvision.StoredProcedures | SyncProvision.Triggers | SyncProvision.TrackingTable;

            return DeprovisionAsync(provision, null, connection, transaction, cancellationToken, progress);
        }

        /// <summary>
        /// Deprovision the orchestrator database based on the schema argument, and the provision enumeration
        /// </summary>
        /// <param name="provision">Provision enumeration to determine which components to deprovision</param>
        /// <param name="serverScopeInfo"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public virtual async Task<bool> DeprovisionAsync(SyncProvision provision, ServerScopeInfo serverScopeInfo = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Deprovisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // Get server scope if not supplied
                if (serverScopeInfo is null)
                {
                    var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

                    var exists = await InternalExistsScopeInfoTableAsync(GetContext(), DbScopeType.Server, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        serverScopeInfo = await InternalGetScopeAsync<ServerScopeInfo>(GetContext(), DbScopeType.Server, ScopeName, scopeBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                // Create a temporary SyncSet for attaching to the schemaTable
                var tmpSchema = new SyncSet();

                // Add this table to schema
                foreach (var table in Setup.Tables)
                    tmpSchema.Tables.Add(new SyncTable(table.TableName, table.SchemaName));

                tmpSchema.EnsureSchema();

                // copy filters from old setup
                foreach (var filter in Setup.Filters)
                    tmpSchema.Filters.Add(filter);

                var isDeprovisioned = await InternalDeprovisionAsync(GetContext(), tmpSchema, Setup, provision, serverScopeInfo, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

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
