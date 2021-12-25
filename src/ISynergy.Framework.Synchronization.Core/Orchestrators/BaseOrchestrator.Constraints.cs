using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Orchestrators
{
    public abstract partial class BaseOrchestrator
    {
        /// <summary>
        /// Reset a table, deleting rows from table and tracking_table
        /// </summary>
        public async Task<bool> ResetTableAsync(SetupTable table, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.None, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // using a fake SyncTable based on SetupTable, since we don't need columns
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);
                var syncAdapter = GetSyncAdapter(schemaTable, Setup);
                await InternalResetTableAsync(GetContext(), syncAdapter, runner.Connection, runner.Transaction).ConfigureAwait(false);
                await runner.CommitAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        /// <summary>
        /// Disabling constraints on one table
        /// </summary>
        public async Task<bool> DisableConstraintsAsync(SetupTable table, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.None, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // using a fake SyncTable based on SetupTable, since we don't need columns
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);
                var syncAdapter = GetSyncAdapter(schemaTable, Setup);
                await InternalDisableConstraintsAsync(GetContext(), syncAdapter, runner.Connection, runner.Transaction).ConfigureAwait(false);
                await runner.CommitAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        /// <summary>
        /// Enabling constraints on one table
        /// </summary>
        public async Task<bool> EnableConstraintsAsync(SetupTable table, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.None, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // using a fake SyncTable based on SetupTable, since we don't need columns
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);
                var syncAdapter = GetSyncAdapter(schemaTable, Setup);
                await InternalEnableConstraintsAsync(GetContext(), syncAdapter, runner.Connection, runner.Transaction).ConfigureAwait(false);
                await runner.CommitAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Disabling all constraints on synced tables
        /// </summary>
        internal async Task InternalDisableConstraintsAsync(SyncContext context, DbSyncAdapter syncAdapter, DbConnection connection, DbTransaction transaction = null)
        {
            var (command, _) = await syncAdapter.GetCommandAsync(DbCommandType.DisableConstraints, connection, transaction).ConfigureAwait(false);

            if (command is null) return;

            await InterceptAsync(new DbCommandArgs(context, command, connection, transaction)).ConfigureAwait(false);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Enabling all constraints on synced tables
        /// </summary>
        internal async Task InternalEnableConstraintsAsync(SyncContext context, DbSyncAdapter syncAdapter, DbConnection connection, DbTransaction transaction)
        {
            var (command, _) = await syncAdapter.GetCommandAsync(DbCommandType.EnableConstraints, connection, transaction).ConfigureAwait(false);

            if (command is null) return;
            await InterceptAsync(new DbCommandArgs(context, command, connection, transaction)).ConfigureAwait(false);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Reset a table, deleting rows from table and tracking_table
        /// </summary>
        internal async Task<bool> InternalResetTableAsync(SyncContext context, DbSyncAdapter syncAdapter, DbConnection connection, DbTransaction transaction)
        {
            var (command, _) = await syncAdapter.GetCommandAsync(DbCommandType.Reset, connection, transaction);

            if (command is not null)
            {
                await InterceptAsync(new DbCommandArgs(context, command, connection, transaction)).ConfigureAwait(false);
                var rowCount = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                return rowCount > 0;
            }

            return true;
        }

    }
}
