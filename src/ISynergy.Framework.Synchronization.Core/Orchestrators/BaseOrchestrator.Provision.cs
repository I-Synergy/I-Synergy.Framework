﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public abstract partial class BaseOrchestrator
    {


        internal async Task<SyncSet> InternalProvisionAsync(SyncContext ctx, bool overwrite, SyncSet schema, SyncSetup setup, SyncProvision provision, object scope, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            // If schema does not have any table, raise an exception
            if (schema == null || schema.Tables == null || !schema.HasTables)
                throw new MissingTablesException();

            await this.InterceptAsync(new ProvisioningArgs(ctx, provision, schema, connection, transaction), cancellationToken).ConfigureAwait(false);

            // get Database builder
            var builder = this.Provider.GetDatabaseBuilder();

            // Initialize database if needed
            await builder.EnsureDatabaseAsync(connection, transaction).ConfigureAwait(false);

            // Check if we have tables AND columns
            // If we don't have any columns it's most probably because user called method with the Setup only
            // So far we have only tables names, it's enough to get the schema
            if (schema.HasTables && !schema.HasColumns)
                schema = await this.InternalGetSchemaAsync(ctx, setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // Get Scope Builder
            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            // Shoudl we create scope
            // if scope is not null, so obviously we have create the table before, so no need to test
            if (provision.HasFlag(SyncProvision.ClientScope) && scope == null)
            {
                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (scope == null)
                    scope = await this.InternalGetScopeAsync<ScopeInfo>(ctx, DbScopeType.Client, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // if scope is not null, so obviously we have create the table before, so no need to test
            if (provision.HasFlag(SyncProvision.ServerScope) && scope == null)
            {
                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (scope == null)
                    scope = await this.InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, this.ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            if (provision.HasFlag(SyncProvision.ServerHistoryScope))
            {
                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await this.InternalCreateScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // Sorting tables based on dependencies between them
            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = this.GetTableBuilder(schemaTable, setup);

                // Check if we need to create a schema there
                var schemaExists = await InternalExistsSchemaAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!schemaExists)
                    await InternalCreateSchemaAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (provision.HasFlag(SyncProvision.Table))
                {
                    var tableExistst = await this.InternalExistsTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!tableExistst)
                        await this.InternalCreateTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                if (provision.HasFlag(SyncProvision.TrackingTable))
                {
                    var trackingTableExistst = await this.InternalExistsTrackingTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!trackingTableExistst)
                        await this.InternalCreateTrackingTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                if (provision.HasFlag(SyncProvision.Triggers))
                    await this.InternalCreateTriggersAsync(ctx, overwrite, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (provision.HasFlag(SyncProvision.StoredProcedures))
                    await this.InternalCreateStoredProceduresAsync(ctx, overwrite, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            }

            // save scope
            if (this is LocalOrchestrator && scope != null)
            {
                var clientScopeInfo = scope as ScopeInfo;
                clientScopeInfo.Schema = schema;
                clientScopeInfo.Setup = setup;

                await this.InternalSaveScopeAsync(ctx, DbScopeType.Client, clientScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }
            else if (this is RemoteOrchestrator && scope != null)
            {
                var serverScopeInfo = scope as ServerScopeInfo;
                serverScopeInfo.Schema = schema;
                serverScopeInfo.Setup = setup;

                await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            var args = new ProvisionedArgs(ctx, provision, schema, connection);
            await this.InterceptAsync(args, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args);

            return schema;
        }

        internal async Task<bool> InternalDeprovisionAsync(SyncContext ctx, SyncSet schema, SyncSetup setup, SyncProvision provision, object scope, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            await this.InterceptAsync(new DeprovisioningArgs(ctx, provision, schema, connection, transaction), cancellationToken).ConfigureAwait(false);

            // get Database builder
            var builder = this.Provider.GetDatabaseBuilder();

            // Sorting tables based on dependencies between them
            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            // Disable check constraints
            if (this.Options.DisableConstraintsOnApplyChanges)
                foreach (var table in schemaTables.Reverse())
                    await this.InternalDisableConstraintsAsync(ctx, this.GetSyncAdapter(table, setup), connection, transaction).ConfigureAwait(false);


            // Checking if we have to deprovision tables
            bool hasDeprovisionTableFlag = provision.HasFlag(SyncProvision.Table);

            // Firstly, removing the flag from the provision, because we need to drop everything in correct order, then drop tables in reverse side
            if (hasDeprovisionTableFlag)
                provision ^= SyncProvision.Table;

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = this.GetTableBuilder(schemaTable, setup);

                if (provision.HasFlag(SyncProvision.StoredProcedures))
                {
                    var allStoredProcedures = new List<DbStoredProcedureType>();

                    foreach (var spt in Enum.GetValues(typeof(DbStoredProcedureType)))
                        allStoredProcedures.Add((DbStoredProcedureType)spt);

                    allStoredProcedures.Reverse();

                    foreach (DbStoredProcedureType storedProcedureType in allStoredProcedures)
                    {
                        // if we are iterating on bulk, but provider do not support it, just loop through and continue
                        if ((storedProcedureType is DbStoredProcedureType.BulkTableType || storedProcedureType is DbStoredProcedureType.BulkUpdateRows || storedProcedureType is DbStoredProcedureType.BulkDeleteRows)
                            && !this.Provider.SupportBulkOperations)
                            continue;

                        var exists = await InternalExistsStoredProcedureAsync(ctx, tableBuilder, storedProcedureType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                        // Drop storedProcedure if already exists
                        if (exists)
                            await InternalDropStoredProcedureAsync(ctx, tableBuilder, storedProcedureType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    }

                    // Removing cached commands
                    var syncAdapter = this.GetSyncAdapter(schemaTable, setup);
                    syncAdapter.RemoveCommands();
                }

                if (provision.HasFlag(SyncProvision.Triggers))
                {
                    foreach (DbTriggerType triggerType in Enum.GetValues(typeof(DbTriggerType)))
                    {
                        var exists = await InternalExistsTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                        // Drop trigger if already exists
                        if (exists)
                            await InternalDropTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    }
                }

                if (provision.HasFlag(SyncProvision.TrackingTable))
                {
                    var exists = await InternalExistsTrackingTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        await this.InternalDropTrackingTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            // Eventually if we have the "Table" flag, then drop the table
            if (hasDeprovisionTableFlag)
            {
                foreach (var schemaTable in schemaTables.Reverse())
                {
                    var tableBuilder = this.GetTableBuilder(schemaTable, setup);

                    var exists = await InternalExistsTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        await this.InternalDropTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            // Get Scope Builder
            var scopeBuilder = this.GetScopeBuilder(this.Options.ScopeInfoTableName);

            bool hasDeleteClientScopeTable = false;
            bool hasDeleteServerScopeTable = false;
            if (provision.HasFlag(SyncProvision.ClientScope))
            {
                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                {
                    await this.InternalDropScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    hasDeleteClientScopeTable = true;
                }
            }

            if (provision.HasFlag(SyncProvision.ServerScope))
            {
                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                {
                    await this.InternalDropScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    hasDeleteServerScopeTable = true;
                }
            }

            if (provision.HasFlag(SyncProvision.ServerHistoryScope))
            {
                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    await this.InternalDropScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // save scope
            if (this is LocalOrchestrator && !hasDeleteClientScopeTable && scope != null)
            {
                var clientScopeInfo = scope as ScopeInfo;
                clientScopeInfo.Schema = null;
                clientScopeInfo.Setup = null;

                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    await this.InternalSaveScopeAsync(ctx, DbScopeType.Client, clientScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }
            else if (this is RemoteOrchestrator && !hasDeleteServerScopeTable && scope != null)
            {
                var serverScopeInfo = scope as ServerScopeInfo;
                serverScopeInfo.Schema = schema;
                serverScopeInfo.Setup = setup;

                var exists = await this.InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    await this.InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            var args = new DeprovisionedArgs(ctx, provision, schema, connection);
            await this.InterceptAsync(args, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args);

            return true;
        }

    }
}
