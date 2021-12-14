﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
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

        /// <summary>
        /// Create a table
        /// </summary>
        /// <param name="syncTable"></param>
        /// <param name="overwrite"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> CreateTableAsync(SyncTable syncTable, bool overwrite = false, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Provisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var hasBeenCreated = false;

                // Get table builder
                var tableBuilder = GetTableBuilder(syncTable, Setup);

                var schemaExists = await InternalExistsSchemaAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!schemaExists)
                    await InternalCreateSchemaAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var exists = await InternalExistsTableAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // should create only if not exists OR if overwrite has been set
                var shouldCreate = !exists || overwrite;

                if (shouldCreate)
                {
                    // Drop if already exists and we need to overwrite
                    if (exists && overwrite)
                        await InternalDropTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    hasBeenCreated = await InternalCreateTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                await runner.CommitAsync().ConfigureAwait(false);

                return hasBeenCreated;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        /// <summary>
        /// Create all tables
        /// </summary>
        /// <param name="schema">A complete schema you want to create, containing table, primary keys and relations</param>
        /// <param name="overwrite"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> CreateTablesAsync(SyncSet schema, bool overwrite = false, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Provisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var atLeastOneHasBeenCreated = false;

                // Sorting tables based on dependencies between them
                var schemaTables = schema.Tables.SortByDependencies(tab => tab.GetRelations().Select(r => r.GetParentTable()));

                // if we overwritten all tables, we need to delete all of them, before recreating them
                if (overwrite)
                {
                    foreach (var schemaTable in schemaTables.Reverse())
                    {
                        var tableBuilder = GetTableBuilder(schemaTable, Setup);
                        var exists = await InternalExistsTableAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                        if (exists)
                            await InternalDropTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                    }
                }
                // Then create them
                foreach (var schemaTable in schema.Tables)
                {
                    // Get table builder
                    var tableBuilder = GetTableBuilder(schemaTable, Setup);

                    var schemaExists = await InternalExistsSchemaAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (!schemaExists)
                        await InternalCreateSchemaAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    var exists = await InternalExistsTableAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    // should create only if not exists OR if overwrite has been set
                    var shouldCreate = !exists || overwrite;

                    if (shouldCreate)
                    {
                        // Drop if already exists and we need to overwrite
                        if (exists && overwrite)
                            await InternalDropTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                        var hasBeenCreated = await InternalCreateTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                        if (hasBeenCreated)
                            atLeastOneHasBeenCreated = true;
                    }
                }

                await runner.CommitAsync().ConfigureAwait(false);

                return atLeastOneHasBeenCreated;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }


        /// <summary>
        /// Check if a table exists
        /// </summary>
        /// <param name="table">A table from your Setup instance, you want to check if it exists</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> ExistTableAsync(SetupTable table, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.None, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // Fake sync table without column definitions. Not need for making a check exists call
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);
                var tableBuilder = GetTableBuilder(schemaTable, Setup);
                var exists = await InternalExistsTableAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                await runner.CommitAsync().ConfigureAwait(false);
                return exists;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Drop a table
        /// </summary>
        /// <param name="table">A table from your Setup instance you want to drop</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> DropTableAsync(SetupTable table, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Deprovisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var hasBeenDropped = false;

                var schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                var schemaTable = schema.Tables[table.TableName, table.SchemaName];

                if (schemaTable is null)
                    throw new MissingTableException(table.GetFullName());

                // Get table builder
                var tableBuilder = GetTableBuilder(schemaTable, Setup);

                var exists = await InternalExistsTableAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (exists)
                    hasBeenDropped = await InternalDropTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return hasBeenDropped;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Drop all tables, declared in the Setup instance
        /// </summary>
        public async Task<bool> DropTablesAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Deprovisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                bool atLeastOneTableHasBeenDropped = false;

                var schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // Sorting tables based on dependencies between them
                var schemaTables = schema.Tables.SortByDependencies(tab => tab.GetRelations().Select(r => r.GetParentTable()));

                foreach (var schemaTable in schemaTables.Reverse())
                {
                    // Get table builder
                    var tableBuilder = GetTableBuilder(schemaTable, Setup);

                    var exists = await InternalExistsTableAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    if (exists)
                        atLeastOneTableHasBeenDropped = await InternalDropTableAsync(GetContext(), Setup, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
                }

                await runner.CommitAsync().ConfigureAwait(false);

                return atLeastOneTableHasBeenDropped;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }


        /// <summary>
        /// Internal add column routine
        /// </summary>
        internal async Task<bool> InternalAddColumnAsync(SyncContext ctx, SyncSetup setup, string addedColumnName, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            if (tableBuilder.TableDescription.Columns.Count <= 0)
                throw new MissingsColumnException(tableBuilder.TableDescription.GetFullName());

            if (tableBuilder.TableDescription.PrimaryKeys.Count <= 0)
                throw new MissingPrimaryKeyException(tableBuilder.TableDescription.GetFullName());

            var command = await tableBuilder.GetAddColumnCommandAsync(addedColumnName, connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var (tableName, _) = Provider.GetParsers(tableBuilder.TableDescription, setup);

            var action = new ColumnCreatingArgs(ctx, addedColumnName, tableBuilder.TableDescription, tableName, command, connection, transaction);

            await InterceptAsync(action, progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await InterceptAsync(new ColumnCreatedArgs(ctx, addedColumnName, tableBuilder.TableDescription, tableName, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;
        }


        /// <summary>
        /// Internal add column routine
        /// </summary>
        internal async Task<bool> InternalDropColumnAsync(SyncContext ctx, SyncSetup setup, string droppedColumnName, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            if (tableBuilder.TableDescription.Columns.Count <= 0)
                throw new MissingsColumnException(tableBuilder.TableDescription.GetFullName());

            if (tableBuilder.TableDescription.PrimaryKeys.Count <= 0)
                throw new MissingPrimaryKeyException(tableBuilder.TableDescription.GetFullName());

            var command = await tableBuilder.GetDropColumnCommandAsync(droppedColumnName, connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var (tableName, _) = Provider.GetParsers(tableBuilder.TableDescription, setup);

            var action = await InterceptAsync(new ColumnDroppingArgs(ctx, droppedColumnName, tableBuilder.TableDescription, tableName, command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await InterceptAsync(new ColumnDroppedArgs(ctx, droppedColumnName, tableBuilder.TableDescription, tableName, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Internal create table routine
        /// </summary>
        internal async Task<bool> InternalCreateTableAsync(SyncContext ctx, SyncSetup setup, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            if (tableBuilder.TableDescription.Columns.Count <= 0)
                throw new MissingsColumnException(tableBuilder.TableDescription.GetFullName());

            if (tableBuilder.TableDescription.PrimaryKeys.Count <= 0)
                throw new MissingPrimaryKeyException(tableBuilder.TableDescription.GetFullName());

            var command = await tableBuilder.GetCreateTableCommandAsync(connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var (tableName, _) = Provider.GetParsers(tableBuilder.TableDescription, setup);

            var action = new TableCreatingArgs(ctx, tableBuilder.TableDescription, tableName, command, connection, transaction);

            await InterceptAsync(action, progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await InterceptAsync(new TableCreatedArgs(ctx, tableBuilder.TableDescription, tableName, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Internal create table routine
        /// </summary>
        internal async Task<bool> InternalCreateSchemaAsync(SyncContext ctx, SyncSetup setup, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var command = await tableBuilder.GetCreateSchemaCommandAsync(connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var action = await InterceptAsync(new SchemaNameCreatingArgs(ctx, tableBuilder.TableDescription, command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await InterceptAsync(new SchemaNameCreatedArgs(ctx, tableBuilder.TableDescription, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Internal drop table routine
        /// </summary>
        internal async Task<bool> InternalDropTableAsync(SyncContext ctx, SyncSetup setup, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var command = await tableBuilder.GetDropTableCommandAsync(connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var (tableName, _) = Provider.GetParsers(tableBuilder.TableDescription, setup);

            var action = await InterceptAsync(new TableDroppingArgs(ctx, tableBuilder.TableDescription, tableName, command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync().ConfigureAwait(false);

            await InterceptAsync(new TableDroppedArgs(ctx, tableBuilder.TableDescription, tableName, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;

        }

        /// <summary>
        /// Internal exists table procedure routine
        /// </summary>
        internal async Task<bool> InternalExistsTableAsync(SyncContext ctx, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            // Get exists command
            var existsCommand = await tableBuilder.GetExistsTableCommandAsync(connection, transaction).ConfigureAwait(false);

            if (existsCommand is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, existsCommand, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            var existsResultObject = await existsCommand.ExecuteScalarAsync().ConfigureAwait(false);
            var exists = Convert.ToInt32(existsResultObject) > 0;
            return exists;

        }

        /// <summary>
        /// Internal exists schema procedure routine
        /// </summary>
        internal async Task<bool> InternalExistsSchemaAsync(SyncContext ctx, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            if (string.IsNullOrEmpty(tableBuilder.TableDescription.SchemaName) || tableBuilder.TableDescription.SchemaName == "dbo")
                return true;

            // Get exists command
            var existsCommand = await tableBuilder.GetExistsSchemaCommandAsync(connection, transaction).ConfigureAwait(false);

            if (existsCommand is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, existsCommand, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            var existsResultObject = await existsCommand.ExecuteScalarAsync().ConfigureAwait(false);
            var exists = Convert.ToInt32(existsResultObject) > 0;
            return exists;

        }

        /// <summary>
        /// Internal exists column procedure routine
        /// </summary>
        internal async Task<bool> InternalExistsColumnAsync(SyncContext ctx, string columnName, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            // Get exists command
            var existsCommand = await tableBuilder.GetExistsColumnCommandAsync(columnName, connection, transaction).ConfigureAwait(false);

            if (existsCommand is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, existsCommand, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            var existsResultObject = await existsCommand.ExecuteScalarAsync().ConfigureAwait(false);
            var exists = Convert.ToInt32(existsResultObject) > 0;
            return exists;

        }



    }
}
