using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Manager;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Data;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Core.Abstractions
{
    public interface IProvider
    {
        int BulkBatchMaxLinesCount { get; set; }
        bool CanBeServerProvider { get; }
        string ConnectionString { get; set; }
        IsolationLevel IsolationLevel { get; set; }
        BaseOrchestrator Orchestrator { get; set; }
        bool SupportsMultipleActiveResultSets { get; set; }
        DbConnection CreateConnection();
        void EnsureSyncException(SyncException syncException);
        DbBuilder GetDatabaseBuilder();
        DbMetadata GetMetadata();
        (ParserName tableName, ParserName trackingName) GetParsers(SyncTable tableDescription, SyncSetup setup);
        string GetProviderTypeName();
        DbScopeBuilder GetScopeBuilder(string scopeInfoTableName);
        DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup);
        DbTableBuilder GetTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup);
        void OnConnectionClosed(DbConnection connection);
        void OnConnectionOpened(DbConnection connection);
        bool ShouldRetryOn(Exception exception);
    }
}