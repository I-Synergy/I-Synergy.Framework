using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
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
        static string providerType;

        public SqlSyncChangeTrackingProvider() : base() { }

        public SqlSyncChangeTrackingProvider(string connectionString) : base()
            => this.ConnectionString = connectionString;

        public SqlSyncChangeTrackingProvider(SqlConnectionStringBuilder builder) : base()
        {
            if (string.IsNullOrEmpty(builder.ConnectionString))
                throw new Exception("You have to provide parameters to the Sql builder to be able to construct a valid connection string.");

            this.ConnectionString = builder.ConnectionString;
        }

        public static new string ProviderType
        {
            get
            {
                if (!string.IsNullOrEmpty(providerType))
                    return providerType;

                Type type = typeof(SqlSyncChangeTrackingProvider);
                providerType = $"{type.Name}, {type}";

                return providerType;
            }

        }
        public override DbScopeBuilder GetScopeBuilder(string scopeInfoTableName) => new SqlChangeTrackingScopeBuilder(scopeInfoTableName);

        public override DbTableBuilder GetTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup)
            => new SqlChangeTrackingTableBuilder(tableDescription, tableName, trackingTableName, setup);

        public override DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup)
            => new SqlChangeTrackingSyncAdapter(tableDescription, tableName, trackingTableName, setup);

        public override DbBuilder GetDatabaseBuilder() => new SqlChangeTrackingBuilder();

    }
}
