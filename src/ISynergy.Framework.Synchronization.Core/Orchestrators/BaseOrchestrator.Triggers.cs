﻿using ISynergy.Framework.Synchronization.Core.Builders;
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
        /// Create a trigger
        /// </summary>
        /// <param name="table">A table from your Setup instance, where you want to create the trigger</param>
        /// <param name="triggerType">Trigger type (Insert, Delete, Update)</param>
        /// <param name="overwrite">If true, drop the existing trriger then create again</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> CreateTriggerAsync(SetupTable table, DbTriggerType triggerType, bool overwrite = false, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Provisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                bool hasBeenCreated = false;

                var schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var schemaTable = schema.Tables[table.TableName, table.SchemaName];

                if (schemaTable is null)
                    throw new MissingTableException(table.GetFullName());

                // Get table builder
                var tableBuilder = GetTableBuilder(schemaTable, Setup);

                var exists = await InternalExistsTriggerAsync(GetContext(), tableBuilder, triggerType, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                // should create only if not exists OR if overwrite has been set
                var shouldCreate = !exists || overwrite;

                if (shouldCreate)
                {
                    // Drop trigger if already exists
                    if (exists && overwrite)
                        await InternalDropTriggerAsync(GetContext(), tableBuilder, triggerType, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                    hasBeenCreated = await InternalCreateTriggerAsync(GetContext(), tableBuilder, triggerType, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);
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
        /// Create a trigger
        /// </summary>
        /// <param name="table">A table from your Setup instance, where you want to create the triggers</param>
        /// <param name="overwrite">If true, drop the existing triggers then create them all, again</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> CreateTriggersAsync(SetupTable table, bool overwrite = false, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Provisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                var schema = await InternalGetSchemaAsync(GetContext(), Setup, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                var schemaTable = schema.Tables[table.TableName, table.SchemaName];

                if (schemaTable is null)
                    throw new MissingTableException(table.GetFullName());

                // Get table builder
                var tableBuilder = GetTableBuilder(schemaTable, Setup);

                var r = await InternalCreateTriggersAsync(GetContext(), overwrite, tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return r;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Check if a trigger exists
        /// </summary>
        /// <param name="table">A table from your Setup instance, where you want to check if the trigger exists</param>
        /// <param name="triggerType">Trigger type (Insert, Delete, Update)</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> ExistTriggerAsync(SetupTable table, DbTriggerType triggerType, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.None, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // Fake sync table without column definitions. Not need for making a check exists call
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);

                // Get table builder
                var tableBuilder = GetTableBuilder(schemaTable, Setup);

                var exists = await InternalExistsTriggerAsync(GetContext(), tableBuilder, triggerType, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                return exists;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Dropping a trigger
        /// </summary>
        /// <param name="table">A table from your Setup instance, where you want to drop the trigger</param>
        /// <param name="triggerType">Trigger type (Insert, Delete, Update)</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> DropTriggerAsync(SetupTable table, DbTriggerType triggerType, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Deprovisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                bool hasBeenDropped = false;

                // Fake sync table without column definitions. Not need for making a drop call
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);

                // Get table builder
                var tableBuilder = GetTableBuilder(schemaTable, Setup);

                var existsAndCanBeDeleted = await InternalExistsTriggerAsync(GetContext(), tableBuilder, triggerType, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                if (existsAndCanBeDeleted)
                    hasBeenDropped = await InternalDropTriggerAsync(GetContext(), tableBuilder, triggerType, runner.Connection, runner.Transaction, cancellationToken, progress).ConfigureAwait(false);

                await runner.CommitAsync().ConfigureAwait(false);

                return hasBeenDropped;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }

        }

        /// <summary>
        /// Drop all triggers
        /// </summary>
        /// <param name="table">A table from your Setup instance, where you want to drop all the triggers</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        public async Task<bool> DropTriggersAsync(SetupTable table, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            try
            {
                await using var runner = await this.GetConnectionAsync(SyncStage.Deprovisioning, connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                // Fake sync table without column definitions. Not need for making a drop call
                var schemaTable = new SyncTable(table.TableName, table.SchemaName);

                // Get table builder
                var tableBuilder = GetTableBuilder(schemaTable, Setup);

                var r = await InternalDropTriggersAsync(GetContext(), tableBuilder, runner.Connection, runner.Transaction, cancellationToken, progress);

                await runner.CommitAsync().ConfigureAwait(false);

                return r;
            }
            catch (Exception ex)
            {
                throw GetSyncError(ex);
            }
        }

        /// <summary>
        /// Internal create trigger routine
        /// </summary>
        internal async Task<bool> InternalCreateTriggerAsync(SyncContext ctx, DbTableBuilder tableBuilder, DbTriggerType triggerType, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            if (tableBuilder.TableDescription.Columns.Count <= 0)
                throw new MissingsColumnException(tableBuilder.TableDescription.GetFullName());

            if (tableBuilder.TableDescription.PrimaryKeys.Count <= 0)
                throw new MissingPrimaryKeyException(tableBuilder.TableDescription.GetFullName());

            var command = await tableBuilder.GetCreateTriggerCommandAsync(triggerType, connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var action = await InterceptAsync(new TriggerCreatingArgs(ctx, tableBuilder.TableDescription, triggerType, command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync();
            await InterceptAsync(new TriggerCreatedArgs(ctx, tableBuilder.TableDescription, triggerType, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Internal create triggers routine
        /// </summary>
        internal async Task<bool> InternalCreateTriggersAsync(SyncContext ctx, bool overwrite, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var hasCreatedAtLeastOneTrigger = false;

            var listTriggerType = Enum.GetValues(typeof(DbTriggerType));

            foreach (DbTriggerType triggerType in listTriggerType)
            {
                var exists = await InternalExistsTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                // Drop trigger if already exists
                if (exists && overwrite)
                    await InternalDropTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                var shouldCreate = !exists || overwrite;

                if (!shouldCreate)
                    continue;

                var hasBeenCreated = await InternalCreateTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (hasBeenCreated)
                    hasCreatedAtLeastOneTrigger = true;
            }

            return hasCreatedAtLeastOneTrigger;
        }

        /// <summary>
        /// Internal drop trigger routine
        /// </summary>
        internal async Task<bool> InternalDropTriggerAsync(SyncContext ctx, DbTableBuilder tableBuilder, DbTriggerType triggerType, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var command = await tableBuilder.GetDropTriggerCommandAsync(triggerType, connection, transaction).ConfigureAwait(false);

            if (command is null)
                return false;

            var action = await InterceptAsync(new TriggerDroppingArgs(ctx, tableBuilder.TableDescription, triggerType, command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            if (action.Cancel || action.Command is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, action.Command, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            await action.Command.ExecuteNonQueryAsync();
            await InterceptAsync(new TriggerDroppedArgs(ctx, tableBuilder.TableDescription, triggerType, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Internal drop triggers routine
        /// </summary>
        internal async Task<bool> InternalDropTriggersAsync(SyncContext ctx, DbTableBuilder tableBuilder, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var hasDroppeAtLeastOneTrigger = false;

            var listTriggerType = Enum.GetValues(typeof(DbTriggerType));

            foreach (DbTriggerType triggerType in listTriggerType)
            {
                var exists = await InternalExistsTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (!exists)
                    continue;

                var dropped = await InternalDropTriggerAsync(ctx, tableBuilder, triggerType, connection, transaction, cancellationToken, progress).ConfigureAwait(false);

                if (dropped)
                    hasDroppeAtLeastOneTrigger = true;
            }

            return hasDroppeAtLeastOneTrigger;

        }

        /// <summary>
        /// Internal exists trigger procedure routine
        /// </summary>
        internal async Task<bool> InternalExistsTriggerAsync(SyncContext ctx, DbTableBuilder tableBuilder, DbTriggerType triggerType, DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            // Get exists command
            var existsCommand = await tableBuilder.GetExistsTriggerCommandAsync(triggerType, connection, transaction).ConfigureAwait(false);

            if (existsCommand is null)
                return false;

            await InterceptAsync(new DbCommandArgs(ctx, existsCommand, connection, transaction), progress, cancellationToken).ConfigureAwait(false);

            var existsResultObject = await existsCommand.ExecuteScalarAsync().ConfigureAwait(false);
            var exists = Convert.ToInt32(existsResultObject) > 0;
            return exists;

        }



    }
}