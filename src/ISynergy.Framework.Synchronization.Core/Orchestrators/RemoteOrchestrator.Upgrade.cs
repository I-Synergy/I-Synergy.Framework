using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Scopes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace ISynergy.Framework.Synchronization.Core
{
    public partial class RemoteOrchestrator
    {
        /// <summary>
        /// Upgrade the database structure to reach the last DMS version
        /// </summary>
        public virtual Task<bool> UpgradeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.Provisioning, async (ctx, connection, transaction) =>
        {
            if (this.Setup is null)
                return false;

            // get Database builder
            var dbBuilder = this.Provider.GetDatabaseBuilder();

            // Initialize database if needed
            await dbBuilder.EnsureDatabaseAsync(connection, transaction).ConfigureAwait(false);

            // Get schema
            var schema = await this.InternalGetSchemaAsync(ctx, this.Setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // If schema does not have any table, raise an exception
            if (schema is null || schema.Tables is null || !schema.HasTables)
                throw new MissingTablesException();

            var builder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var serverScopeInfos = await this.InternalGetAllScopesAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, builder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (serverScopeInfos is null || serverScopeInfos.Count <= 0)
                throw new MissingServerScopeInfoException();

            return await this.InternalUpgradeAsync(ctx, schema, serverScopeInfos, builder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

        }, connection, transaction, cancellationToken);


        /// <summary>
        /// Check if we need to upgrade the Database Structure
        /// </summary>
        public virtual Task<bool> NeedsToUpgradeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        => RunInTransactionAsync(SyncStage.Provisioning, async (ctx, connection, transaction) =>
        {
            // get Database builder
            var dbBuilder = this.Provider.GetDatabaseBuilder();

            // Initialize database if needed
            await dbBuilder.EnsureDatabaseAsync(connection, transaction).ConfigureAwait(false);

            var builder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, builder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (!exists)
                return false;

            var scopes = await this.InternalGetAllScopesAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, builder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            if (scopes is null || scopes.Count <= 0)
                return false;

            return InternalNeedsToUpgrade(ctx, scopes);

        }, connection, transaction, cancellationToken);

        internal virtual bool InternalNeedsToUpgrade(SyncContext context, List<ServerScopeInfo> serverScopeInfos)
        {
            var version = _versionService.ProductVersion;

            // get the smallest version of all scope in the scope info server tables
            foreach (var serverScopeInfo in serverScopeInfos)
            {
                var tmpVersion = new Version(serverScopeInfo.Version);

                if (tmpVersion < version)
                    version = tmpVersion;
            }

            return version < _versionService.ProductVersion;

        }


        internal virtual async Task<bool> InternalUpgradeAsync(SyncContext context, SyncSet schema, List<ServerScopeInfo> serverScopeInfos, DbScopeBuilder builder, DbConnection connection, DbTransaction transaction,
                        CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {

            var version = _versionService.ProductVersion;

            // get the smallest version of all scope in the scope info server tables
            foreach (var serverScopeInfo in serverScopeInfos)
            {
                var tmpVersion = new Version(serverScopeInfo.Version);

                if (tmpVersion < version)
                    version = tmpVersion;
            }

            // beta version
            if (version.Major == 0)
            {
                if (version.Minor <= 5)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 6, 0), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 6 && version.Build == 0)
                    version = await UpgdrateTo601Async(context, schema, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 6 && version.Build == 1)
                    version = await UpgdrateTo602Async(context, schema, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 6 && version.Build >= 2)
                    version = await UpgdrateTo700Async(context, schema, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 7 && version.Build == 0)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 7, 1), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 7 && version.Build == 1)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 7, 2), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 7 && version.Build == 2)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 7, 3), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 7 && version.Build >= 3)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 8, 0), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 8 && version.Build == 0)
                    version = await UpgdrateTo801Async(context, schema, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 8 && version.Build >= 1)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 9, 0), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (version.Minor == 9 && version.Build == 0)
                    version = await AutoUpgdrateToNewVersionAsync(context, new Version(0, 9, 1), connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            }

            foreach (var serverScopeInfo in serverScopeInfos)
            {
                var oldVersion = new Version(serverScopeInfo.Version);
                if (oldVersion != version)
                {
                    serverScopeInfo.Version = version.ToString();
                    await this.InternalSaveScopeAsync(context, DbScopeType.Server, serverScopeInfo, builder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            return version == _versionService.ProductVersion;

        }

        private async Task<Version> UpgdrateTo601Async(SyncContext context, SyncSet schema, DbConnection connection, DbTransaction transaction,
                        CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {

            var newVersion = new Version(0, 6, 1);
            // Sorting tables based on dependencies between them
            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            var message = $"Upgrade to {newVersion}:";
            var args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = this.GetTableBuilder(schemaTable, this.Setup);
                await InternalCreateStoredProceduresAsync(context, true, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                args = new UpgradeProgressArgs(context, $"ALL Stored procedures for table {tableBuilder.TableDescription.GetFullName()} updated", newVersion, connection, transaction);
                this.ReportProgress(context, progress, args, connection, transaction);
            }

            message = "Upgrade to 0.6.1 done.";
            args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);

            return newVersion;
        }


        private async Task<Version> UpgdrateTo602Async(SyncContext context, SyncSet schema, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var newVersion = new Version(0, 6, 2);

            var message = $"Upgrade to {newVersion}:";
            var args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);
            // Update the "Update trigger" for all tables


            // Sorting tables based on dependencies between them
            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = this.GetTableBuilder(schemaTable, this.Setup);

                // Upgrade Select Initial Changes
                var exists = await InternalExistsTriggerAsync(context, tableBuilder, DbTriggerType.Update, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                if (exists)
                    await InternalDropTriggerAsync(context, tableBuilder, DbTriggerType.Update, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                await InternalCreateTriggerAsync(context, tableBuilder, DbTriggerType.Update, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                args = new UpgradeProgressArgs(context, $"Update Trigger for table {tableBuilder.TableDescription.GetFullName()} updated", newVersion, connection, transaction);
                this.ReportProgress(context, progress, args, connection, transaction);

            }

            message = $"Upgrade to {newVersion} done.";
            args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);



            return newVersion;
        }

        private async Task<Version> UpgdrateTo700Async(SyncContext context, SyncSet schema, DbConnection connection, DbTransaction transaction,
                     CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {

            var newVersion = new Version(0, 7, 0);
            // Sorting tables based on dependencies between them

            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            var message = $"Upgrade to {newVersion}:";
            var args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = this.GetTableBuilder(schemaTable, this.Setup);

                // Upgrade Reset stored procedure
                var exists = await InternalExistsStoredProcedureAsync(context, tableBuilder,
                    DbStoredProcedureType.Reset, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                if (exists)
                    await InternalDropStoredProcedureAsync(context, tableBuilder, DbStoredProcedureType.Reset, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                await InternalCreateStoredProcedureAsync(context, tableBuilder, DbStoredProcedureType.Reset, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                args = new UpgradeProgressArgs(context, $"Reset stored procedure for table {tableBuilder.TableDescription.GetFullName()} updated", newVersion, connection, transaction);
                this.ReportProgress(context, progress, args, connection, transaction);

                // Upgrade Update stored procedure
                var existsUpdateSP = await InternalExistsStoredProcedureAsync(context, tableBuilder,
                    DbStoredProcedureType.UpdateRow, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                if (existsUpdateSP)
                    await InternalDropStoredProcedureAsync(context, tableBuilder, DbStoredProcedureType.UpdateRow, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                await InternalCreateStoredProcedureAsync(context, tableBuilder, DbStoredProcedureType.UpdateRow, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                args = new UpgradeProgressArgs(context, $"Update stored procedure for table {tableBuilder.TableDescription.GetFullName()} updated", newVersion, connection, transaction);
                this.ReportProgress(context, progress, args, connection, transaction);

                // Upgrade Bulk Update stored procedure
                if (this.Provider.SupportBulkOperations)
                {
                    var existsBulkUpdateSP = await InternalExistsStoredProcedureAsync(context, tableBuilder,
                        DbStoredProcedureType.BulkUpdateRows, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    if (existsBulkUpdateSP)
                        await InternalDropStoredProcedureAsync(context, tableBuilder, DbStoredProcedureType.BulkUpdateRows, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    await InternalCreateStoredProcedureAsync(context, tableBuilder, DbStoredProcedureType.BulkUpdateRows, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    args = new UpgradeProgressArgs(context, $"Bulk Update stored procedure for table {tableBuilder.TableDescription.GetFullName()} updated", newVersion, connection, transaction);
                    this.ReportProgress(context, progress, args, connection, transaction);
                }

            }

            return newVersion;
        }


        private async Task<Version> UpgdrateTo801Async(SyncContext context, SyncSet schema, DbConnection connection, DbTransaction transaction,
               CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {

            var newVersion = new Version(0, 8, 1);
            // Sorting tables based on dependencies between them

            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            var message = $"Upgrade to {newVersion}:";
            var args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = this.GetTableBuilder(schemaTable, this.Setup);
                await InternalCreateStoredProceduresAsync(context, true, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                args = new UpgradeProgressArgs(context, $"ALL Stored procedures for table {tableBuilder.TableDescription.GetFullName()} updated", newVersion, connection, transaction);
                this.ReportProgress(context, progress, args, connection, transaction);
            }

            return newVersion;
        }



        private Task<Version> AutoUpgdrateToNewVersionAsync(SyncContext context, Version newVersion, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var message = $"Upgrade to {newVersion}:";
            var args = new UpgradeProgressArgs(context, message, newVersion, connection, transaction);
            this.ReportProgress(context, progress, args, connection, transaction);

            return Task.FromResult(newVersion);
        }
    }
}
