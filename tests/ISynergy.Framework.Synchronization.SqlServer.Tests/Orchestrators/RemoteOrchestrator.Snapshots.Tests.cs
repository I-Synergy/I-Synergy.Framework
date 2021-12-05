using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Parameters;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class RemoteOrchestratorTests
    {
        private readonly DatabaseHelper _databaseHelper;

        public string[] Tables => new string[]
        {
            "SalesLT.ProductCategory", "SalesLT.ProductModel", "SalesLT.Product", "Employee", "Customer", "Address", "CustomerAddress", "EmployeeAddress",
            "SalesLT.SalesOrderHeader", "SalesLT.SalesOrderDetail", "Posts", "Tags", "PostTag",
            "PricesList", "PricesListCategory", "PricesListDetail"
        };

        public RemoteOrchestratorTests()
        {
            _databaseHelper = new DatabaseHelper();
        }

        [TestMethod]
        public async Task RemoteOrchestrator_CreateSnapshot_CheckInterceptors()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_srv");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var serverProvider = new SqlSyncProvider(cs);
            
            var ctx = new DataContext((dbName, serverProvider), true, true);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scopesnap1";
            var onSnapshotCreating = false;
            var onSnapshotCreated = false;

            // snapshot directory
            var snapshotDirctoryName = _databaseHelper.GetRandomName();
            var snapshotDirectory = Path.Combine(Environment.CurrentDirectory, snapshotDirctoryName);

            var options = new SyncOptions
            {
                SnapshotsDirectory = snapshotDirectory,
                BatchSize = 200
            };

            var setup = new SyncSetup(Tables);

            var remoteOrchestrator = new RemoteOrchestrator(serverProvider, options, setup, scopeName);

            // Assert on connection and transaction interceptors
            BaseOrchestratorTests.AssertConnectionAndTransaction(remoteOrchestrator, SyncStage.SnapshotCreating);

            remoteOrchestrator.OnSnapshotCreating(args =>
            {
                Assert.IsInstanceOfType(args, typeof(SnapshotCreatingArgs));
                Assert.AreEqual(SyncStage.SnapshotCreating, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNotNull(args.Transaction);
                Assert.AreEqual(ConnectionState.Open, args.Connection.State);
                Assert.IsNotNull(args.Schema);
                Assert.AreEqual(snapshotDirectory, args.SnapshotDirectory);
                Assert.AreNotEqual(0, args.Timestamp);

                onSnapshotCreating = true;
            });
            remoteOrchestrator.OnSnapshotCreated(args =>
            {
                Assert.IsInstanceOfType(args, typeof(SnapshotCreatedArgs));
                Assert.AreEqual(SyncStage.SnapshotCreating, args.Context.SyncStage);
                Assert.AreEqual(scopeName, args.Context.ScopeName);
                Assert.IsNotNull(args.Connection);
                Assert.IsNull(args.Transaction);
                Assert.IsNotNull(args.BatchInfo);

                var finalDirectoryFullName = Path.Combine(snapshotDirectory, scopeName);

                Assert.AreEqual(finalDirectoryFullName, args.BatchInfo.DirectoryRoot);
                Assert.AreEqual("ALL", args.BatchInfo.DirectoryName);
                Assert.AreEqual(1, args.BatchInfo.BatchPartsInfo.Count);
                Assert.AreEqual(16, args.BatchInfo.BatchPartsInfo[0].Tables.Length);
                Assert.IsTrue(args.BatchInfo.BatchPartsInfo[0].IsLastBatch);

                onSnapshotCreated = true;
            });

            var bi = await remoteOrchestrator.CreateSnapshotAsync();

            Assert.AreEqual(SyncStage.SnapshotCreating, remoteOrchestrator.GetContext().SyncStage);

            Assert.IsTrue(onSnapshotCreating);
            Assert.IsTrue(onSnapshotCreated);


            var dbNameCli = _databaseHelper.GetRandomName("tcp_lo_cli");
            await _databaseHelper.CreateDatabaseAsync(dbNameCli, true);

            var csClient = _databaseHelper.GetConnectionString(dbNameCli);
            var clientProvider = new SqlSyncProvider(csClient);


            // Make a first sync to be sure everything is in place
            var agent = new SyncAgent(clientProvider, serverProvider, options, setup, scopeName);

            var onSnapshotApplying = false;
            var onSnapshotApplied = false;



            agent.LocalOrchestrator.OnSnapshotApplying(saa =>
            {
                onSnapshotApplying = true;
            });

            agent.LocalOrchestrator.OnSnapshotApplied(saa =>
            {
                onSnapshotApplied = true;
            });


            // Making a first sync, will initialize everything we need
            await agent.SynchronizeAsync();

            Assert.IsTrue(onSnapshotApplying);
            Assert.IsTrue(onSnapshotApplied);



            _databaseHelper.DropDatabase(dbName);
        }


        [TestMethod]
        public async Task RemoteOrchestrator_CreateSnapshot_CheckBatchInfo()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, true);
            await ctx.Database.EnsureCreatedAsync();

            var rowsCount = GetServerDatabaseRowsCount((dbName, sqlProvider));

            var scopeName = "scopesnap2";

            // snapshot directory
            var snapshotDirctoryName = _databaseHelper.GetRandomName();
            var snapshotDirectory = Path.Combine(Environment.CurrentDirectory, snapshotDirctoryName);

            var options = new SyncOptions
            {
                SnapshotsDirectory = snapshotDirectory,
                BatchSize = 200
            };

            var setup = new SyncSetup(Tables);
            var provider = new SqlSyncProvider(cs);

            var orchestrator = new RemoteOrchestrator(provider, options, setup, scopeName);

            var bi = await orchestrator.CreateSnapshotAsync();

            var finalDirectoryFullName = Path.Combine(snapshotDirectory, scopeName);

            Assert.IsNotNull(bi);
            Assert.AreEqual(finalDirectoryFullName, bi.DirectoryRoot);
            Assert.AreEqual("ALL", bi.DirectoryName);
            Assert.AreEqual(1, bi.BatchPartsInfo.Count);
            Assert.AreEqual(16, bi.BatchPartsInfo[0].Tables.Length);
            Assert.IsTrue(bi.BatchPartsInfo[0].IsLastBatch);
            Assert.AreEqual(rowsCount, bi.RowsCount);

            // Check summary.json exists.
            var summaryFile = Path.Combine(bi.GetDirectoryFullPath(), "summary.json");
            var summaryString = new StreamReader(summaryFile).ReadToEnd();
            var summaryObject = JObject.Parse(summaryString);

            Assert.IsNotNull(summaryObject);
            string summaryDirname = (string)summaryObject["dirname"];
            Assert.IsNotNull(summaryDirname);
            Assert.AreEqual("ALL", summaryDirname);

            string summaryDir = (string)summaryObject["dir"];
            Assert.IsNotNull(summaryDir);
            Assert.AreEqual(finalDirectoryFullName, summaryDir);

            Assert.AreEqual(1, summaryObject["parts"].Count());
            Assert.IsNotNull(summaryObject["parts"][0]["file"]);
            Assert.IsNotNull(summaryObject["parts"][0]["index"]);
            Assert.AreEqual(0, (int)summaryObject["parts"][0]["index"]);
            Assert.IsNotNull(summaryObject["parts"][0]["last"]);
            Assert.IsTrue((bool)summaryObject["parts"][0]["last"]);
            Assert.AreEqual(16, summaryObject["parts"][0]["tables"].Count());

            _databaseHelper.DropDatabase(dbName);
        }


        [TestMethod]
        public async Task RemoteOrchestrator_CreateSnapshot_WithParameters_CheckBatchInfo()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, true);
            await ctx.Database.EnsureCreatedAsync();

            // snapshot directory
            var snapshotDirctoryName = _databaseHelper.GetRandomName();
            var snapshotDirectory = Path.Combine(Environment.CurrentDirectory, snapshotDirctoryName);

            var options = new SyncOptions
            {
                SnapshotsDirectory = snapshotDirectory,
                BatchSize = 200
            };

            var setup = new SyncSetup(Tables);
            var provider = new SqlSyncProvider(cs);

            setup.Filters.Add("Customer", "CompanyName");

            var addressCustomerFilter = new SetupFilter("CustomerAddress");
            addressCustomerFilter.AddParameter("CompanyName", "Customer");
            addressCustomerFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressCustomerFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup.Filters.Add(addressCustomerFilter);

            var addressFilter = new SetupFilter("Address");
            addressFilter.AddParameter("CompanyName", "Customer");
            addressFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "AddressId", "Address", "AddressId");
            addressFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            addressFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup.Filters.Add(addressFilter);

            var orderHeaderFilter = new SetupFilter("SalesOrderHeader");
            orderHeaderFilter.AddParameter("CompanyName", "Customer");
            orderHeaderFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "CustomerId", "SalesOrderHeader", "CustomerId");
            orderHeaderFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            orderHeaderFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup.Filters.Add(orderHeaderFilter);

            var orderDetailsFilter = new SetupFilter("SalesOrderDetail");
            orderDetailsFilter.AddParameter("CompanyName", "Customer");
            orderDetailsFilter.AddJoin(Join.Left, "SalesOrderHeader").On("SalesOrderDetail", "SalesOrderID", "SalesOrderHeader", "SalesOrderID");
            orderDetailsFilter.AddJoin(Join.Left, "CustomerAddress").On("CustomerAddress", "CustomerId", "SalesOrderHeader", "CustomerId");
            orderDetailsFilter.AddJoin(Join.Left, "Customer").On("CustomerAddress", "CustomerId", "Customer", "CustomerId");
            orderDetailsFilter.AddWhere("CompanyName", "Customer", "CompanyName");
            setup.Filters.Add(orderDetailsFilter);


            var orchestrator = new RemoteOrchestrator(provider, options, setup);

            var parameters = new SyncParameters();
            var p1 = new SyncParameter("CompanyName", "A Bike Store");
            parameters.Add(p1);

            var bi = await orchestrator.CreateSnapshotAsync(parameters);

            var finalDirectoryFullName = Path.Combine(snapshotDirectory, SyncOptions.DefaultScopeName);

            Assert.IsNotNull(bi);
            Assert.AreEqual(finalDirectoryFullName, bi.DirectoryRoot);
            Assert.AreEqual("CompanyName_ABikeStore", bi.DirectoryName);
            Assert.AreEqual(1, bi.BatchPartsInfo.Count);
            Assert.AreEqual(16, bi.BatchPartsInfo[0].Tables.Length);
            Assert.IsTrue(bi.BatchPartsInfo[0].IsLastBatch);

            // Check summary.json exists.
            var summaryFile = Path.Combine(bi.GetDirectoryFullPath(), "summary.json");
            var summaryString = new StreamReader(summaryFile).ReadToEnd();
            var summaryObject = JObject.Parse(summaryString);

            Assert.IsNotNull(summaryObject);
            string summaryDirname = (string)summaryObject["dirname"];
            Assert.IsNotNull(summaryDirname);
            Assert.AreEqual("CompanyName_ABikeStore", summaryDirname);

            string summaryDir = (string)summaryObject["dir"];
            Assert.IsNotNull(summaryDir);
            Assert.AreEqual(finalDirectoryFullName, summaryDir);

            Assert.AreEqual(1, summaryObject["parts"].Count());
            Assert.IsNotNull(summaryObject["parts"][0]["file"]);
            Assert.IsNotNull(summaryObject["parts"][0]["index"]);
            Assert.AreEqual(0, (int)summaryObject["parts"][0]["index"]);
            Assert.IsNotNull(summaryObject["parts"][0]["last"]);
            Assert.IsTrue((bool)summaryObject["parts"][0]["last"]);
            Assert.AreEqual(16, summaryObject["parts"][0]["tables"].Count());

            _databaseHelper.DropDatabase(dbName);
        }



        [TestMethod]
        public async Task RemoteOrchestrator_CreateSnapshot_ShouldFail_If_MissingMandatoriesOptions()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);
            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);
            var ctx = new DataContext((dbName, sqlProvider), true, true);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scopesnap";

            // snapshot directory
            var snapshotDirctoryName = _databaseHelper.GetRandomName();
            var snapshotDirectory = Path.Combine(Environment.CurrentDirectory, snapshotDirctoryName);

            var options = new SyncOptions { SnapshotsDirectory = snapshotDirectory };

            var setup = new SyncSetup(Tables);
            var provider = new SqlSyncProvider(cs);

            var orchestrator = new RemoteOrchestrator(provider, options, setup, scopeName);
            var se = await Assert.ThrowsExceptionAsync<SyncException>(() => orchestrator.CreateSnapshotAsync());

            Assert.AreEqual(SyncStage.SnapshotCreating, se.SyncStage);
            Assert.AreEqual(SyncSide.ServerSide, se.Side);
            Assert.AreEqual("SnapshotMissingMandatariesOptionsException", se.TypeName);

            options = new SyncOptions { BatchSize = 2000 };
            orchestrator = new RemoteOrchestrator(provider, options, setup, scopeName);
            se = await Assert.ThrowsExceptionAsync<SyncException>(() => orchestrator.CreateSnapshotAsync());

            Assert.AreEqual(SyncStage.SnapshotCreating, se.SyncStage);
            Assert.AreEqual(SyncSide.ServerSide, se.Side);
            Assert.AreEqual("SnapshotMissingMandatariesOptionsException", se.TypeName);

            options = new SyncOptions { };
            orchestrator = new RemoteOrchestrator(provider, options, setup, scopeName);
            se = await Assert.ThrowsExceptionAsync<SyncException>(() => orchestrator.CreateSnapshotAsync());

            Assert.AreEqual(SyncStage.SnapshotCreating, se.SyncStage);
            Assert.AreEqual(SyncSide.ServerSide, se.Side);
            Assert.AreEqual("SnapshotMissingMandatariesOptionsException", se.TypeName);

            _databaseHelper.DropDatabase(dbName);
        }


        public int GetServerDatabaseRowsCount((string DatabaseName, CoreProvider Provider) t)
        {
            int totalCountRows = 0;

            using (var serverDbCtx = new DataContext(t))
            {
                totalCountRows += serverDbCtx.Address.Count();
                totalCountRows += serverDbCtx.Customer.Count();
                totalCountRows += serverDbCtx.CustomerAddress.Count();
                totalCountRows += serverDbCtx.Employee.Count();
                totalCountRows += serverDbCtx.EmployeeAddress.Count();
                totalCountRows += serverDbCtx.Log.Count();
                totalCountRows += serverDbCtx.Posts.Count();
                totalCountRows += serverDbCtx.PostTag.Count();
                totalCountRows += serverDbCtx.PricesList.Count();
                totalCountRows += serverDbCtx.PricesListCategory.Count();
                totalCountRows += serverDbCtx.PricesListDetail.Count();
                totalCountRows += serverDbCtx.Product.Count();
                totalCountRows += serverDbCtx.ProductCategory.Count();
                totalCountRows += serverDbCtx.ProductModel.Count();
                totalCountRows += serverDbCtx.SalesOrderDetail.Count();
                totalCountRows += serverDbCtx.SalesOrderHeader.Count();
                //totalCountRows += serverDbCtx.Sql.Count();
                totalCountRows += serverDbCtx.Tags.Count();
            }

            return totalCountRows;
        }

    }
}
