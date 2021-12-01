﻿using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core
{
    public class MetadataCleaningArgs : ProgressArgs
    {
        public SyncSetup Setup { get; }
        public long TimeStampStart { get; }

        public MetadataCleaningArgs(SyncContext context, SyncSetup setup, long timeStampStart, DbConnection connection, DbTransaction transaction)
        : base(context, connection, transaction)

        {
            this.Setup = setup;
            this.TimeStampStart = timeStampStart;
        }
        public override string Source => Connection.Database;
        public override string Message => $"Cleaning Metadatas.";

        public override int EventId => SyncEventsId.MetadataCleaning.Id;
    }

    public class MetadataCleanedArgs : ProgressArgs
    {
        public MetadataCleanedArgs(SyncContext context, DatabaseMetadatasCleaned databaseMetadatasCleaned, DbConnection connection = null, DbTransaction transaction = null)
            : base(context, connection, transaction)
        {
            this.DatabaseMetadatasCleaned = databaseMetadatasCleaned;
        }

        /// <summary>
        /// Gets or Sets the rows count cleaned for all tables, during a DeleteMetadatasAsync call
        /// </summary>
        public DatabaseMetadatasCleaned DatabaseMetadatasCleaned { get; set; }

        public override string Source => Connection.Database;
        public override string Message => $"Tables Cleaned:{DatabaseMetadatasCleaned.Tables.Count}. Rows Cleaned:{DatabaseMetadatasCleaned.RowsCleanedCount}.";

        public override int EventId => SyncEventsId.MetadataCleaned.Id;
    }


    public static partial class InterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider action when a provider is cleaning metadata
        /// </summary>
        public static void OnMetadataCleaning(this BaseOrchestrator orchestrator, Action<MetadataCleaningArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a provider is cleaning metadata
        /// </summary>
        public static void OnMetadataCleaning(this BaseOrchestrator orchestrator, Func<MetadataCleaningArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a provider has cleaned metadata
        /// </summary>
        public static void OnMetadataCleaned(this BaseOrchestrator orchestrator, Action<MetadataCleanedArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider action when a provider has cleaned metadata
        /// </summary>
        public static void OnMetadataCleaned(this BaseOrchestrator orchestrator, Func<MetadataCleanedArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }

    public static partial class SyncEventsId
    {
        public static EventId MetadataCleaning => CreateEventId(3000, nameof(MetadataCleaning));
        public static EventId MetadataCleaned => CreateEventId(3050, nameof(MetadataCleaned));
    }

}