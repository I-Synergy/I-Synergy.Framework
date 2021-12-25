﻿using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{


    /// <summary>
    /// Event args raised when a batch changes is applied on a datasource
    /// </summary>
    public class TableChangesBatchAppliedArgs : ProgressArgs
    {
        public TableChangesBatchAppliedArgs(SyncContext context, TableChangesApplied tableChangesApplied, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            TableChangesApplied = tableChangesApplied;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        /// <summary>
        /// Table changes applied
        /// </summary>
        public TableChangesApplied TableChangesApplied { get; set; }

        public override string Source => Connection.Database;
        public override string Message => $"[{TableChangesApplied.TableName}] [{TableChangesApplied.State}] " +
                                          $"Applied:({TableChangesApplied.Applied}) Total:({TableChangesApplied.TotalAppliedCount}/{TableChangesApplied.TotalRowsCount}).";

        public override int EventId => SyncEventsId.TableChangesApplied.Id;
    }

    /// <summary>
    /// Event args before a batch changes is going to be applied on a datasource
    /// </summary>
    public class TableChangesApplyingSyncRowsArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }

        public TableChangesApplyingSyncRowsArgs(SyncContext context, BatchInfo batchInfo, List<SyncRow> syncRows, SyncTable schemaTable, DataRowState state, DbCommand command, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            State = state;
            Command = command;
            BatchInfo = batchInfo;
            SyncRows = syncRows;
            SchemaTable = schemaTable;
        }

        /// <summary>
        /// Gets the RowState of the applied rows
        /// </summary>
        public DataRowState State { get; }
        public BatchInfo BatchInfo { get; }
        public List<SyncRow> SyncRows { get; }

        public SyncTable SchemaTable { get; }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Source => Connection.Database;
        public override string Message => $"Applying [{SchemaTable.GetFullName()}] batch rows. State:{State}. Count:{SyncRows.Count()}";

        public override int EventId => SyncEventsId.TableChangesApplying.Id;
    }

    /// <summary>
    /// Event args raised when all changes for a table have been applied on a datasource
    /// </summary>
    public class TableChangesAppliedArgs : ProgressArgs
    {
        public TableChangesAppliedArgs(SyncContext context, TableChangesApplied tableChangesApplied, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            TableChangesApplied = tableChangesApplied;
        }

        public TableChangesApplied TableChangesApplied { get; set; }
        public override SyncProgressLevel ProgressLevel => TableChangesApplied.Applied > 0 ? SyncProgressLevel.Information : SyncProgressLevel.Debug;
        public override string Source => Connection.Database;
        public override string Message => $"[{TableChangesApplied.TableName}] Changes {TableChangesApplied.State} Applied:{TableChangesApplied.Applied}. Resolved Conflicts:{TableChangesApplied.ResolvedConflicts}.";
        public override int EventId => SyncEventsId.TableChangesApplied.Id;
    }


    /// <summary>
    /// Event args before a table changes is going to be applied on a datasource
    /// </summary>
    public class TableChangesApplyingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;

        public TableChangesApplyingArgs(SyncContext context, BatchInfo batchInfo, IEnumerable<BatchPartInfo> batchPartInfos, SyncTable schemaTable, DataRowState state, DbCommand command, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            BatchInfo = batchInfo;
            BatchPartInfos = batchPartInfos;
            SchemaTable = schemaTable;
            State = state;
            Command = command;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        /// <summary>
        /// Gets the RowState of the applied rows
        /// </summary>
        public DataRowState State { get; }
        public DbCommand Command { get; set; }
        public BatchInfo BatchInfo { get; set; }
        public IEnumerable<BatchPartInfo> BatchPartInfos { get; }

        /// <summary>
        /// Gets the table schema
        /// </summary>
        public SyncTable SchemaTable { get; }

        public override string Source => Connection.Database;
        public override string Message => $"Applying Changes To {SchemaTable.GetFullName()}.";

        public override int EventId => SyncEventsId.TableChangesApplying.Id;
    }


    public static partial class InterceptorsExtensions
    {

        /// <summary>
        /// Intercept the provider action when a table starts to apply changes
        /// </summary>
        public static void OnTableChangesApplying(this BaseOrchestrator orchestrator, Action<TableChangesApplyingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider action when a table starts to apply changes
        /// </summary>
        public static void OnTableChangesApplying(this BaseOrchestrator orchestrator, Func<TableChangesApplyingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a batch changes is going to be applied on a table
        /// </summary>
        public static void OnTableChangesApplyingSyncRows(this BaseOrchestrator orchestrator, Action<TableChangesApplyingSyncRowsArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider action when a batch changes is going to be applied on a table
        /// </summary>
        public static void OnTableChangesApplyingSyncRows(this BaseOrchestrator orchestrator, Func<TableChangesApplyingSyncRowsArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a batch changes is applied on a datasource table
        /// </summary>
        public static void OnTableChangesBatchApplied(this BaseOrchestrator orchestrator, Action<TableChangesBatchAppliedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider action when a batch changes is applied on a datasource table
        /// </summary>
        public static void OnTableChangesBatchApplied(this BaseOrchestrator orchestrator, Func<TableChangesBatchAppliedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a all changes have been applied on a datasource table
        /// </summary>
        public static void OnTableChangesApplied(this BaseOrchestrator orchestrator, Action<TableChangesAppliedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider action when a all changes have been applied on a datasource table
        /// </summary>
        public static void OnTableChangesApplied(this BaseOrchestrator orchestrator, Func<TableChangesAppliedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId TableChangesSelecting => CreateEventId(13000, nameof(TableChangesSelecting));
        public static EventId TableChangesSelected => CreateEventId(13050, nameof(TableChangesSelected));
        public static EventId TableChangesApplying => CreateEventId(13100, nameof(TableChangesApplying));
        public static EventId TableChangesApplied => CreateEventId(13150, nameof(TableChangesApplied));

    }
}
