using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public class ColumnCreatedArgs : ProgressArgs
    {
        public string ColumnName { get; }
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;

        public ColumnCreatedArgs(SyncContext context, string columnName, SyncTable table, ParserName tableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            TableName = tableName;
            ColumnName = columnName;
            Table = table;
        }

        public override string Source => Connection.Database;
        public override string Message => $"[{ColumnName}] Added.";

        public override int EventId => SyncEventsId.ColumnCreated.Id;
    }

    public class ColumnCreatingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public string ColumnName { get; }
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public ColumnCreatingArgs(SyncContext context, string columnName, SyncTable table, ParserName tableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            ColumnName = columnName;
            Table = table;
            TableName = tableName;
            Command = command;
        }
        public override string Source => Connection.Database;
        public override string Message => $"[{ColumnName}] Adding.";
        public override int EventId => SyncEventsId.ColumnCreating.Id;

    }

    public class ColumnDroppedArgs : ProgressArgs
    {
        public string ColumnName { get; }
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public ColumnDroppedArgs(SyncContext context, string columnName, SyncTable table, ParserName tableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            TableName = tableName;
            ColumnName = columnName;
            Table = table;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;

        public override string Source => Connection.Database;
        public override string Message => $"[{ColumnName}] Dropped.";
        public override int EventId => SyncEventsId.ColumnDropped.Id;
    }

    public class ColumnDroppingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public string ColumnName { get; }
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public ColumnDroppingArgs(SyncContext context, string columnName, SyncTable table, ParserName tableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Command = command;
            TableName = tableName;
            ColumnName = columnName;
            Table = table;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;

        public override string Source => Connection.Database;
        public override string Message => $"[{ColumnName}] Dropping.";

        public override int EventId => SyncEventsId.ColumnDropping.Id;

    }


    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when a column is creating
        /// </summary>
        public static void OnColumnCreating(this BaseOrchestrator orchestrator, Action<ColumnCreatingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a column is creating
        /// </summary>
        public static void OnColumnCreating(this BaseOrchestrator orchestrator, Func<TableCreatingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a column is created
        /// </summary>
        public static void OnColumnCreated(this BaseOrchestrator orchestrator, Action<ColumnCreatedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a column is created
        /// </summary>
        public static void OnColumnCreated(this BaseOrchestrator orchestrator, Func<ColumnCreatedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a column is dropping
        /// </summary>
        public static void OnColumnDropping(this BaseOrchestrator orchestrator, Action<ColumnDroppingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a column is dropping
        /// </summary>
        public static void OnColumnDropping(this BaseOrchestrator orchestrator, Func<ColumnDroppingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a column is dropped
        /// </summary>
        public static void OnColumnDropped(this BaseOrchestrator orchestrator, Action<ColumnDroppedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a column is dropped
        /// </summary>
        public static void OnColumnDropped(this BaseOrchestrator orchestrator, Func<ColumnDroppedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId ColumnCreating => CreateEventId(12300, nameof(ColumnCreating));
        public static EventId ColumnCreated => CreateEventId(12350, nameof(ColumnCreated));
        public static EventId ColumnDropping => CreateEventId(12400, nameof(ColumnDropping));
        public static EventId ColumnDropped => CreateEventId(12450, nameof(ColumnDropped));
    }

}
