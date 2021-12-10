﻿using ISynergy.Framework.AspNetCore.Synchronization.Tests.Data;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Abstractions.Tests;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Parameters;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Tests.Tcp.Base
{
    public abstract class BaseTcpFilterTests : BaseTcpTests, IDisposable
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
        /// Gets the remote orchestrator and its database name
        /// </summary>
        public (string DatabaseName, ProviderType ProviderType, CoreProvider Provider) Server { get; private set; }

        /// <summary>
        /// Gets the dictionary of all local orchestrators with database name as key
        /// </summary>
        public List<(string DatabaseName, ProviderType ProviderType, CoreProvider Provider)> Clients { get; set; }

        /// <summary>
        /// Gets a bool indicating if we should generate the schema for tables
        /// </summary>
        public bool UseFallbackSchema => ServerType == ProviderType.Sql;

        protected readonly IDatabaseHelper _databaseHelper;

        /// <summary>
        /// For each test, Create a server database and some clients databases, depending on ProviderType provided in concrete class
        /// </summary>
        public BaseTcpFilterTests()
        {
            // Since we are creating a lot of databases
            // each database will have its own pool
            // Droping database will not clear the pool associated
            // So clear the pools on every start of a new test
            _databaseHelper.ClearAllPools();


            // get the server provider (and db created) without seed
            var serverDatabaseName = _databaseHelper.GetRandomName("tcpfilt_sv_");

            // create remote orchestrator
            var serverProvider = this.CreateProvider(this.ServerType, serverDatabaseName);

            this.Server = (serverDatabaseName, this.ServerType, serverProvider);

            // Get all clients providers
            Clients = new List<(string DatabaseName, ProviderType ProviderType, CoreProvider Provider)>(this.ClientsType.Count);

            // Generate Client database
            foreach (var clientType in this.ClientsType)
            {
                var dbCliName = _databaseHelper.GetRandomName("tcpfilt_cli_");
                var localProvider = this.CreateProvider(clientType, dbCliName);

                this.Clients.Add((dbCliName, clientType, localProvider));
            }
        }

        /// <summary>
        /// Drop all databases used for the tests
        /// </summary>
        public void Dispose()
        {
            _databaseHelper.DropDatabase(Server.DatabaseName);

            foreach (var client in Clients)
                _databaseHelper.DropDatabase(client.DatabaseName);
        }


        [TestMethod]
        public virtual async Task SchemaIsCreated()
        {
            // create a server db without seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, new SyncOptions(), this.FilterSetup);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);

                var schema = await agent.LocalOrchestrator.GetSchemaAsync();

                // Check we have the correct columns replicated
                using var c = client.Provider.CreateConnection();

                await c.OpenAsync();

                foreach (var setupTable in FilterSetup.Tables)
                {
                    var syncTable = new SyncTable(setupTable.TableName, setupTable.SchemaName);

                    var (tableName, trackingTableName) = client.Provider.GetParsers(syncTable, FilterSetup);

                    var tableBuilder = client.Provider.GetTableBuilder(syncTable, tableName, trackingTableName, this.FilterSetup);

                    var clientColumns = await tableBuilder.GetColumnsAsync(c, null);

                    // Check we have the same columns count
                    if (setupTable.Columns.Count == 0)
                    {
                        using var serverConnection = this.Server.Provider.CreateConnection();

                        serverConnection.Open();

                        var tableServerManagerFactory = this.Server.Provider.GetTableBuilder(syncTable, tableName, trackingTableName, this.FilterSetup);
                        var serverColumns = await tableServerManagerFactory.GetColumnsAsync(serverConnection, null);

                        serverConnection.Close();

                        Assert.AreEqual(serverColumns.Count(), clientColumns.Count());

                        // Check we have the same columns names
                        foreach (var serverColumn in serverColumns)
                            Assert.IsTrue(clientColumns.Any((col) => col.ColumnName == serverColumn.ColumnName));
                    }
                    else
                    {
                        Assert.AreEqual(setupTable.Columns.Count, clientColumns.Count());

                        // Check we have the same columns names
                        foreach (var setupColumn in setupTable.Columns)
                            Assert.IsTrue(clientColumns.Any((col) => col.ColumnName == setupColumn));
                    }
                }
                c.Close();
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

            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);
                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(rowsCount, this.GetServerDatabaseRowsCount(client));

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

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
                Assert.AreEqual(rowsCount, this.GetServerDatabaseRowsCount(client));
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
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                Assert.AreEqual(rowsCount + 2, this.GetServerDatabaseRowsCount(client));

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

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

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
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

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
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                await agent.SynchronizeAsync();
            }

            rowsCount = this.GetServerDatabaseRowsCount(this.Server);
            foreach (var client in Clients)
                Assert.AreEqual(rowsCount, this.GetServerDatabaseRowsCount(client));
        }


        /// <summary>
        /// Insert four rows on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Delete_TwoTables_FromClient(SyncOptions options)
        {
            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

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
                    SalesOrderNumber = $"SO-99099",
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
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                //Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(4, s.TotalChangesUploaded);
                //Assert.AreEqual(0, s.TotalSyncConflicts);
            }

            // Now sync again to be sure all clients have all lines
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                await agent.SynchronizeAsync();
            }


            // Delete lines from client
            // Now sync again to be sure all clients have all lines
            foreach (var client in Clients)
            {
                using var ctx = new DataContext(client, this.UseFallbackSchema);
                ctx.SalesOrderDetail.RemoveRange(ctx.SalesOrderDetail.ToList());
                ctx.SalesOrderHeader.RemoveRange(ctx.SalesOrderHeader.ToList());
                await ctx.SaveChangesAsync();
            }

            // now sync

            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                //Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(8, s.TotalChangesUploaded);
                //Assert.AreEqual(0, s.TotalSyncConflicts);
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

            // ----------------------------------
            // Setting correct options for sync agent to be able to reach snapshot
            // ----------------------------------
            var snapshotDirctory = _databaseHelper.GetRandomName();
            var directory = Path.Combine(Environment.CurrentDirectory, snapshotDirctory);
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

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                var snapshotApplying = 0;
                var snapshotApplied = 0;

                agent.LocalOrchestrator.OnSnapshotApplying(saa => snapshotApplying++);
                agent.LocalOrchestrator.OnSnapshotApplied(saa => snapshotApplied++);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                Assert.AreEqual(1, snapshotApplying);
                Assert.AreEqual(1, snapshotApplied);
            }
        }

        /// <summary>
        /// Insert rows on server, and ensure DISTINCT is applied correctly 
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Insert_TwoTables_EnsureDistinct(SyncOptions options)
        {
            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

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

                var addressLine2 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);
                var newAddress2 = new Address { AddressLine1 = addressLine2 };
                serverDbCtx.Address.Add(newAddress2);

                await serverDbCtx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "Secondary Home 1"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress);

                var newCustomerAddress2 = new CustomerAddress
                {
                    AddressId = newAddress2.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "Secondary Home 2"
                };

                serverDbCtx.CustomerAddress.Add(newCustomerAddress2);

                await serverDbCtx.SaveChangesAsync();

                // Update customer
                var customer = serverDbCtx.Customer.Find(Guid.NewGuid());
                customer.FirstName = "Orlanda";

                await serverDbCtx.SaveChangesAsync();
            }

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(5, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }
        }


        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Using_ExistingClientDatabase_ProvisionDeprovision(SyncOptions options)
        {
            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create a client schema without seeding
                await this.EnsureDatabaseSchemaAndSeedAsync(client, false, UseFallbackSchema);

                var localOrchestrator = new LocalOrchestrator(_versionService, client.Provider, options, this.FilterSetup);

                // just check interceptor
                var onTableCreatedCount = 0;
                localOrchestrator.OnTableCreated(args => onTableCreatedCount++);

                // Provision the database with all tracking tables, stored procedures, triggers and scope
                await localOrchestrator.ProvisionAsync();

                //--------------------------
                // ASSERTION
                //--------------------------

                // check if scope table is correctly created
                var scopeInfoTableExists = await localOrchestrator.ExistScopeInfoTableAsync(DbScopeType.Client, options.ScopeInfoTableName);
                Assert.IsTrue(scopeInfoTableExists);

                // get the db manager
                foreach (var setupTable in this.FilterSetup.Tables)
                {
                    Assert.IsTrue(await localOrchestrator.ExistTrackingTableAsync(setupTable));

                    Assert.IsTrue(await localOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Delete));
                    Assert.IsTrue(await localOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Insert));
                    Assert.IsTrue(await localOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Update));

                    if (client.ProviderType == ProviderType.Sql)
                    {
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkDeleteRows));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkTableType));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkUpdateRows));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.DeleteMetadata));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.DeleteRow));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.Reset));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectChanges));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectInitializedChanges));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectRow));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.UpdateRow));

                        // Filters here
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectChangesWithFilters));
                        Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectInitializedChangesWithFilters));
                    }

                }

                //localOrchestrator.OnTableProvisioned(null);

                //// Deprovision the database with all tracking tables, stored procedures, triggers and scope
                await localOrchestrator.DeprovisionAsync();

                // check if scope table is correctly created
                scopeInfoTableExists = await localOrchestrator.ExistScopeInfoTableAsync(DbScopeType.Client, options.ScopeInfoTableName);
                Assert.IsFalse(scopeInfoTableExists);

                // get the db manager
                foreach (var setupTable in this.FilterSetup.Tables)
                {
                    Assert.IsFalse(await localOrchestrator.ExistTrackingTableAsync(setupTable));

                    Assert.IsFalse(await localOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Delete));
                    Assert.IsFalse(await localOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Insert));
                    Assert.IsFalse(await localOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Update));


                    if (client.ProviderType == ProviderType.Sql)
                    {
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkDeleteRows));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkTableType));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkUpdateRows));

                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.DeleteMetadata));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.DeleteRow));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.Reset));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectChanges));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectInitializedChanges));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectRow));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.UpdateRow));

                        // check filters are deleted
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectChangesWithFilters));
                        Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectInitializedChangesWithFilters));
                    }

                }


            }
        }


        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Using_ExistingClientDatabase_Filter_With_NotSyncedColumn(SyncOptions options)
        {

            if (this.Server.ProviderType != ProviderType.Sql)
                return;

            var clients = this.Clients.Where(c => c.ProviderType == ProviderType.Sql || c.ProviderType == ProviderType.Sqlite);

            var setup = new SyncSetup(new string[] { "Customer" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases WITH schema, and WITHOUT seeding
            foreach (var client in clients)
            {
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);
                await this.EnsureDatabaseSchemaAndSeedAsync(client, false, UseFallbackSchema);
            }


            var filter = new SetupFilter("Customer");
            filter.AddParameter("EmployeeID", DbType.Int32, true);
            filter.AddCustomWhere("EmployeeID = @EmployeeID or @EmployeeID is null");

            setup.Filters.Add(filter);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);

            }
        }

        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Migration_Adding_Table(SyncOptions options)
        {

            var setup = new SyncSetup(new string[] { "Customer" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

            // Adding a new table
            setup.Tables.Add("Employee");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync(SyncType.Reinitialize);

                Assert.AreEqual(5, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

        }

        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Migration_Modifying_Table(SyncOptions options)
        {

            var setup = new SyncSetup(new string[] { "Customer" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

            // Adding a new column to Customer
            setup.Tables["Customer"].Columns.Add("EmailAddress");



            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);

                // create agent with filtered tables and parameter
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync(SyncType.Reinitialize);

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

        }


        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Migration_Removing_Table(SyncOptions options)
        {
            var setup = new SyncSetup(new string[] { "Customer", "Employee" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(5, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

            // Adding a new column to Customer
            setup.Tables.Remove(setup.Tables["Customer"]);
            setup.Filters.Clear();

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

        }


        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Deprovision_Should_Remove_Filtered_StoredProcedures(SyncOptions options)
        {
            var setup = new SyncSetup(new string[] { "Customer", "Employee" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(5, s.ChangesAppliedOnClient.TotalAppliedChanges);

                await agent.LocalOrchestrator.DeprovisionAsync();

                foreach (var setupTable in setup.Tables)
                {
                    Assert.IsFalse(await agent.LocalOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Delete));
                    Assert.IsFalse(await agent.LocalOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Insert));
                    Assert.IsFalse(await agent.LocalOrchestrator.ExistTriggerAsync(setupTable, DbTriggerType.Update));

                    if (client.ProviderType == ProviderType.Sql)
                    {
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkDeleteRows));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkTableType));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.BulkUpdateRows));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.DeleteMetadata));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.DeleteRow));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.Reset));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectChanges));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectInitializedChanges));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectRow));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.UpdateRow));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectChangesWithFilters));
                        Assert.IsFalse(await agent.LocalOrchestrator.ExistStoredProcedureAsync(setupTable, DbStoredProcedureType.SelectInitializedChangesWithFilters));
                    }
                }

            }
        }



        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Migration_Rename_TrackingTable(SyncOptions options)
        {

            var setup = new SyncSetup(new string[] { "Customer" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

            // Modifying pref and sufix
            setup.StoredProceduresPrefix = "sp__";

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync(SyncType.Reinitialize);

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

        }


        /// <summary>
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Migration_Adding_Table_AndReinitialize_TableOnly(SyncOptions options)
        {
            var setup = new SyncSetup(new string[] { "[Customer]" });

            // Filter columns. We are not syncing EmployeeID, BUT this column will be part of the filter
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }

            // Adding a new table
            setup.Tables.Add("Employee");

            // Trying to Hack the Reinitialize Thing
            foreach (var client in Clients)
            {
                // create an agent 
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);

                // ON SERVER : When trying to get changes from the server, just replace the command with the Initialize command
                // and get ALL the rows for the migrated new table
                agent.RemoteOrchestrator.OnTableChangesSelecting(async tcs =>
                {
                    if (tcs.Context.AdditionalProperties == null || tcs.Context.AdditionalProperties.Count <= 0)
                        return;

                    if (tcs.Context.AdditionalProperties.ContainsKey(tcs.Table.GetFullName()))
                    {
                        var addProp = tcs.Context.AdditionalProperties[tcs.Table.GetFullName()];
                        if (addProp == "Reinitialize")
                        {
                            var adapter = agent.RemoteOrchestrator.GetSyncAdapter(tcs.Table, setup);
                            var command = await adapter.GetCommandAsync(DbCommandType.SelectInitializedChanges, tcs.Connection, tcs.Transaction, tcs.Table.GetFilter());
                            tcs.Command = command;
                        }
                    }
                });

                // On client
                agent.LocalOrchestrator.OnMigrated(ma =>
                {
                    // migrateTables are empty if not migration tables has been done.
                    var migratedTables = ma.Migration.Tables;

                    foreach (var migratedTable in migratedTables)
                    {
                        var tableName = migratedTable.SetupTable.GetFullName();

                        if (migratedTable.Table == MigrationAction.Create)
                        {
                            if (ma.Context.AdditionalProperties == null)
                                ma.Context.AdditionalProperties = new Dictionary<string, string>();

                            ma.Context.AdditionalProperties.Add(tableName, "Reinitialize");
                        }
                    }
                });

                // ON CLIENT : Forcing Reset of the table to be sure no conflicts will be raised
                // And all rows will be re-applied 
                agent.LocalOrchestrator.OnTableChangesApplying(async tca =>
                {
                    if (tca.Context.AdditionalProperties == null || tca.Context.AdditionalProperties.Count <= 0)
                        return;

                    if (tca.State != DataRowState.Modified)
                        return;

                    if (tca.Context.AdditionalProperties.ContainsKey(tca.Table.GetFullName()))
                    {
                        var addProp = tca.Context.AdditionalProperties[tca.Table.GetFullName()];
                        if (addProp == "Reinitialize")
                            await agent.LocalOrchestrator.ResetTableAsync(setup.Tables[tca.Table.TableName, tca.Table.SchemaName], tca.Connection, tca.Transaction);
                    }
                });


                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(3, s.ChangesAppliedOnClient.TotalAppliedChanges);


                s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);

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
            var options = new SyncOptions
            {
                SnapshotsDirectory = directory,
                BatchSize = 200
            };
            // ----------------------------------
            // Create a snapshot
            // ----------------------------------
            var remoteOrchestrator = new RemoteOrchestrator(_versionService, Server.Provider, options, this.FilterSetup);

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

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.IsTrue(Directory.Exists(rootDirectory));
                Assert.IsTrue(Directory.Exists(Path.Combine(rootDirectory, nameDirectory)));
            }

        }


        /// <summary>
        /// Insert one row in two tables on server, should be correctly sync on all clients
        /// </summary>
        [TestMethod]
        public async Task Snapshot_Initialize_ThenClientUploadSync_ThenReinitialize()
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // snapshot directory
            var snapshotDirctory = _databaseHelper.GetRandomName();
            var directory = Path.Combine(Environment.CurrentDirectory, snapshotDirctory);

            var options = new SyncOptions
            {
                SnapshotsDirectory = directory,
                BatchSize = 200
            };

            var remoteOrchestrator = new RemoteOrchestrator(_versionService, Server.Provider, options, this.FilterSetup);

            await remoteOrchestrator.CreateSnapshotAsync(this.FilterParameters);

            // ----------------------------------
            // Add rows on server AFTER snapshot
            // ----------------------------------
            var productId = Guid.NewGuid();
            var productName = _databaseHelper.GetRandomName();
            var productNumber = productName.ToUpperInvariant().Substring(0, 10);

            var productCategoryName = _databaseHelper.GetRandomName();
            var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

            using (var ctx = new DataContext(this.Server))
            {
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);

                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                ctx.Add(product);


                var addressLine1 = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var newAddress = new Address { AddressLine1 = addressLine1 };

                ctx.Address.Add(newAddress);
                await ctx.SaveChangesAsync();

                var newCustomerAddress = new CustomerAddress
                {
                    AddressId = newAddress.AddressId,
                    CustomerId = Guid.NewGuid(),
                    AddressType = "OTH"
                };

                ctx.CustomerAddress.Add(newCustomerAddress);

                await ctx.SaveChangesAsync();
            }

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);


            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // ----------------------------------
            // Now add rows on client
            // ----------------------------------

            foreach (var client in Clients)
            {
                var name = _databaseHelper.GetRandomName();
                var pn = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var product = new Product { ProductId = Guid.NewGuid(), ProductCategoryId = "_BIKES", Name = name, ProductNumber = pn };

                using var ctx = new DataContext(client, this.UseFallbackSchema);
                ctx.Product.Add(product);
                await ctx.SaveChangesAsync();
            }

            // Sync all clients
            // First client  will upload one line and will download nothing
            // Second client will upload one line and will download one line
            // thrid client  will upload one line and will download two lines
            var download = 0;
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(download++, s.TotalChangesDownloaded);
                Assert.AreEqual(1, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Get count of rows
            rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // ----------------------------------
            // Now Reinitialize
            // ----------------------------------

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, this.FilterSetup);

                agent.Parameters.AddRange(this.FilterParameters);


                var s = await agent.SynchronizeAsync(SyncType.Reinitialize);

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }
        }


        /// <summary>
        /// </summary>
        [TestMethod]
        public async Task Synchronize_ThenDeprovision_ThenAddPrefixes()
        {
            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "Customer" });

            // Filtered columns. 
            setup.Tables["Customer"].Columns.AddRange(new string[] { "CustomerID", "EmployeeID", "NameStyle", "FirstName", "LastName" });

            // create a server schema and seed
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            setup.Filters.Add("Customer", "EmployeeID");

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                // create agent with filtered tables and parameter
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.ChangesAppliedOnClient.TotalAppliedChanges);

            }
            foreach (var client in Clients)
            {
                // Deprovision everything
                var localOrchestrator = new LocalOrchestrator(_versionService, client.Provider, options, setup);
                await localOrchestrator.DeprovisionAsync();

            }

            // Adding a new table
            setup.Tables.Add("Employee");

            // Adding prefixes
            setup.StoredProceduresPrefix = "sync";
            setup.StoredProceduresSuffix = "sp";
            setup.TrackingTablesPrefix = "track";
            setup.TrackingTablesSuffix = "tbl";
            setup.TriggersPrefix = "trg";
            setup.TriggersSuffix = "tbl";

            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService,client.Provider, Server.Provider, options, setup);
                agent.Parameters.Add("EmployeeID", 1);

                var s = await agent.SynchronizeAsync(SyncType.Reinitialize);
                Assert.AreEqual(5, s.ChangesAppliedOnClient.TotalAppliedChanges);
            }
        }
    }
}
