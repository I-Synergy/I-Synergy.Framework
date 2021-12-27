﻿using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using ISynergy.Framework.AspNetCore.Synchronization.Tests.Data;
using ISynergy.Framework.AspNetCore.Synchronization.Tests.Serializers;
using ISynergy.Framework.AspNetCore.Synchronization.Tests.TestServer;
using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Parameter;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Tests.Http.Base
{
    public abstract class BaseHttpFilterTests : BaseTests
    {
        /// <summary>
        /// Gets the sync filtered tables involved in the tests
        /// </summary>
        public abstract SyncSetup FilterSetup { get; }

        /// <summary>
        /// Gets the filter parameter value
        /// </summary>
        public abstract SyncParameters FilterParameters { get; }

        /// <summary>
        /// For each test, Create a server database and some clients databases, depending on ProviderType provided in concrete class
        /// </summary>
        public BaseHttpFilterTests()
            : base()
        {
            // Since we are creating a lot of databases
            // each database will have its own pool
            // Droping database will not clear the pool associated
            // So clear the pools on every start of a new test
            _databaseHelper.ClearAllPools();

            // get the server provider (and db created) without seed
            var serverDatabaseName = _databaseHelper.GetRandomName("httpfilt_sv_");

            // create remote orchestrator
            var serverProvider = this.CreateProvider(this.ServerType, serverDatabaseName);

            // create web remote orchestrator
            WebServerOrchestrator = new WebServerOrchestrator(_versionService, serverProvider, new SyncOptions(), new SyncSetup());

            // public property
            Server = (serverDatabaseName, this.ServerType, serverProvider);

            // Create a kestrel server
            kestrel = new KestrelTestServer(this.WebServerOrchestrator, this.UseFiddler);

            // start server and get uri
            ServiceUri = this.kestrel.Run();

            // Get all clients providers
            Clients = new List<(string DatabaseName, ProviderType ProviderType, CoreProvider Provider)>(this.ClientsType.Count);

            // Generate Client database
            foreach (var clientType in this.ClientsType)
            {
                var dbCliName = _databaseHelper.GetRandomName("httpfilt_cli_");
                var localProvider = this.CreateProvider(clientType, dbCliName);

                this.Clients.Add((dbCliName, clientType, localProvider));
            }
        }

        /// <summary>
        /// Drop all databases used for the tests
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            _databaseHelper.DropDatabase(Server.DatabaseName);

            foreach (var client in Clients)
                _databaseHelper.DropDatabase(client.DatabaseName);
        }

        [TestMethod]
        public virtual async Task SchemaIsCreated()
        {
            // create a server db without seed
            await this.EnsureDatabaseSchemaAndSeedAsync(Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService));

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
            }
        }

        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public virtual async Task RowsCount(SyncOptions options)
        {
            // create a server db and seed it
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;

            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
            }
        }


        /// <summary>
        /// Insert two rows on server, should be correctly sync on all clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Insert_TwoTables_FromServer(SyncOptions options)
        {
            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Create a new address & customer address on server
            using (var serverDbCtx = new DataContext(this.Server))
            {
                var addressLine1 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var newAddress = new Address { AddressLine1 = addressLine1 };

                serverDbCtx.Address.Add(newAddress);
                await serverDbCtx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "OTH"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress);
                await serverDbCtx.SaveChangesAsync();
            }

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }
        }


        /// <summary>
        /// Insert four rows on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Insert_TwoTables_FromClient(SyncOptions options)
        {
            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;


            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Insert 4 lines on each client
            foreach (var client in Clients)
            {
                var soh = new SalesOrderHeader
                {
                    SalesOrderNumber = $"SO-99999",
                    RevisionNumber = 1,
                    Status = 5,
                    OnlineOrderFlag = true,
                    PurchaseOrderNumber = "PO348186287",
                    AccountNumber = "10-4020-000609",
                    CustomerId = Guid.NewGuid(),
                    ShipToAddressId = 4,
                    BillToAddressId = 5,
                    ShipMethod = "CAR TRANSPORTATION",
                    SubTotal = 6530.35M,
                    TaxAmt = 70.4279M,
                    Freight = 22.0087M,
                    TotalDue = 6530.35M + 70.4279M + 22.0087M
                };
                using var ctx = new DataContext(client, this.UseFallbackSchema);
                var productId = ctx.Product.First().ProductId;

                var sod1 = new SalesOrderDetail { OrderQty = 1, ProductId = productId, UnitPrice = 3578.2700M };
                var sod2 = new SalesOrderDetail { OrderQty = 2, ProductId = productId, UnitPrice = 44.5400M };
                var sod3 = new SalesOrderDetail { OrderQty = 2, ProductId = productId, UnitPrice = 1431.5000M };

                soh.SalesOrderDetail.Add(sod1);
                soh.SalesOrderDetail.Add(sod2);
                soh.SalesOrderDetail.Add(sod3);

                ctx.SalesOrderHeader.Add(soh);
                await ctx.SaveChangesAsync();

            }

            // Sync all clients
            // First client  will upload 4 lines and will download nothing
            // Second client will upload 4 lines and will download 8 lines
            // thrid client  will upload 4 lines and will download 12 lines
            var download = 0;
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                //Assert.AreEqual(download, s.TotalChangesDownloaded);
                Assert.AreEqual(4, s.TotalChangesUploaded);
                //Assert.AreEqual(0, s.TotalSyncConflicts);
                download += 4;
            }

            // Now sync again to be sure all clients have all lines
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                await agent.SynchronizeAsync();
            }
        }

        /// <summary>
        /// Insert one row in two tables on server, should be correctly sync on all clients
        /// </summary>
        [TestMethod]
        public async Task Snapshot_Initialize()
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // snapshot directory
            var snapshotDirctory = _databaseHelper.GetRandomName();
            var directory = Path.Combine(Environment.CurrentDirectory, snapshotDirctory);
            // ----------------------------------
            // Setting correct options for sync agent to be able to reach snapshot
            // ----------------------------------
            var options = new SyncOptions
            {
                SnapshotsDirectory = directory,
                BatchSize = 200
            };

            // ----------------------------------
            // Create a snapshot
            // ----------------------------------
            var remoteOrchestrator = new RemoteOrchestrator(_versionService, Server.Provider, options, this.FilterSetup);
            await remoteOrchestrator.CreateSnapshotAsync(this.FilterParameters);


            // ----------------------------------
            // Add rows on server AFTER snapshot
            // ----------------------------------
            // Create a new address & customer address on server
            using (var serverDbCtx = new DataContext(this.Server))
            {
                var addressLine1 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var newAddress = new Address { AddressLine1 = addressLine1 };

                serverDbCtx.Address.Add(newAddress);
                await serverDbCtx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "OTH"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress);
                await serverDbCtx.SaveChangesAsync();
            }

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;
            this.WebServerOrchestrator.Options.SnapshotsDirectory = directory;
            this.WebServerOrchestrator.Options.BatchSize = 200;

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var snapshotApplying = 0;
                var snapshotApplied = 0;

                agent.LocalOrchestrator.OnSnapshotApplying(saa => snapshotApplying++);
                agent.LocalOrchestrator.OnSnapshotApplied(saa => snapshotApplied++);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                Assert.AreEqual(1, snapshotApplying);
                Assert.AreEqual(1, snapshotApplied);
            }
        }

        /// <summary>
        /// Insert two rows on server, should be correctly sync on all clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task CustomSerializer_MessagePack(SyncOptions options)
        {
            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;
            this.WebServerOrchestrator.SerializerFactories.Add(new CustomMessagePackSerializerFactory());

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter and serializer message pack
                var webClientOrchestrator = new WebClientOrchestrator(this.ServiceUri, _versionService);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Create a new address & customer address on server
            using (var serverDbCtx = new DataContext(this.Server))
            {
                var addressLine1 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var newAddress = new Address { AddressLine1 = addressLine1 };

                serverDbCtx.Address.Add(newAddress);
                await serverDbCtx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "OTH"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress);
                await serverDbCtx.SaveChangesAsync();
            }

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter and serializer message pack
                var webClientOrchestrator = new WebClientOrchestrator(this.ServiceUri, _versionService);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }
        }

        /// <summary>
        /// Insert one row in two tables on server, should be correctly sync on all clients
        /// </summary>
        [TestMethod]
        public async Task Snapshot_ShouldNot_Delete_Folders()
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // ----------------------------------
            // Setting correct options for sync agent to be able to reach snapshot
            // ----------------------------------
            var snapshotDirctory = _databaseHelper.GetRandomName();
            var directory = Path.Combine(Environment.CurrentDirectory, snapshotDirctory);
            var serverOptions = new SyncOptions
            {
                SnapshotsDirectory = directory,
                BatchDirectory = Path.Combine(SyncOptions.GetDefaultUserBatchDiretory(), "srv"),
                BatchSize = 200
            };

            var clientOptions = new SyncOptions
            {
                BatchDirectory = Path.Combine(SyncOptions.GetDefaultUserBatchDiretory(), "cli"),
                BatchSize = 200
            };

            // ----------------------------------
            // Create a snapshot
            // ----------------------------------
            var remoteOrchestrator = new RemoteOrchestrator(_versionService, Server.Provider, serverOptions, this.FilterSetup);

            // getting snapshot directory names
            var (rootDirectory, nameDirectory) = await remoteOrchestrator.GetSnapshotDirectoryAsync(this.FilterParameters).ConfigureAwait(false);

            Assert.IsFalse(Directory.Exists(rootDirectory));
            Assert.IsFalse(Directory.Exists(Path.Combine(rootDirectory, nameDirectory)));

            await remoteOrchestrator.CreateSnapshotAsync(this.FilterParameters);

            Assert.IsTrue(Directory.Exists(rootDirectory));
            Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, nameDirectory)));


            // ----------------------------------
            // Add rows on server AFTER snapshot
            // ----------------------------------
            // Create a new address & customer address on server
            using (var serverDbCtx = new DataContext(this.Server))
            {
                var addressLine1 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var newAddress = new Address { AddressLine1 = addressLine1 };

                serverDbCtx.Address.Add(newAddress);
                await serverDbCtx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "OTH"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress);
                await serverDbCtx.SaveChangesAsync();
            }

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;
            this.WebServerOrchestrator.Options.SnapshotsDirectory = serverOptions.SnapshotsDirectory;
            this.WebServerOrchestrator.Options.BatchSize = serverOptions.BatchSize;
            this.WebServerOrchestrator.Options.BatchDirectory = serverOptions.BatchDirectory;

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), clientOptions);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.IsTrue(Directory.Exists(rootDirectory));
                Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, nameDirectory)));

                var serverFiles = Directory.GetFiles(serverOptions.BatchDirectory, "*", SearchOption.AllDirectories);
                var clientFiles = Directory.GetFiles(clientOptions.BatchDirectory, "*", SearchOption.AllDirectories);

                Assert.IsTrue(serverFiles.Count() == 0);
                Assert.IsTrue(clientFiles.Count() == 0);
            }
        }


        /// <summary>
        /// Insert one row in two tables on server, should be correctly sync on all clients
        /// </summary>
        [TestMethod]
        public async Task Snapshot_Initialize_With_CustomSeriazlizer_MessagePack()
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // snapshot directory
            var snapshotDirctory = _databaseHelper.GetRandomName();
            var directory = Path.Combine(Environment.CurrentDirectory, snapshotDirctory);
            // ----------------------------------
            // Setting correct options for sync agent to be able to reach snapshot
            // ----------------------------------
            var options = new SyncOptions
            {
                SnapshotsDirectory = directory,
                BatchSize = 200
            };

            // ----------------------------------
            // Create a snapshot
            // ----------------------------------
            var remoteOrchestrator = new RemoteOrchestrator(_versionService, Server.Provider, options, this.FilterSetup);
            await remoteOrchestrator.CreateSnapshotAsync(this.FilterParameters);


            // ----------------------------------
            // Add rows on server AFTER snapshot
            // ----------------------------------
            // Create a new address & customer address on server
            using (var serverDbCtx = new DataContext(this.Server))
            {
                var addressLine1 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var newAddress = new Address { AddressLine1 = addressLine1 };

                serverDbCtx.Address.Add(newAddress);
                await serverDbCtx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "OTH"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress);
                await serverDbCtx.SaveChangesAsync();
            }

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup = this.FilterSetup;
            this.WebServerOrchestrator.Options.SnapshotsDirectory = directory;
            this.WebServerOrchestrator.Options.BatchSize = 200;
            this.WebServerOrchestrator.SerializerFactories.Add(new CustomMessagePackSerializerFactory());

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(this.ServiceUri, _versionService), options);
                agent.Parameters.AddRange(this.FilterParameters);

                var snapshotApplying = 0;
                var snapshotApplied = 0;

                agent.LocalOrchestrator.OnSnapshotApplying(saa => snapshotApplying++);
                agent.LocalOrchestrator.OnSnapshotApplied(saa => snapshotApplied++);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                Assert.AreEqual(1, snapshotApplying);
                Assert.AreEqual(1, snapshotApplied);
            }
        }

    }
}
