using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Metadata;
using ISynergy.Framework.Synchronization.Core.Model.Parsers;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Data.Common;

namespace ISynergy.Framework.Synchronization.Client.Providers
{

    /// <summary>
    /// This provider is only here to be able to have a valid WebClientOrchestrator
    /// </summary>
    public class ClientCoreProvider : CoreProvider
    {
        public override DbMetadata GetMetadata() => throw new NotImplementedException();

        public override string GetProviderTypeName() => "Fancy";

        public override bool SupportBulkOperations => throw new NotImplementedException();

        public override bool CanBeServerProvider => true;

        public override DbConnection CreateConnection() => throw new NotImplementedException();

        public override DbBuilder GetDatabaseBuilder() => throw new NotImplementedException();
        public override (ParserName tableName, ParserName trackingName) GetParsers(SyncTable tableDescription, SyncSetup setup) => throw new NotImplementedException();
        public override DbScopeBuilder GetScopeBuilder(string scope) => throw new NotImplementedException();
        public override DbSyncAdapter GetSyncAdapter(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup) => throw new NotImplementedException();
        public override DbTableBuilder GetTableBuilder(SyncTable tableDescription, ParserName tableName, ParserName trackingTableName, SyncSetup setup) => throw new NotImplementedException();
    }
}
