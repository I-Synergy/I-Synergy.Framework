﻿using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{

    /// <summary>
    /// Contains batches information about changes from underline data sources
    /// </summary>
    public class TableChangesSelectedArgs : ProgressArgs
    {
        public TableChangesSelectedArgs(SyncContext context, BatchInfo batchInfo, IEnumerable<BatchPartInfo> batchPartInfos, SyncTable schemaTable, TableChangesSelected changesSelected, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            BatchInfo = batchInfo;
            BatchPartInfos = batchPartInfos;
            SchemaTable = schemaTable;
            TableChangesSelected = changesSelected;
        }

        /// <summary>
        /// Gets the BatchInfo related
        /// </summary>
        public BatchInfo BatchInfo { get; }

        /// <summary>
        /// Gets the SyncTable instances containing all changes selected.
        /// If you get this instance from a call from GetEstimatedChangesCount, this property is always null
        /// </summary>
        public IEnumerable<BatchPartInfo> BatchPartInfos { get; }

        /// <summary>
        /// Gets the table schema
        /// </summary>
        public SyncTable SchemaTable { get; }

        /// <summary>
        /// Gets the incremental summary of changes selected
        /// </summary>
        public TableChangesSelected TableChangesSelected { get; }

        public override SyncProgressLevel ProgressLevel => TableChangesSelected.TotalChanges > 0 ? SyncProgressLevel.Information : SyncProgressLevel.Debug;

        public override string Source => Connection.Database;
        public override string Message => $"[{TableChangesSelected.TableName}] [Total] Upserts:{TableChangesSelected.Upserts}. Deletes:{TableChangesSelected.Deletes}. Total:{TableChangesSelected.TotalChanges}.";
        public override int EventId => SyncEventsId.TableChangesSelected.Id;
    }

    /// <summary>
    /// Contains SyncRow selected to be written in the batchpart info
    /// </summary>
    public class TableChangesSelectedSyncRowArgs : ProgressArgs
    {
        public TableChangesSelectedSyncRowArgs(SyncContext context, SyncRow syncRow, SyncTable schemaTable, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            SyncRow = syncRow;
            SchemaTable = schemaTable;
        }
        public SyncRow SyncRow { get; set; }

        /// <summary>
        /// Gets the table schema
        /// </summary>
        public SyncTable SchemaTable { get; }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;

        public override string Source => Connection.Database;
        public override string Message => $"[{SchemaTable.GetFullName()}] [SyncRow] {SyncRow.ToString()}.";
        public override int EventId => SyncEventsId.TableChangesSelected.Id;
    }


    /// <summary>
    /// Raise before selecting changes will occur
    /// </summary>
    public class TableChangesSelectingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public TableChangesSelectingArgs(SyncContext context, SyncTable schemaTable, DbCommand command, DbConnection connection, DbTransaction transaction)
            : base(context, connection, transaction)
        {
            SchemaTable = schemaTable;
            Command = command;
        }

        /// <summary>
        /// Gets the table from where the changes are going to be selected.
        /// </summary>
        public SyncTable SchemaTable { get; }
        public override string Source => Connection.Database;
        public override string Message => $"[{SchemaTable.GetFullName()}] Getting Changes.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override int EventId => SyncEventsId.TableChangesSelecting.Id;
    }


 
    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider action when changes are going to be selected on each table defined in the configuration schema
        /// </summary>
        public static void OnTableChangesSelecting(this BaseOrchestrator orchestrator, Action<TableChangesSelectingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider action when changes are going to be selected on each table defined in the configuration schema
        /// </summary>
        public static void OnTableChangesSelecting(this BaseOrchestrator orchestrator, Func<TableChangesSelectingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when changes are selected on each table defined in the configuration schema
        /// </summary>
        public static void OnTableChangesSelected(this BaseOrchestrator orchestrator, Action<TableChangesSelectedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider action when changes are selected on each table defined in the configuration schema
        /// </summary>
        public static void OnTableChangesSelected(this BaseOrchestrator orchestrator, Func<TableChangesSelectedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a sync row is about to be serialized in a batch part info after have been selected from the data source
        /// </summary>
        public static void OnTableChangesSelectedSyncRow(this BaseOrchestrator orchestrator, Action<TableChangesSelectedSyncRowArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a sync row is about to be serialized in a batch part info after have been selected from the data source
        /// </summary>
        public static void OnTableChangesSelectedSyncRow(this BaseOrchestrator orchestrator, Func<TableChangesSelectedSyncRowArgs, Task> action)
            => orchestrator.SetInterceptor(action);


    }

}
