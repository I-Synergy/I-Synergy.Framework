﻿using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public class TrackingTableCreatedArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public ParserName TrackingTableName { get; }

        public TrackingTableCreatedArgs(SyncContext context, SyncTable table, ParserName trackingTableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TrackingTableName = trackingTableName;
        }

        public override string Source => Connection.Database;
        public override string Message => $"[{TrackingTableName}] tracking table created.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TrackingTableCreated.Id;
    }

    public class TrackingTableCreatingArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public ParserName TrackingTableName { get; }
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }

        public TrackingTableCreatingArgs(SyncContext context, SyncTable table, ParserName trackingTableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TrackingTableName = trackingTableName;
            Command = command;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{TrackingTableName}] tracking table creating.";
        public override int EventId => SyncEventsId.TrackingTableCreating.Id;

    }

    public class TrackingTableDroppedArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public ParserName TrackingTableName { get; }

        public TrackingTableDroppedArgs(SyncContext context, SyncTable table, ParserName trackingTableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TrackingTableName = trackingTableName;
        }

        public override string Source => Connection.Database;
        public override string Message => $"[{TrackingTableName}] Tracking Table Dropped.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TrackingTableDropped.Id;
    }

    public class TrackingTableDroppingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public SyncTable Table { get; }
        public ParserName TrackingTableName { get; }

        public TrackingTableDroppingArgs(SyncContext context, SyncTable table, ParserName trackingTableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TrackingTableName = trackingTableName;
            Command = command;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Source => Connection.Database;
        public override string Message => $"[{TrackingTableName}] Tracking Table Dropping.";
        public override int EventId => SyncEventsId.TrackingTableDropping.Id;

    }

    public class TrackingTableRenamedArgs : ProgressArgs
    {
        public ParserName TrackingTableName { get; }
        public ParserName OldTrackingTableName { get; set; }

        public TrackingTableRenamedArgs(SyncContext context, SyncTable table, ParserName trackingTableName, ParserName oldTrackingTableName, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            TrackingTableName = trackingTableName;
            OldTrackingTableName = oldTrackingTableName;
        }

        public override string Source => Connection.Database;
        public override string Message => $"[{TrackingTableName}] Tracking Table Renamed.";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => SyncEventsId.TrackingTableRenamed.Id;
    }

    public class TrackingTableRenamingArgs : ProgressArgs
    {
        public SyncTable Table { get; }
        public ParserName TrackingTableName { get; }
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }
        public ParserName OldTrackingTableName { get; set; }

        public TrackingTableRenamingArgs(SyncContext context, SyncTable table, ParserName trackingTableName, ParserName oldTrackingTableName, DbCommand command, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            Table = table;
            TrackingTableName = trackingTableName;
            Command = command;
            OldTrackingTableName = oldTrackingTableName;

        }
        public override string Source => Connection.Database;
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override string Message => $"[{TrackingTableName}] Tracking Table Renaming.";
        public override int EventId => SyncEventsId.TrackingTableRenaming.Id;

    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableCreating(this BaseOrchestrator orchestrator, Action<TrackingTableCreatingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableCreating(this BaseOrchestrator orchestrator, Func<TrackingTableCreatingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableCreated(this BaseOrchestrator orchestrator, Action<TrackingTableCreatedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableCreated(this BaseOrchestrator orchestrator, Func<TrackingTableCreatedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a tracking table is dropping
        /// </summary>
        public static void OnTrackingTableDropping(this BaseOrchestrator orchestrator, Action<TrackingTableDroppingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a tracking table is dropping
        /// </summary>
        public static void OnTrackingTableDropping(this BaseOrchestrator orchestrator, Func<TrackingTableDroppingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a tracking table is dropped
        /// </summary>
        public static void OnTrackingTableDropped(this BaseOrchestrator orchestrator, Action<TrackingTableDroppedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a tracking table is dropped
        /// </summary>
        public static void OnTrackingTableDropped(this BaseOrchestrator orchestrator, Func<TrackingTableDroppedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableRenaming(this BaseOrchestrator orchestrator, Action<TrackingTableRenamingArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableRenaming(this BaseOrchestrator orchestrator, Func<TrackingTableRenamingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableRenamed(this BaseOrchestrator orchestrator, Action<TrackingTableRenamedArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when a tracking table is creating
        /// </summary>
        public static void OnTrackingTableRenamed(this BaseOrchestrator orchestrator, Func<TrackingTableRenamedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId TrackingTableCreating => CreateEventId(14000, nameof(TrackingTableCreating));
        public static EventId TrackingTableCreated => CreateEventId(14050, nameof(TrackingTableCreated));
        public static EventId TrackingTableDropping => CreateEventId(14100, nameof(TrackingTableDropping));
        public static EventId TrackingTableDropped => CreateEventId(14150, nameof(TrackingTableDropped));
        public static EventId TrackingTableRenaming => CreateEventId(14200, nameof(TrackingTableRenaming));
        public static EventId TrackingTableRenamed => CreateEventId(14250, nameof(TrackingTableRenamed));

    }
}
