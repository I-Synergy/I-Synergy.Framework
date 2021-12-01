﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{

    public class LocalTimestampLoadingArgs : ProgressArgs
    {
        public bool Cancel { get; set; } = false;
        public DbCommand Command { get; set; }

        public LocalTimestampLoadingArgs(SyncContext context, DbCommand command, DbConnection connection, DbTransaction transaction) : base(context, connection, transaction)
        {
            this.Command = command;
        }

        public override string Source => Connection.Database;
        public override string Message => $"Getting Local Timestamp.";

        public override int EventId => SyncEventsId.LocalTimestampLoading.Id;
    }
    public class LocalTimestampLoadedArgs : ProgressArgs
    {
        public LocalTimestampLoadedArgs(SyncContext context, long localTimestamp, DbConnection connection, DbTransaction transaction) : base(context, connection, transaction)
        {
            this.LocalTimestamp = localTimestamp;
        }

        public override string Source => Connection.Database;
        public override string Message => $"Local Timestamp Loaded:{LocalTimestamp}.";
        public long LocalTimestamp { get; }
        public override int EventId => SyncEventsId.LocalTimestampLoaded.Id;
    }
    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider action when a database is reading a timestamp
        /// </summary>
        public static void OnLocalTimestampLoading(this BaseOrchestrator orchestrator, Action<LocalTimestampLoadingArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a database is reading a timestamp
        /// </summary>
        public static void OnLocalTimestampLoading(this BaseOrchestrator orchestrator, Func<LocalTimestampLoadingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a database has read a timestamp
        /// </summary>
        public static void OnLocalTimestampLoaded(this BaseOrchestrator orchestrator, Action<LocalTimestampLoadedArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a database has read a timestamp
        /// </summary>
        public static void OnLocalTimestampLoaded(this BaseOrchestrator orchestrator, Func<LocalTimestampLoadedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId LocalTimestampLoading => CreateEventId(2000, nameof(LocalTimestampLoading));
        public static EventId LocalTimestampLoaded => CreateEventId(2050, nameof(LocalTimestampLoaded));
    }

}
