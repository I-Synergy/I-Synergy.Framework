using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Manager;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Adapters;
using ISynergy.Framework.Synchronization.SqlServer.Builders;
using ISynergy.Framework.Synchronization.SqlServer.Detectors;
using ISynergy.Framework.Synchronization.SqlServer.Metadata;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace ISynergy.Framework.Synchronization.SqlServer.Providers
{
    public class SqlSyncProvider : CoreProvider
    {
        private DbMetadata _dbMetadata;
        static string _providerType;

        public SqlSyncProvider() : base()
        { }

        public SqlSyncProvider(string connectionString) : base()
        {
            ConnectionString = connectionString;

            if (!string.IsNullOrEmpty(ConnectionString))
                SupportsMultipleActiveResultSets = new SqlConnectionStringBuilder(ConnectionString).MultipleActiveResultSets;

        }

        public SqlSyncProvider(SqlConnectionStringBuilder builder) : base()
        {
            if (string.IsNullOrEmpty(builder.ConnectionString))
                throw new Exception("You have to provide parameters to the Sql builder to be able to construct a valid connection string.");

            ConnectionString = builder.ConnectionString;
            SupportsMultipleActiveResultSets = builder.MultipleActiveResultSets;
        }

        public override string GetProviderTypeName() => ProviderType;

        public static string ProviderType
        {
            get
            {
                if (!string.IsNullOrEmpty(_providerType))
                    return _providerType;

                var type = typeof(SqlSyncProvider);
                _providerType = $"{type.Name}, {type}";

                return _providerType;
            }
        }

        /// <summary>
        /// Gets or sets the Metadata object which parse Sql server types
        /// </summary>
        public override DbMetadata GetMetadata()
        {
            if (_dbMetadata is null)
                _dbMetadata = new SqlDbMetadata();

            return _dbMetadata;
        }

        /// <summary>
        /// Gets a chance to make a retry connection
        /// </summary>
        public override bool ShouldRetryOn(Exception exception) => SqlServerTransientExceptionDetector.ShouldRetryOn(exception);

        public override void EnsureSyncException(SyncException syncException)
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);

                syncException.DataSource = builder.DataSource;
                syncException.InitialCatalog = builder.InitialCatalog;
            }

            // Can add more info from SqlException
            var sqlException = syncException.InnerException as SqlException;

            if (sqlException is null)
                return;

            syncException.Number = sqlException.Number;

            return;
        }

        /// <summary>
        /// Sql Server supports to be a server side provider
        /// </summary>
        public override bool CanBeServerProvider => true;

        public override (ParserName tableName, ParserName trackingName) GetParsers(SyncTable tableDescription, SyncSetup setup)
        {
            var originalTableName = ParserName.Parse(tableDescription);

            var pref = setup.TrackingTablesPrefix;
            var suf = setup.TrackingTablesSuffix;

            // be sure, at least, we have a suffix if we have empty values. 
            // othewise, we have the same name for both table and tracking table
            if (string.IsNullOrEmpty(pref) && string.IsNullOrEmpty(suf))
                suf = "_tracking";

            var trakingTableNameString = $"{pref}{originalTableName.ObjectName}{suf}";

            if (!string.IsNullOrEmpty(originalTableName.SchemaName))
                trakingTableNameString = $"{originalTableName.SchemaName}.{trakingTableNameString}";

            var trackingTableName = ParserName.Parse(trakingTableNameString);

            return (originalTableName, trackingTableName);
        }

        public override DbConnection CreateConnection() => new SqlConnection(ConnectionString);
        public override DbScopeBuilder GetScopeBuilder(string scopeInfoTableName) => new SqlScopeBuilder(scopeInfoTableName);

        /// <summary>
        /// Get the table builder. Table builder builds table, stored procedures and triggers
        /// </summary>
        public override DbTableBuilder GetTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup)
        => new SqlTableBuilder(tableDescription, tableName, trackingTableName, setup);

        public override DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup)
            => new SqlSyncAdapter(tableDescription, tableName, trackingTableName, setup);

        public override DbBuilder GetDatabaseBuilder() => new SqlBuilder();

    }
}
