using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
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
            if (schema is null || schema.Tables is null || !schema.HasTables)
                throw new MissingTablesException();

            await InterceptAsync(new ProvisioningArgs(ctx, provision, schema, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            // get Database builder
            var builder = Provider.GetDatabaseBuilder();

            // Initialize database if needed
            await builder.EnsureDatabaseAsync(connection, transaction).ConfigureAwait(false);

            // Check if we have tables AND columns
            // If we don't have any columns it's most probably because user called method with the Setup only
            // So far we have only tables names, it's enough to get the schema
            if (schema.HasTables && !schema.HasColumns)
                schema = await InternalGetSchemaAsync(ctx, setup, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            // Get Scope Builder
            var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

            // Shoudl we create scope
            // if scope is not null, so obviously we have create the table before, so no need to test
            if (provision.HasFlag(SyncProvision.ClientScope) && scope is null)
            {
                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (scope is null)
                    scope = await InternalGetScopeAsync<ScopeInfo>(ctx, DbScopeType.Client, ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // if scope is not null, so obviously we have create the table before, so no need to test
            if (provision.HasFlag(SyncProvision.ServerScope) && scope is null)
            {
                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (scope is null)
                    scope = await InternalGetScopeAsync<ServerScopeInfo>(ctx, DbScopeType.Server, ScopeName, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            if (provision.HasFlag(SyncProvision.ServerHistoryScope))
            {
                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    await InternalCreateScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // Sorting tables based on dependencies between them
            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = GetTableBuilder(schemaTable, setup);

                // Check if we need to create a schema there
                var schemaExists = await InternalExistsSchemaAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!schemaExists)
                    await InternalCreateSchemaAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (provision.HasFlag(SyncProvision.Table))
                {
                    var tableExistst = await InternalExistsTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!tableExistst)
                        await InternalCreateTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                if (provision.HasFlag(SyncProvision.TrackingTable))
                {
                    var trackingTableExistst = await InternalExistsTrackingTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!trackingTableExistst)
                        await InternalCreateTrackingTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                if (provision.HasFlag(SyncProvision.Triggers))
                    await InternalCreateTriggersAsync(ctx, overwrite, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (provision.HasFlag(SyncProvision.StoredProcedures))
                    await InternalCreateStoredProceduresAsync(ctx, overwrite, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            }

            // save scope
            if (this is LocalOrchestrator && scope is not null)
            {
                var clientScopeInfo = scope as ScopeInfo;
                clientScopeInfo.Schema = schema;
                clientScopeInfo.Setup = setup;

                await InternalSaveScopeAsync(ctx, DbScopeType.Client, clientScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }
            else if (this is RemoteOrchestrator && scope is not null)
            {
                var serverScopeInfo = scope as ServerScopeInfo;
                serverScopeInfo.Schema = schema;
                serverScopeInfo.Setup = setup;

                await InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            var args = new ProvisionedArgs(ctx, provision, schema, connection);
            await InterceptAsync(args, progress, cancellationToken).ConfigureAwait(false);

            return schema;
        }

        internal async Task<bool> InternalDeprovisionAsync(SyncContext ctx, SyncSet schema, SyncSetup setup, SyncProvision provision, object scope, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            await InterceptAsync(new DeprovisioningArgs(ctx, provision, schema, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            // get Database builder
            var builder = Provider.GetDatabaseBuilder();

            // Sorting tables based on dependencies between them
            var schemaTables = schema.Tables
                .SortByDependencies(tab => tab.GetRelations()
                    .Select(r => r.GetParentTable()));

            // Disable check constraints
            if (Options.DisableConstraintsOnApplyChanges)
                foreach (var table in schemaTables.Reverse())
                    await InternalDisableConstraintsAsync(ctx, GetSyncAdapter(table, setup), connection, transaction).ConfigureAwait(false);


            // Checking if we have to deprovision tables
            bool hasDeprovisionTableFlag = provision.HasFlag(SyncProvision.Table);

            // Firstly, removing the flag from the provision, because we need to drop everything in correct order, then drop tables in reverse side
            if (hasDeprovisionTableFlag)
                provision ^= SyncProvision.Table;

            foreach (var schemaTable in schemaTables)
            {
                var tableBuilder = GetTableBuilder(schemaTable, setup);

                if (provision.HasFlag(SyncProvision.StoredProcedures))
                {
                    await InternalDropStoredProceduresAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    // Removing cached commands
                    var syncAdapter = GetSyncAdapter(schemaTable, setup);
                    syncAdapter.RemoveCommands();
                }

                if (provision.HasFlag(SyncProvision.Triggers))
                {
                    await InternalDropTriggersAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                if (provision.HasFlag(SyncProvision.TrackingTable))
                {
                    var exists = await InternalExistsTrackingTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        await InternalDropTrackingTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            // Eventually if we have the "Table" flag, then drop the table
            if (hasDeprovisionTableFlag)
            {
                foreach (var schemaTable in schemaTables.Reverse())
                {
                    var tableBuilder = GetTableBuilder(schemaTable, setup);

                    var exists = await InternalExistsTableAsync(ctx, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        await InternalDropTableAsync(ctx, setup, tableBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            // Get Scope Builder
            var scopeBuilder = GetScopeBuilder(Options.ScopeInfoTableName);

            bool hasDeleteClientScopeTable = false;
            bool hasDeleteServerScopeTable = false;
            if (provision.HasFlag(SyncProvision.ClientScope))
            {
                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                {
                    await InternalDropScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    hasDeleteClientScopeTable = true;
                }
            }

            if (provision.HasFlag(SyncProvision.ServerScope))
            {
                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                {
                    await InternalDropScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                    hasDeleteServerScopeTable = true;
                }
            }

            if (provision.HasFlag(SyncProvision.ServerHistoryScope))
            {
                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    await InternalDropScopeInfoTableAsync(ctx, DbScopeType.ServerHistory, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            // save scope
            if (this is LocalOrchestrator && !hasDeleteClientScopeTable && scope is not null)
            {
                var clientScopeInfo = scope as ScopeInfo;
                clientScopeInfo.Schema = null;
                clientScopeInfo.Setup = null;

                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Client, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    await InternalSaveScopeAsync(ctx, DbScopeType.Client, clientScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }
            else if (this is RemoteOrchestrator && !hasDeleteServerScopeTable && scope is not null)
            {
                var serverScopeInfo = scope as ServerScopeInfo;
                serverScopeInfo.Schema = schema;
                serverScopeInfo.Setup = setup;

                var exists = await InternalExistsScopeInfoTableAsync(ctx, DbScopeType.Server, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    await InternalSaveScopeAsync(ctx, DbScopeType.Server, serverScopeInfo, scopeBuilder, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            }

            var args = new DeprovisionedArgs(ctx, provision, schema, connection);
            await InterceptAsync(args, progress, cancellationToken).ConfigureAwait(false);

            return true;
        }

    }
}
