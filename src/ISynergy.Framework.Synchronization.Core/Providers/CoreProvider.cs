﻿using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Manager;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace ISynergy.Framework.Synchronization.Core.Providers
{
    /// <summary>
    /// Core provider : should be implemented by any server / client provider
    /// </summary>
    public abstract partial class CoreProvider : IProvider
    {
        /// <summary>
        /// Gets the reference to the orchestrator owner of this instance
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public BaseOrchestrator Orchestrator { get; set; }

        /// <summary>
        /// Connection is opened. this method is called before any interceptors
        /// </summary>
        public virtual void OnConnectionOpened(DbConnection connection) { }

        /// <summary>
        /// Connection is closed. this method is called after all interceptors
        /// </summary>
        public virtual void OnConnectionClosed(DbConnection connection) { }

        /// <summary>
        /// Create a new instance of the implemented Connection provider
        /// </summary>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Get Database Builder which can create object at the database level
        /// </summary>
        /// <returns></returns>
        public abstract DbBuilder GetDatabaseBuilder();

        /// <summary>
        /// Get a table builder helper which can create object at the table level
        /// </summary>
        /// <returns></returns>
        public abstract DbTableBuilder GetTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup);

        /// <summary>
        /// Get sync adapter which can executes all the commands needed for a complete sync
        /// </summary>
        public abstract DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup);

        /// <summary>
        /// Create a Scope Builder, which can create scope table, and scope config
        /// </summary>
        public abstract DbScopeBuilder GetScopeBuilder(string scopeInfoTableName);

        /// <summary>
        /// Gets or sets the metadata resolver (validating the columns definition from the data store)
        /// </summary>
        public abstract DbMetadata GetMetadata();

        /// <summary>
        /// Get the provider type name
        /// </summary>
        public abstract string GetProviderTypeName();

        /// <summary>
        /// Gets or sets the connection string used by the implemented provider
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets a boolean indicating if the provider can be a server side provider
        /// </summary>
        public abstract bool CanBeServerProvider { get; }

        /// <summary>
        /// Gets the default isolation level used during transaction
        /// </summary>
        public virtual IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Gets or Sets the number of line for every batch bulk operations
        /// </summary>
        public virtual int BulkBatchMaxLinesCount { get; set; } = 10000;

        /// <summary>
        /// Gets or Sets if the provider supports multi results sets on the same connection
        /// </summary>
        public virtual bool SupportsMultipleActiveResultSets { get; set; } = false;

        /// <summary>
        /// Get naming tables
        /// </summary>
        public abstract (ParserName tableName, ParserName trackingName) GetParsers(SyncTable tableDescription, SyncSetup setup);

        /// <summary>
        /// Let a chance to provider to enrich SyncExecption
        /// </summary>
        public virtual void EnsureSyncException(SyncException syncException) { }

        /// <summary>
        /// Let's a chance to retry on error if connection has been refused.
        /// </summary>
        public virtual bool ShouldRetryOn(Exception exception) => false;
    }
}
