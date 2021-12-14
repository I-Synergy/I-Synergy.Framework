using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public class SchemaNameCreatedArgs : ProgressArgs
    {
        public SyncTable Table { get; }

        public SchemaNameCreatedArgs(SyncContext context, SyncTable table, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.SchemaName}] Schema Created.";

        public override int EventId => SyncEventsId.SchemaNameCreated.Id;
    }

    public class SchemaNameCreatingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public SyncTable Table { get; }

        public SchemaNameCreatingArgs(SyncContext context, SyncTable table, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            Command = command;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.SchemaName}] Schema Creating.";
        public override int EventId => SyncEventsId.SchemaNameCreating.Id;
    }

    public class TableCreatedArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public TableCreatedArgs(SyncContext context, SyncTable table, ParserName tableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            TableName = tableName;
            Table = table;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Table Created.";
        public override int EventId => SyncEventsId.TableCreated.Id;
    }

    public class TableCreatingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public TableCreatingArgs(SyncContext context, SyncTable table, ParserName tableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TableName = tableName;
            Command = command;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Table Creating.";
        public override int EventId => SyncEventsId.TableCreating.Id;
    }

    public class TableDroppedArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public TableDroppedArgs(SyncContext context, SyncTable table, ParserName tableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            TableName = tableName;
            Table = table;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Table Dropped.";
        public override int EventId => SyncEventsId.TableDropped.Id;
    }

    public class TableDroppingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public SyncTable Table { get; }
        public ParserName TableName { get; }

        public TableDroppingArgs(SyncContext context, SyncTable table, ParserName tableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context,  connection, transaction)
        {
            Command = command;
            TableName = tableName;
            Table = table;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Table Dropping.";
        public override int EventId => SyncEventsId.TableDropping.Id;
    }


    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when database schema is created (works only on SQL Server)
        /// </summary>
        public static void OnSchemaNameCreated(this BaseOrchestrator orchestrator, Action<SchemaNameCreatedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when database schema is created (works only on SQL Server)
        /// </summary>
        public static void OnSchemaNameCreated(this BaseOrchestrator orchestrator, Func<SchemaNameCreatedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when database schema is creating (works only on SQL Server)
        /// </summary>
        public static void OnSchemaNameCreating(this BaseOrchestrator orchestrator, Action<SchemaNameCreatingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when database schema is creating (works only on SQL Server)
        /// </summary>
        public static void OnSchemaNameCreating(this BaseOrchestrator orchestrator, Func<SchemaNameCreatingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a table is creating
        /// </summary>
        public static void OnTableCreating(this BaseOrchestrator orchestrator, Action<TableCreatingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a table is creating
        /// </summary>
        public static void OnTableCreating(this BaseOrchestrator orchestrator, Func<TableCreatingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a table is created
        /// </summary>
        public static void OnTableCreated(this BaseOrchestrator orchestrator, Action<TableCreatedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a table is created
        /// </summary>
        public static void OnTableCreated(this BaseOrchestrator orchestrator, Func<TableCreatedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a table is dropping
        /// </summary>
        public static void OnTableDropping(this BaseOrchestrator orchestrator, Action<TableDroppingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a table is dropping
        /// </summary>
        public static void OnTableDropping(this BaseOrchestrator orchestrator, Func<TableDroppingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a table is dropped
        /// </summary>
        public static void OnTableDropped(this BaseOrchestrator orchestrator, Action<TableDroppedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a table is dropped
        /// </summary>
        public static void OnTableDropped(this BaseOrchestrator orchestrator, Func<TableDroppedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId SchemaNameCreating => CreateEventId(12000, nameof(SchemaNameCreating));
        public static EventId SchemaNameCreated => CreateEventId(12050, nameof(SchemaNameCreated));
        public static EventId TableCreating => CreateEventId(12100, nameof(TableCreating));
        public static EventId TableCreated => CreateEventId(12150, nameof(TableCreated));
        public static EventId TableDropping => CreateEventId(12200, nameof(TableDropping));
        public static EventId TableDropped => CreateEventId(12250, nameof(TableDropped));
    }

}
