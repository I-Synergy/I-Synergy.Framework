using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Adapters;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Builders;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using System;
using System.Data.SqlClient;

namespace ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Providers
{
    public class SqlSyncChangeTrackingProvider : SqlSyncProvider
    {
        private static string _providerType;

        public SqlSyncChangeTrackingProvider() : base() { }

        public SqlSyncChangeTrackingProvider(string connectionString) : base()
            => ConnectionString = connectionString;

        public SqlSyncChangeTrackingProvider(SqlConnectionStringBuilder builder) : base()
        {
            if (string.IsNullOrEmpty(builder.ConnectionString))
                throw new Exception("You have to provide parameters to the Sql builder to be able to construct a valid connection string.");

            ConnectionString = builder.ConnectionString;
        }

        public static new string ProviderType
        {
            get
            {
                if (!string.IsNullOrEmpty(_providerType))
                    return _providerType;

                Type type = typeof(SqlSyncChangeTrackingProvider);
                _providerType = $"{type.Name}, {type}";

                return _providerType;
            }
        }

        public override DbScopeBuilder GetScopeBuilder(string scopeInfoTableName) => 
            new SqlChangeTrackingScopeBuilder(scopeInfoTableName);

        public override DbTableBuilder GetTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup) => 
            new SqlChangeTrackingTableBuilder(tableDescription, tableName, trackingTableName, setup);

        public override DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup) => 
            new SqlChangeTrackingSyncAdapter(tableDescription, tableName, trackingTableName, setup);

        public override DbBuilder GetDatabaseBuilder() => 
            new SqlChangeTrackingBuilder();
    }
}
