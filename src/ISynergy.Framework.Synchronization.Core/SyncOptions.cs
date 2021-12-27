using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ISynergy.Framework.Synchronization.Core
{
    /// <summary>
    /// This class determines all the options you can set on Client and Server, that could potentially be different
    /// </summary>
    public class SyncOptions
    {
        /// <summary>
        /// Default name if nothing is specified for the scope inf table, stored on the client db
        /// On the server side, server scope table is prefixed with _server and history table with _history
        /// </summary>
        public const string DefaultScopeInfoTableName = "scope_info";

        /// <summary>
        /// Default scope name if not specified
        /// </summary>
        public const string DefaultScopeName = "DefaultScope";
        private int batchSize;

        /// <summary>
        /// Gets or Sets the directory used for batch mode.
        /// Default value is [User Temp Path]/[I-Synergy.Synchronization]
        /// </summary>
        public string BatchDirectory { get; set; }

        /// <summary>
        /// Gets or Sets the directory where snapshots are stored.
        /// This value could be overwritten by server is used in an http mode
        /// </summary>
        public string SnapshotsDirectory { get; set; }

        /// <summary>
        /// Gets or Sets the size used (approximatively in kb, depending on the serializer) for each batch file, in batch mode. 
        /// Default is 5000 
        /// Min value is 1000
        /// </summary>
        public int BatchSize
        {
            get => batchSize;
            set => batchSize = Math.Max(value, 100);
        }

        /// <summary>
        /// Gets or Sets the log level for sync operations. Default value is false.
        /// </summary>
        public bool UseVerboseErrors { get; set; }

        /// <summary>
        /// Gets or Sets if we should clean tracking table metadatas.
        /// </summary>
        public bool CleanMetadatas { get; set; }

        /// <summary>
        /// Gets or Sets if we should cleaning tmp dir files after sync.
        /// </summary>
        public bool CleanFolder { get; set; }

        /// <summary>
        /// Gets or Sets if we should disable constraints before making apply changes 
        /// Default value is false
        /// </summary>

        // trying false by default : https://github.com/Mimetis/Dotmim.Sync/discussions/453#discussioncomment-380530
        public bool DisableConstraintsOnApplyChanges { get; set; }

        /// <summary>
        /// Gets or Sets the scope_info table name. Default is scope_info
        /// On the server side, server scope table is prefixed with _server and history table with _history
        /// </summary>
        public string ScopeInfoTableName { get; set; }

        /// <summary>
        /// Gets or Sets the default conflict resolution policy. This value could potentially be ovewritten and replaced by the server
        /// </summary>
        public ConflictResolutionPolicy ConflictResolutionPolicy { get; set; }

        /// <summary>
        /// Gets or Sets the default logger used for logging purpose
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or Sets the local serializer used to buffer rows on disk
        /// </summary>
        public ILocalSerializerFactory LocalSerializerFactory { get; set; }

        /// <summary>
        /// Gets the Progress Level
        /// </summary>
        public SyncProgressLevel ProgressLevel { get; set; }


        /// <summary>
        /// Create a new instance of options with default values
        /// </summary>
        public SyncOptions()
        {
            BatchDirectory = GetDefaultUserBatchDiretory();
            BatchSize = 5000;
            CleanMetadatas = true;
            CleanFolder = true;
            UseVerboseErrors = false;
            DisableConstraintsOnApplyChanges = false;
            ScopeInfoTableName = DefaultScopeInfoTableName;
            ConflictResolutionPolicy = ConflictResolutionPolicy.ServerWins;
            Logger = new LoggerFactory().CreateLogger(nameof(SyncOptions));
            ProgressLevel = SyncProgressLevel.Information;
            LocalSerializerFactory = new LocalJsonSerializerFactory();
        }


        /// <summary>
        /// Get the default Batch directory full path ([User Temp Path]/[I-Synergy.Synchronization])
        /// </summary>
        public static string GetDefaultUserBatchDiretory() => Path.Combine(GetDefaultUserTempPath(), GetDefaultUserBatchDirectoryName());

        /// <summary>
        /// Get the default user tmp folder
        /// </summary>
        public static string GetDefaultUserTempPath() => Path.GetTempPath();

        /// <summary>
        /// Get the default sync tmp folder name
        /// </summary>
        public static string GetDefaultUserBatchDirectoryName() => "I-Synergy.Synchronization";
    }
}
