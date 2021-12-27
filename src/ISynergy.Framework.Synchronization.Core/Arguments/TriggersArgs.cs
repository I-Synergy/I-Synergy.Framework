using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public class TriggerCreatedArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public DbTriggerType TriggerType { get; }

        public TriggerCreatedArgs(SyncContext context, SyncTable table, DbTriggerType triggerType, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TriggerType = triggerType;
        }

        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Trigger [{TriggerType}] Created.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TriggerCreated.Id;
    }

    public class TriggerCreatingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public SyncTable Table { get; }
        public DbTriggerType TriggerType { get; }

        public TriggerCreatingArgs(SyncContext context, SyncTable table, DbTriggerType triggerType, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TriggerType = triggerType;
            Command = command;
        }
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Trigger [{TriggerType}] Creating.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TriggerCreating.Id;
    }

    public class TriggerDroppedArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public DbTriggerType TriggerType { get; }

        public TriggerDroppedArgs(SyncContext context, SyncTable table, DbTriggerType triggerType, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TriggerType = triggerType;
        }

        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Trigger [{TriggerType}] Dropped.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TriggerDropped.Id;
    }

    public class TriggerDroppingArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public DbTriggerType TriggerType { get; }

        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }

        public TriggerDroppingArgs(SyncContext context, SyncTable table, DbTriggerType triggerType, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TriggerType = triggerType;
            Command = command;
        }
        public override string Source => Connection.Database;
        public override string Message => $"[{Table.GetFullName()}] Trigger [{TriggerType}] Dropping.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TriggerDropping.Id;
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when a trigger is creating
        /// </summary>
        public static void OnTriggerCreating(this BaseOrchestrator orchestrator, Action<TriggerCreatingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a trigger is creating
        /// </summary>
        public static void OnTriggerCreating(this BaseOrchestrator orchestrator, Func<TriggerCreatingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a trigger is created
        /// </summary>
        public static void OnTriggerCreated(this BaseOrchestrator orchestrator, Action<TriggerCreatedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a trigger is created
        /// </summary>
        public static void OnTriggerCreated(this BaseOrchestrator orchestrator, Func<TriggerCreatedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a trigger is dropping
        /// </summary>
        public static void OnTriggerDropping(this BaseOrchestrator orchestrator, Action<TriggerDroppingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a trigger is dropping
        /// </summary>
        public static void OnTriggerDropping(this BaseOrchestrator orchestrator, Func<TriggerDroppingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a trigger is dropped
        /// </summary>
        public static void OnTriggerDropped(this BaseOrchestrator orchestrator, Action<TriggerDroppedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a trigger is dropped
        /// </summary>
        public static void OnTriggerDropped(this BaseOrchestrator orchestrator, Func<TriggerDroppedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId TriggerCreating => CreateEventId(15000, nameof(TriggerCreating));
        public static EventId TriggerCreated => CreateEventId(15050, nameof(TriggerCreated));
        public static EventId TriggerDropping => CreateEventId(15100, nameof(TriggerDropping));
        public static EventId TriggerDropped => CreateEventId(15150, nameof(TriggerDropped));

    }
}
