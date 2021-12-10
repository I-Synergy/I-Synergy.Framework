using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Base;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class LocalOrchestratorTests : BaseTest
    {
        [Ignore]
        public async Task LocalOrchestrator_BeginSession_ShouldIncrement_SyncStage()
        {
            var options = new SyncOptions();
            var setup = new SyncSetup();
            var provider = new SqlSyncProvider();
            var onSessionBegin = false;


            var localOrchestrator = new LocalOrchestrator(_versionService, provider, options, setup);
            var ctx = localOrchestrator.GetContext();

            localOrchestrator.OnSessionBegin(args =>
            {
                Assert.AreEqual(SyncStage.BeginSession, args.Context.SyncStage);
                Assert.IsInstanceOfType(args, typeof(SessionBeginArgs));
                Assert.IsNull(args.Connection);
                Assert.IsNull(args.Transaction);
                onSessionBegin = true;
            });

            await localOrchestrator.BeginSessionAsync();

            Assert.AreEqual(SyncStage.BeginSession, ctx.SyncStage);
            Assert.IsTrue(onSessionBegin);
        }

        [Ignore]
        public async Task LocalOrchestrator_EndSession_ShouldIncrement_SyncStage()
        {
            var options = new SyncOptions();
            var setup = new SyncSetup();
            var provider = new SqlSyncProvider();
            var onSessionEnd = false;

            var localOrchestrator = new LocalOrchestrator(_versionService, provider, options, setup);
            var ctx = localOrchestrator.GetContext();

            localOrchestrator.OnSessionEnd(args =>
            {
                Assert.AreEqual(SyncStage.EndSession, args.Context.SyncStage);
                Assert.IsInstanceOfType(args, typeof(SessionEndArgs));
                Assert.IsNull(args.Connection);
                Assert.IsNull(args.Transaction);
                onSessionEnd = true;
            });

            await localOrchestrator.EndSessionAsync();

            Assert.AreEqual(SyncStage.EndSession, ctx.SyncStage);
            Assert.IsTrue(onSessionEnd);
        }

        [Ignore]
        public async Task LocalOrchestrator_Interceptor_ShouldSerialize()
        {
            var dbNameSrv = _databaseHelper.GetRandomName("tcp_lo_srv");
            await _databaseHelper.CreateDatabaseAsync(dbNameSrv, true);

            var dbNameCli = _databaseHelper.GetRandomName("tcp_lo_cli");
            await _databaseHelper.CreateDatabaseAsync(dbNameCli, true);

            var csServer = _databaseHelper.GetConnectionString(dbNameSrv);
            var serverProvider = new SqlSyncProvider(csServer);

            var csClient = _databaseHelper.GetConnectionString(dbNameCli);
            var clientProvider = new SqlSyncProvider(csClient);

            await new DataContext((dbNameSrv, serverProvider), true, false).Database.EnsureCreatedAsync();
            await new DataContext((dbNameCli, clientProvider), true, false).Database.EnsureCreatedAsync();

            var scopeName = "scopesnap1";

            // Make a first sync to be sure everything is in place
            var agent = new SyncAgent(_versionService, clientProvider, serverProvider, this.Tables, scopeName);

            // Making a first sync, will initialize everything we need
            var r = await agent.SynchronizeAsync();

            // Get the orchestrators
            var localOrchestrator = agent.LocalOrchestrator;
            var remoteOrchestrator = agent.RemoteOrchestrator;

            // Server side : Create a product category and a product
            // Create a productcategory item
            // Create a new product on server
            var productId = Guid.NewGuid();
            var productName = _databaseHelper.GetRandomName();
            var productNumber = productName.ToUpperInvariant().Substring(0, 10);

            var productCategoryName = _databaseHelper.GetRandomName();
            var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

            using (var ctx = new DataContext((dbNameCli, clientProvider)))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);

                await ctx.SaveChangesAsync();
            }


            // Defining options with Batchsize to enable serialization on disk
            agent.Options.BatchSize = 2000;

            var myRijndael = new RijndaelManaged();
            myRijndael.GenerateKey();
            myRijndael.GenerateIV();

            // Encrypting data on disk
            localOrchestrator.OnSerializingSet(ssa =>
            {
                // Create an encryptor to perform the stream transform.
                var encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            var strSet = JsonConvert.SerializeObject(ssa.Set);
                            swEncrypt.Write(strSet);
                        }
                        ssa.Result = msEncrypt.ToArray();
                    }
                }
            });

            // Get changes to be populated to the server.
            // Should intercept the OnSerializing interceptor
            var changes = await localOrchestrator.GetChangesAsync();
        }
    }
}
