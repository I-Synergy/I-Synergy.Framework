using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Interceptors.Tests
{
    public partial class InterceptorsTests
    {
        [Ignore]
        public async Task LocalTimestamp()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" });

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup);

            var onLTLoading = 0;
            var onLTLoaded = 0;

            localOrchestrator.OnLocalTimestampLoading(tca => onLTLoading++);
            localOrchestrator.OnLocalTimestampLoaded(tca => onLTLoaded++);

            var ts = await localOrchestrator.GetLocalTimestampAsync();

            Assert.AreEqual(1, onLTLoading);
            Assert.AreEqual(1, onLTLoaded);


            _databaseHelper.DropDatabase(dbName);
        }

    }
}
