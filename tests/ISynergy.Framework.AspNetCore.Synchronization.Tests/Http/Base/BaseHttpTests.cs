﻿using ISynergy.Framework.AspNetCore.Synchronization.Extensions;
using ISynergy.Framework.AspNetCore.Synchronization.Tests.Data;
using ISynergy.Framework.Synchronization.Client;
using ISynergy.Framework.Synchronization.Client.Arguments;
using ISynergy.Framework.Synchronization.Client.Exceptions;
using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.Core.Tests.Converters;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Tests.Http.Base
{
    public abstract class BaseHttpTests : BaseTests
    {
        [TestMethod]
        public virtual async Task SchemaIsCreated()
        {
            // create a server db without seed
            await this.EnsureDatabaseSchemaAndSeedAsync(Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri));

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
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
            }
        }

        /// <summary>
        /// Check a bad connection should raise correct error
        /// </summary>
        [TestMethod]
        public async Task Bad_ConnectionFromServer_ShouldRaiseError()
        {

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            var onReconnect = new Action<ReConnectArgs>(args =>
                 Console.WriteLine($"Can't connect to database {args.Connection?.Database}. Retry N°{args.Retry}. Waiting {args.WaitingTimeSpan.Milliseconds}. Exception:{args.HandledException.Message}."));

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);
            // change the remote orchestrator connection string
            this.WebServerOrchestrator.Provider.ConnectionString = $@"Server=unknown;Database=unknown;UID=sa;PWD=unknown";

            this.WebServerOrchestrator.OnReConnect(onReconnect);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                webClientOrchestrator.SyncPolicy.RetryCount = 0;

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);


                agent.LocalOrchestrator.OnReConnect(onReconnect);

                var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
                {
                    var s = await agent.SynchronizeAsync();

                });
            }
        }

        [TestMethod]
        public async Task Bad_TableWithoutPrimaryKeys_ShouldRaiseError()
        {
            var tableTestCreationScript = "Create Table TableTest (TestId int, TestName varchar(50))";

            // Create an empty server database
            await this.CreateDatabaseAsync(this.ServerType, this.Server.DatabaseName, true);

            // Create the table on the server
            await _databaseHelper.ExecuteScriptAsync(this.Server.DatabaseName, tableTestCreationScript); ;

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.Add("TableTest");

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                webClientOrchestrator.SyncPolicy.RetryCount = 0;
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);

                var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
                {
                    var s = await agent.SynchronizeAsync();
                });

                Assert.AreEqual(SyncSide.ServerSide, se.Side);
                Assert.AreEqual("MissingPrimaryKeyException", se.TypeName);
                Assert.AreEqual(this.Server.DatabaseName, se.InitialCatalog);

            }
        }

        [TestMethod]
        public async Task Bad_ColumnSetup_DoesNotExistInSchema_ShouldRaiseError()
        {
            // create a server db without seed
            await this.EnsureDatabaseSchemaAndSeedAsync(Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);
            // Add a malformatted column name
            this.WebServerOrchestrator.Setup.Tables["Employee"].Columns.AddRange(new string[] { "EmployeeID", "FirstName", "LastNam" });

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                webClientOrchestrator.SyncPolicy.RetryCount = 0;

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);

                var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
                {
                    var s = await agent.SynchronizeAsync();
                });

                Assert.AreEqual(SyncSide.ServerSide, se.Side);
                Assert.AreEqual("MissingColumnException", se.TypeName);
            }
        }

        [TestMethod]
        public async Task Bad_TableSetup_DoesNotExistInSchema_ShouldRaiseError()
        {
            // create a server db without seed
            await this.EnsureDatabaseSchemaAndSeedAsync(Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);
            // Add a fake table to setup tables
            this.WebServerOrchestrator.Setup.Tables.Add("WeirdTable");

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                webClientOrchestrator.SyncPolicy.RetryCount = 0;

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);

                var se = await Assert.ThrowsExceptionAsync<SyncException>(async () =>
                {
                    var s = await agent.SynchronizeAsync();
                });

                Assert.AreEqual(SyncSide.ServerSide, se.Side);
                Assert.AreEqual("MissingTableException", se.TypeName);
            }
        }

        /// <summary>
        /// Insert one row on server, should be correctly sync on all clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Insert_OneTable_FromServer(SyncOptions options)
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Create a new product on server
            var name = _databaseHelper.GetRandomName();
            var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

            var product = new Product { ProductId = Guid.NewGuid(), Name = name, ProductNumber = productNumber };

            using (var serverDbCtx = new DataContext(this.Server))
            {
                serverDbCtx.Product.Add(product);
                await serverDbCtx.SaveChangesAsync();
            }

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(1, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }
        }

        /// <summary>
        /// Insert one row on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Insert_OneTable_FromClient(SyncOptions options)
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Insert one line on each client
            foreach (var client in Clients)
            {
                var name = _databaseHelper.GetRandomName();
                var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var product = new Product { ProductId = Guid.NewGuid(), Name = name, ProductNumber = productNumber };

                using var serverDbCtx = new DataContext(client, this.UseFallbackSchema);
                serverDbCtx.Product.Add(product);
                await serverDbCtx.SaveChangesAsync();
            }

            // Sync all clients
            // First client  will upload one line and will download nothing
            // Second client will upload one line and will download one line
            // thrid client  will upload one line and will download two lines
            var download = 0;
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(download++, s.TotalChangesDownloaded);
                Assert.AreEqual(1, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

        }

        /// <summary>
        /// Delete rows on server, should be correctly sync on all clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Delete_OneTable_FromServer(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // get rows count
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // part of the filter
            var employeeId = 1;
            // will be defined when address is inserted
            var addressId = 0;

            // Insert one address row and one addressemployee row
            using (var serverDbCtx = new DataContext(this.Server))
            {
                // Insert a new address for employee 1
                var city = "Paris " + _databaseHelper.GetRandomName();
                var addressline1 = "Rue Monthieu " + _databaseHelper.GetRandomName();
                var stateProvince = "Ile de France";
                var countryRegion = "France";
                var postalCode = "75001";

                var address = new Address
                {
                    AddressLine1 = addressline1,
                    City = city,
                    StateProvince = stateProvince,
                    CountryRegion = countryRegion,
                    PostalCode = postalCode

                };

                serverDbCtx.Add(address);
                await serverDbCtx.SaveChangesAsync();
                addressId = address.AddressId;

                var employeeAddress = new EmployeeAddress
                {
                    EmployeeId = employeeId,
                    AddressId = address.AddressId,
                    AddressType = "SERVER"
                };

                var ea = serverDbCtx.EmployeeAddress.Add(employeeAddress);
                await serverDbCtx.SaveChangesAsync();

            }

            // add 2 lines to rows count
            rowsCount += 2;

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                // check rows are create on client
                using var ctx = new DataContext(client, this.UseFallbackSchema);
                var finalAddressesCount = await ctx.Address.AsNoTracking().CountAsync(a => a.AddressId == addressId);
                var finalEmployeeAddressesCount = await ctx.EmployeeAddress.AsNoTracking().CountAsync(a => a.AddressId == addressId && a.EmployeeId == employeeId);
                Assert.AreEqual(1, finalAddressesCount);
                Assert.AreEqual(1, finalEmployeeAddressesCount);


            }

            // Delete those lines from server
            using (var serverDbCtx = new DataContext(this.Server))
            {
                // Get the addresses query
                var address = await serverDbCtx.Address.SingleAsync(a => a.AddressId == addressId);
                var empAddress = await serverDbCtx.EmployeeAddress.SingleAsync(a => a.AddressId == addressId && a.EmployeeId == employeeId);

                // remove them
                serverDbCtx.EmployeeAddress.Remove(empAddress);
                serverDbCtx.Address.Remove(address);

                // Execute query
                await serverDbCtx.SaveChangesAsync();
            }

            // Sync and check we have delete these lines on each server
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(2, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                // check row deleted on client values
                using var ctx = new DataContext(client, this.UseFallbackSchema);
                var finalAddressesCount = await ctx.Address.AsNoTracking().CountAsync(a => a.AddressId == addressId);
                var finalEmployeeAddressesCount = await ctx.EmployeeAddress.AsNoTracking().CountAsync(a => a.AddressId == addressId && a.EmployeeId == employeeId);
                Assert.AreEqual(0, finalAddressesCount);
                Assert.AreEqual(0, finalEmployeeAddressesCount);
            }
        }

        /// <summary>
        /// Insert thousand or rows. Check if batch mode works correctly
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Insert_ThousandRows_FromClient(SyncOptions options)
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                await agent.SynchronizeAsync();
            }

            // Insert one thousand lines on each client
            foreach (var client in Clients)
            {
                using var ctx = new DataContext(client, this.UseFallbackSchema);
                for (var i = 0; i < 2000; i++)
                {
                    var name = _databaseHelper.GetRandomName();
                    var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                    var product = new Product { ProductId = Guid.NewGuid(), Name = name, ProductNumber = productNumber };

                    ctx.Product.Add(product);
                }
                await ctx.SaveChangesAsync();
            }

            // Sync all clients
            // First client  will upload 2000 lines and will download nothing
            // Second client will upload 2000 lines and will download 2000 lines
            // Third client  will upload 2000 line and will download 4000 lines
            var download = 0;
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(download * 2000, s.TotalChangesDownloaded);
                Assert.AreEqual(2000, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                download++;
            }

        }

        /// <summary>
        /// Insert one row on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Reinitialize_Client(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Insert one line on each client
            foreach (var client in Clients)
            {
                var productCategoryName = _databaseHelper.GetRandomName();
                var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

                var productId = Guid.NewGuid();
                var productName = _databaseHelper.GetRandomName();
                var productNumber = productName.ToUpperInvariant().Substring(0, 10);

                using var ctx = new DataContext(client, this.UseFallbackSchema);
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);
                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber, ProductCategoryId = productCategoryId };
                ctx.Add(product);
                await ctx.SaveChangesAsync();
            }

            // Sync all clients
            // inserted rows will be deleted 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync(SyncType.Reinitialize);

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

            }
        }

        /// <summary>
        /// Insert one row on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task ReinitializeWithUpload_Client(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Insert one line on each client
            foreach (var client in Clients)
            {
                var productCategoryName = _databaseHelper.GetRandomName();
                var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

                var productId = Guid.NewGuid();
                var productName = _databaseHelper.GetRandomName();
                var productNumber = productName.ToUpperInvariant().Substring(0, 10);

                using var ctx = new DataContext(client, this.UseFallbackSchema);
                var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                ctx.Add(pc);
                var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber, ProductCategoryId = productCategoryId };
                ctx.Add(product);
                await ctx.SaveChangesAsync();
            }

            // Sync all clients
            // client  will upload two lines and will download all + its two lines
            var download = 2;
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                var s = await agent.SynchronizeAsync(SyncType.ReinitializeWithUpload);

                Assert.AreEqual(rowsCount + download, s.TotalChangesDownloaded);
                Assert.AreEqual(2, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
                download += 2;
            }


        }

        /// <summary>
        /// Insert one row on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Bad_Converter_NotRegisteredOnServer_ShouldRaiseError(SyncOptions options)
        {
            // create a server db and seed it
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                // Add a converter on the client.
                // But this converter is not register on the server side converters list.
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri, new DateConverter());
                webClientOrchestrator.SyncPolicy.RetryCount = 0;

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                var exception = await Assert.ThrowsExceptionAsync<HttpSyncWebException>(async () =>
                {
                    var s = await agent.SynchronizeAsync();

                });

                Assert.AreEqual("HttpConverterNotConfiguredException", exception.TypeName);
            }
        }

        /// <summary>
        /// Check web interceptors are working correctly
        /// </summary>
        //[DataTestMethod]
        //[DataRow(typeof(SyncOptionsData))]
        public async Task Check_Interceptors_WebServerOrchestrator(SyncOptions options)
        {
            // create a server db and seed it
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            this.WebServerOrchestrator.OnHttpGettingRequest(r =>
            {
                Assert.IsNotNull(r.HttpContext);
                Assert.IsNotNull(r.Context);
            });

            this.WebServerOrchestrator.OnHttpSendingResponse(r =>
            {
                Assert.IsNotNull(r.HttpContext);
                Assert.IsNotNull(r.Context);
            });


            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
            }

            this.WebServerOrchestrator.OnHttpGettingRequest(null);
            this.WebServerOrchestrator.OnHttpSendingResponse(null);
        }

        /// <summary>
        /// Check web interceptors are working correctly
        /// </summary>
        //[DataTestMethod]
        //[DataRow(typeof(SyncOptionsData))]
        public async Task Check_Interceptors_WebClientOrchestrator(SyncOptions options)
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            foreach (var client in Clients)
            {
                var wenClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, wenClientOrchestrator, options);

                // Interceptor on sending scopes
                wenClientOrchestrator.OnHttpGettingScopeResponse(sra =>
                {
                    // check we a scope name
                    Assert.IsNotNull(sra.Context);
                });

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                wenClientOrchestrator.OnHttpGettingScopeResponse(null);
            }

            // Insert one line on each client
            foreach (var client in Clients)
            {
                var name = _databaseHelper.GetRandomName();
                var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var product = new Product { ProductId = Guid.NewGuid(), Name = name, ProductNumber = productNumber };

                using var serverDbCtx = new DataContext(client, this.UseFallbackSchema);
                serverDbCtx.Product.Add(product);
                await serverDbCtx.SaveChangesAsync();
            }

            // Sync all clients
            // First client  will upload one line and will download nothing
            // Second client will upload one line and will download one line
            // thrid client  will upload one line and will download two lines
            var download = 0;
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                // Just before sending changes, get changes sent
                webClientOrchestrator.OnHttpSendingChangesRequest(sra =>
                {
                    // check we have rows
                    Assert.IsTrue(sra.Request.Changes.HasRows);
                });


                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(download++, s.TotalChangesDownloaded);
                Assert.AreEqual(1, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                webClientOrchestrator.OnHttpSendingChangesRequest(null);
            }

        }

        /// <summary>
        /// Insert one row on each client, should be sync on server and clients
        /// </summary>
        //[DataTestMethod]
        //[DataRow(typeof(SyncOptionsData))]
        public async Task Converter_Registered_ShouldConvertDateTime(SyncOptions options)
        {
            // create a server db and seed it
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get response just before response sent back from server
            // Assert if datetime are correctly converted to long
            this.WebServerOrchestrator.OnHttpSendingChanges(sra =>
            {
                if (sra.Response.Changes == null)
                    return;

                // check we have rows
                Assert.IsTrue(sra.Response.Changes.HasRows);

                // getting a table where we know we have date time
                var table = sra.Response.Changes.Tables.FirstOrDefault(t => t.TableName == "Employee");

                if (table != null)
                {
                    Assert.IsTrue(table.Rows.Count > 0);

                    foreach (var row in table.Rows)
                    {
                        var dateCell = row[5];

                        // check we have an integer here
                        Assert.IsInstanceOfType(dateCell, typeof(long));
                    }
                }
            });

            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri, new DateConverter());
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
            }

            this.WebServerOrchestrator.OnHttpSendingChanges(null);
        }


        /// <summary>
        /// Insert one row in two tables on server, should be correctly sync on all clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Snapshot_Initialize(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // snapshot directory
            var snapshotDirctoryName = _databaseHelper.GetRandomName();
            var snapshotDirectory = Path.Combine(Environment.CurrentDirectory, snapshotDirctoryName);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);
            this.WebServerOrchestrator.Options.SnapshotsDirectory = snapshotDirectory;
            this.WebServerOrchestrator.Options.BatchSize = 2000;

            // ----------------------------------
            // Create a snapshot
            // ----------------------------------
            await this.WebServerOrchestrator.CreateSnapshotAsync();

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

                await ctx.SaveChangesAsync();
            }

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }
        }

        /// <summary>
        /// Insert one row on each client, should be sync on server and clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task IsOutdated_ShouldWork_If_Correct_Action(SyncOptions options)
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);


            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(0, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // Call a server delete metadata to update the last valid timestamp value in scope_info_server table
            var dmc = await this.WebServerOrchestrator.DeleteMetadatasAsync();

            // Insert one line on each client
            foreach (var client in Clients)
            {
                // Client side : Create a product category and a product
                var productId = Guid.NewGuid();
                var productName = _databaseHelper.GetRandomName();
                var productNumber = productName.ToUpperInvariant().Substring(0, 10);
                var productCategoryName = _databaseHelper.GetRandomName();
                var productCategoryId = productCategoryName.ToUpperInvariant().Substring(0, 6);

                using (var ctx = new DataContext(client, this.UseFallbackSchema))
                {
                    var pc = new ProductCategory { ProductCategoryId = productCategoryId, Name = productCategoryName };
                    ctx.Add(pc);

                    var product = new Product { ProductId = productId, Name = productName, ProductNumber = productNumber };
                    ctx.Add(product);

                    await ctx.SaveChangesAsync();
                }

                // Generate an outdated situation
                await _databaseHelper.ExecuteScriptAsync(client.DatabaseName,
                                    $"Update scope_info set scope_last_server_sync_timestamp={dmc.TimestampLimit - 1}");

                // create a new agent
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                //// Making a first sync, will initialize everything we need
                //var se = await Assert.ThrowsExceptionAsync<SyncException>(() => agent.SynchronizeAsync());

                //Assert.AreEqual(SyncSide.ClientSide, se.Side);
                //Assert.AreEqual("OutOfDateException", se.TypeName);

                // Intercept outdated event, and make a reinitialize with upload action
                agent.LocalOrchestrator.OnOutdated(oa => oa.Action = OutdatedAction.ReinitializeWithUpload);

                var r = await agent.SynchronizeAsync();
                var c = GetServerDatabaseRowsCount(this.Server);
                Assert.AreEqual(c, r.TotalChangesDownloaded);
                Assert.AreEqual(2, r.TotalChangesUploaded);

            }


        }

        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public virtual async Task Handling_DifferentScopeNames(SyncOptions options)
        {
            // create a server db and seed it
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);
            this.WebServerOrchestrator.ScopeName = "customScope1";

            // Execute a sync on all clients and check results
            foreach (var client in this.Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options, "customScope1");

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
            }
        }

        /// <summary>
        /// Try to get changes from server without making a first sync
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task GetChanges_BeforeServerIsInitialized(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // ----------------------------------
            // Get changes
            // ----------------------------------
            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                // Ensure scope is created locally
                var clientScope = await agent.LocalOrchestrator.GetClientScopeAsync();

                // get changes from server, without any changes sent from client side
                var changes = await webClientOrchestrator.GetChangesAsync(clientScope);

                Assert.AreEqual(rowsCount, changes.ServerChangesSelected.TotalChangesSelected);
            }
        }

        /// <summary>
        /// Try to get changes from server without making a first sync
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task GetChanges_AfterServerIsInitialized(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }


            // ----------------------------------
            // Add rows on server AFTER first sync (so everything should be initialized)
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

                await ctx.SaveChangesAsync();
            }

            // ----------------------------------
            // Get changes
            // ----------------------------------
            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                // Ensure scope is created locally
                var clientScope = await agent.LocalOrchestrator.GetClientScopeAsync();

                // get changes from server, without any changes sent from client side
                var changes = await webClientOrchestrator.GetChangesAsync(clientScope);

                Assert.AreEqual(2, changes.ServerChangesSelected.TotalChangesSelected);

            }
        }


        /// <summary>
        /// Try to get changes from server without making a first sync
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task GetEstimatedChangesCount_BeforeServerIsInitialized(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // ----------------------------------
            // Get changes
            // ----------------------------------
            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                // Ensure scope is created locally
                var clientScope = await agent.LocalOrchestrator.GetClientScopeAsync();

                // get changes from server, without any changes sent from client side
                var changes = await webClientOrchestrator.GetEstimatedChangesCountAsync(clientScope);

                Assert.AreEqual(rowsCount, changes.ServerChangesSelected.TotalChangesSelected);
            }
        }

        /// <summary>
        /// Try to get changes from server without making a first sync
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task GetEstimatedChangesCount_AfterServerIsInitialized(SyncOptions options)
        {
            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }


            // ----------------------------------
            // Add rows on server AFTER first sync (so everything should be initialized)
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

                await ctx.SaveChangesAsync();
            }

            // ----------------------------------
            // Get changes
            // ----------------------------------
            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                // Ensure scope is created locally
                var clientScope = await agent.LocalOrchestrator.GetClientScopeAsync();

                // get changes from server, without any changes sent from client side
                var changes = await webClientOrchestrator.GetEstimatedChangesCountAsync(clientScope);

                Assert.AreEqual(2, changes.ServerChangesSelected.TotalChangesSelected);

            }
        }

        [TestMethod]
        public async Task WithBatchingEnabled_WhenSessionIsLostDuringApplyChanges_ChangesAreNotLost()
        {
            // Arrange
            var options = new SyncOptions { BatchSize = 100 };

            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // insert 1000 new products so batching is used
            var rowsToSend = 1000;
            var productNumber = "12345";

            foreach (var client in Clients)
            {
                var products = Enumerable.Range(1, rowsToSend).Select(i =>
                    new Product { ProductId = Guid.NewGuid(), Name = Guid.NewGuid().ToString("N"), ProductNumber = productNumber + $"_{i}_{client.ProviderType}" });

                using var clientDbCtx = new DataContext(client, this.UseFallbackSchema);
                clientDbCtx.Product.AddRange(products);
                await clientDbCtx.SaveChangesAsync();
            }

            // for each client, fake that the sync session is interrupted
            var clientCount = 0;
            foreach (var client in Clients)
            {
                var batchIndex = 0;

                var orch = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, orch, options);

                this.WebServerOrchestrator.OnHttpSendingResponse(async args =>
                {
                    // SendChangesInProgress is occuring when server is receiving data from client
                    // We are droping session on the second batch
                    if (args.HttpStep == HttpStep.SendChangesInProgress && batchIndex == 1)
                    {
                        args.HttpContext.Session.Clear();
                        await args.HttpContext.Session.CommitAsync();
                    }

                    batchIndex++;

                });

                var ex = await Assert.ThrowsExceptionAsync<HttpSyncWebException>(() => agent.SynchronizeAsync());

                this.WebServerOrchestrator.OnHttpSendingResponse(null);

                // Assert
                Assert.IsNotNull(ex); //"exception required!"
                Assert.AreEqual("HttpSessionLostException", ex.TypeName);

                // Act 2: Ensure client can recover
                var agent2 = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s2 = await agent2.SynchronizeAsync();

                Assert.AreEqual(rowsToSend, s2.TotalChangesUploaded);
                Assert.AreEqual(rowsToSend * clientCount, s2.TotalChangesDownloaded);
                Assert.AreEqual(0, s2.TotalResolvedConflicts);

                clientCount++;

                using var serverDbCtx = new DataContext(this.Server);
                var serverCount = serverDbCtx.Product.Count(p => p.ProductNumber.Contains($"{productNumber}_"));
                Assert.AreEqual(rowsToSend * clientCount, serverCount);

            }

        }

        [TestMethod]
        public async Task WithBatchingEnabled_WhenSessionIsLostDuringGetChanges_ChangesAreNotLost()
        {
            // Arrange
            var options = new SyncOptions { BatchSize = 100 };

            // create a server schema with seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Execute a sync on all clients and check results
            foreach (var client in Clients)
            {
                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }

            // insert 1000 new products so batching is used
            var rowsToReceive = 1000;
            var productNumber = "12345";

            var products = Enumerable.Range(1, rowsToReceive).Select(i =>
                new Product { ProductId = Guid.NewGuid(), Name = Guid.NewGuid().ToString("N"), ProductNumber = productNumber + $"_{i}" });

            using (var serverDbCtx = new DataContext(this.Server))
            {
                serverDbCtx.Product.AddRange(products);
                await serverDbCtx.SaveChangesAsync();
            }

            // for each client, fake that the sync session is interrupted
            foreach (var client in Clients)
            {
                var batchIndex = 0;

                // restreint parallelism degrees to be sure the batch index is not downloaded at the end
                // (This will not raise the error if the batchindex 1 is downloaded as the last part)
                var orch = new WebClientOrchestrator(_versionService, this.ServiceUri, maxDownladingDegreeOfParallelism: 1);
                var agent = new SyncAgent(_versionService, client.Provider, orch, options);

                // IMPORTANT: Simulate server-side session loss after first batch message is already transmitted
                this.WebServerOrchestrator.OnHttpSendingResponse(async args =>
                {
                    // GetMoreChanges is occuring when server is sending back data to client
                    // We are droping session on the second batch
                    if (args.HttpStep == HttpStep.GetMoreChanges && batchIndex == 1)
                    {
                        args.HttpContext.Session.Clear();
                        await args.HttpContext.Session.CommitAsync();
                    }

                    if (args.HttpStep == HttpStep.GetMoreChanges)
                        batchIndex++;
                });

                var ex = await Assert.ThrowsExceptionAsync<HttpSyncWebException>(async () =>
                {
                    var r = await agent.SynchronizeAsync();
                });

                // Assert
                Assert.IsNotNull(ex); //"exception required!"
                Assert.AreEqual("HttpSessionLostException", ex.TypeName);

                this.WebServerOrchestrator.OnHttpSendingResponse(null);

                // Act 2: Ensure client can recover
                var agent2 = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);

                var s2 = await agent2.SynchronizeAsync();

                Assert.AreEqual(0, s2.TotalChangesUploaded);
                Assert.AreEqual(rowsToReceive, s2.TotalChangesDownloaded);
                Assert.AreEqual(0, s2.TotalResolvedConflicts);

                using var clientDbCtx = new DataContext(client, this.UseFallbackSchema);
                var serverCount = clientDbCtx.Product.Count(p => p.ProductNumber.Contains($"{productNumber}_"));
                Assert.AreEqual(rowsToReceive, serverCount);
            }

        }

        /// <summary>
        /// Insert one row on server, should be correctly sync on all clients
        /// </summary>
        [DataTestMethod]
        [DataRow(typeof(SyncOptionsData))]
        public async Task Parallel_Sync_For_TwentyClients(SyncOptions options)
        {
            // create a server database
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // Provision server, to be sure no clients will try to do something that could break server
            var remoteOrchestrator = new RemoteOrchestrator(_versionService, this.Server.Provider, options, new SyncSetup(Tables));

            // Ensure schema is ready on server side. Will create everything we need (triggers, tracking, stored proc, scopes)
            var scope = await remoteOrchestrator.EnsureSchemaAsync();

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);


            var providers = this.Clients.Select(c => c.ProviderType).Distinct();
            var createdDatabases = new List<(ProviderType ProviderType, string DatabaseName)>();

            var clientProviders = new List<CoreProvider>();
            foreach (var provider in providers)
                for (var i = 0; i < 10; i++)
                {
                    // Create the provider
                    var dbCliName = _databaseHelper.GetRandomName("http_cli_");
                    var localProvider = this.CreateProvider(provider, dbCliName);

                    clientProviders.Add(localProvider);

                    // Create the database
                    await this.CreateDatabaseAsync(provider, dbCliName, true);
                    createdDatabases.Add((provider, dbCliName));
                }

            var allTasks = new List<Task<SyncResult>>();

            // Execute a sync on all clients and add the task to a list of tasks
            foreach (var clientProvider in clientProviders)
            {
                var agent = new SyncAgent(_versionService, clientProvider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                allTasks.Add(agent.SynchronizeAsync());
            }

            // Await all tasks
            await Task.WhenAll(allTasks);

            foreach (var s in allTasks)
            {
                Assert.AreEqual(rowsCount, s.Result.TotalChangesDownloaded);
                Assert.AreEqual(0, s.Result.TotalChangesUploaded);
                Assert.AreEqual(0, s.Result.TotalResolvedConflicts);
            }


            // Create a new product on server 
            var name = _databaseHelper.GetRandomName();
            var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

            var product = new Product { ProductId = Guid.NewGuid(), Name = name, ProductNumber = productNumber };

            using (var serverDbCtx = new DataContext(this.Server))
            {
                serverDbCtx.Product.Add(product);
                await serverDbCtx.SaveChangesAsync();
            }

            allTasks = new List<Task<SyncResult>>();

            // Execute a sync on all clients to get the new server row
            foreach (var clientProvider in clientProviders)
            {
                var agent = new SyncAgent(_versionService, clientProvider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                allTasks.Add(agent.SynchronizeAsync());
            }

            // Await all tasks
            await Task.WhenAll(allTasks);

            foreach (var s in allTasks)
            {
                Assert.AreEqual(1, s.Result.TotalChangesDownloaded);
                Assert.AreEqual(0, s.Result.TotalChangesUploaded);
                Assert.AreEqual(0, s.Result.TotalResolvedConflicts);
            }

            foreach (var db in createdDatabases)
                _databaseHelper.DropDatabase(db.DatabaseName);

        }



        /// <summary>
        /// Insert one row on server, should be correctly sync on all clients
        /// </summary>
        [TestMethod]
        public async Task Intermitent_Connection_SyncPolicy_RetryOnHttpGettingRequest_ShouldWork()
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);


            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            var interrupted = new Dictionary<HttpStep, bool>();

            // When Server Orchestrator send back the response, we will make an interruption
            this.WebServerOrchestrator.OnHttpGettingRequest(args =>
            {
                if (!interrupted.ContainsKey(args.HttpStep))
                    interrupted.Add(args.HttpStep, false);

                // interrupt each step to see if it's working
                if (!interrupted[args.HttpStep])
                {
                    interrupted[args.HttpStep] = true;
                    throw new TimeoutException($"Timeout exception raised on step {args.HttpStep}");
                }

            });

            var options = new SyncOptions { BatchSize = 10 };

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);

                var policyRetries = 0;
                webClientOrchestrator.OnHttpPolicyRetrying(args =>
                {
                    policyRetries++;
                });

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
                Assert.AreEqual(5, policyRetries);
                interrupted.Clear();
            }

            this.WebServerOrchestrator.OnHttpGettingRequest(null);

        }

        /// <summary>
        /// On Intermittent connection, should work even if server has done its part
        /// </summary>
        [TestMethod]
        public async Task Intermitent_Connection_SyncPolicy_RetryOnHttpSendingResponse_ShouldWork()
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);


            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            var interrupted = new Dictionary<HttpStep, bool>();

            // When Server Orchestrator send back the response, we will make an interruption
            this.WebServerOrchestrator.OnHttpSendingResponse(args =>
            {
                if (!interrupted.ContainsKey(args.HttpStep))
                    interrupted.Add(args.HttpStep, false);

                // interrupt each step to see if it's working
                if (!interrupted[args.HttpStep])
                {
                    interrupted[args.HttpStep] = true;
                    throw new TimeoutException($"Timeout exception raised on step {args.HttpStep}");
                }

            });

            var options = new SyncOptions { BatchSize = 10 };

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);

                var policyRetries = 0;
                webClientOrchestrator.OnHttpPolicyRetrying(args =>
                {
                    policyRetries++;
                });

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
                Assert.AreEqual(5, policyRetries);
                interrupted.Clear();
            }

            this.WebServerOrchestrator.OnHttpSendingResponse(null);
        }


        /// <summary>
        /// On Intermittent connection, should work even if server has already applied a batch  and then timeout for some reason 
        /// Client will resend the batch again, but that's ok, since we are merging
        /// </summary>
        [TestMethod]
        public async Task Intermitent_Connection_SyncPolicy_InsertClientRow_ShouldWork()
        {
            // create a server schema without seeding
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, true, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // Get count of rows
            var rowsCount = this.GetServerDatabaseRowsCount(this.Server);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            var options = new SyncOptions { BatchSize = 10 };

            // Execute a sync on all clients to initialize client and server schema 
            foreach (var client in Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);

                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator, options);

                var s = await agent.SynchronizeAsync();

                Assert.AreEqual(rowsCount, s.TotalChangesDownloaded);
                Assert.AreEqual(0, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);
            }


            // Insert one line on each client
            foreach (var client in Clients)
            {
                using var cliCtx = new DataContext(client, this.UseFallbackSchema);
                for (var i = 0; i < 1000; i++)
                {
                    var name = _databaseHelper.GetRandomName();
                    var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                    var product = new Product { ProductId = Guid.NewGuid(), Name = name, ProductNumber = productNumber };

                    cliCtx.Product.Add(product);
                }
                await cliCtx.SaveChangesAsync();
            }


            // Sync all clients
            // First client  will upload one line and will download nothing
            // Second client will upload one line and will download one line
            // thrid client  will upload one line and will download two lines
            var download = 0;
            foreach (var client in Clients)
            {
                var interruptedBatch = false;
                // When Server Orchestrator send back the response, we will make an interruption
                this.WebServerOrchestrator.OnHttpSendingResponse(args =>
                {
                    // Throw error when sending changes to server
                    if (args.HttpStep == HttpStep.SendChangesInProgress && !interruptedBatch)
                    {
                        interruptedBatch = true;
                        throw new TimeoutException($"Timeout exception raised on step {args.HttpStep}");
                    }

                });

                var agent = new SyncAgent(_versionService, client.Provider, new WebClientOrchestrator(_versionService, this.ServiceUri), options);
                var s = await agent.SynchronizeAsync();

                this.WebServerOrchestrator.OnHttpSendingResponse(null);

                Assert.AreEqual(download, s.TotalChangesDownloaded);
                Assert.AreEqual(1000, s.TotalChangesUploaded);
                Assert.AreEqual(0, s.TotalResolvedConflicts);

                // We have one batch that has been sent 2 times; it will be merged correctly on server
                Assert.IsTrue(s.ChangesAppliedOnServer.TotalAppliedChanges >= 1001);
                Assert.IsTrue(s.ChangesAppliedOnServer.TotalAppliedChanges <= 1050);
                Assert.AreEqual(1000, s.ClientChangesSelected.TotalChangesSelected);

                download += 1000;
            }
        }


        /// <summary>
        /// Testing if blob are consistent across sync
        /// </summary>
        [TestMethod]
        public virtual async Task Blob_ShouldBeConsistent_AndSize_ShouldBeMaintained()
        {
            // create a server db and seed it
            await this.EnsureDatabaseSchemaAndSeedAsync(this.Server, false, UseFallbackSchema);

            // create empty client databases
            foreach (var client in this.Clients)
                await this.CreateDatabaseAsync(client.ProviderType, client.DatabaseName, true);

            // configure server orchestrator
            this.WebServerOrchestrator.Setup.Tables.AddRange(Tables);

            // Execute a sync on all clients to initialize schemas
            foreach (var client in this.Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);
                var s = await agent.SynchronizeAsync();
            }

            // Create a new product on server with a big thumbnail photo
            var name = _databaseHelper.GetRandomName();
            var productNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = name,
                ProductNumber = productNumber,
                ThumbNailPhoto = new byte[20000]
            };

            using (var serverDbCtx = new DataContext(this.Server))
            {
                serverDbCtx.Product.Add(product);
                await serverDbCtx.SaveChangesAsync();
            }

            // Create a new product on client with a big thumbnail photo
            foreach (var client in this.Clients)
            {
                var clientName = _databaseHelper.GetRandomName();
                var clientProductNumber = _databaseHelper.GetRandomName().ToUpperInvariant().Substring(0, 10);

                var clientProduct = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = clientName,
                    ProductNumber = clientProductNumber,
                    ThumbNailPhoto = new byte[20000]
                };

                using (var clientDbCtx = new DataContext(client, UseFallbackSchema))
                {
                    clientDbCtx.Product.Add(product);
                    await clientDbCtx.SaveChangesAsync();
                }
            }
            // Two sync to be sure all clients have all rows from all
            foreach (var client in this.Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);
                var s = await agent.SynchronizeAsync();
            }
            foreach (var client in this.Clients)
            {
                var webClientOrchestrator = new WebClientOrchestrator(_versionService, this.ServiceUri);
                var agent = new SyncAgent(_versionService, client.Provider, webClientOrchestrator);
                var s = await agent.SynchronizeAsync();
            }


            // check rows count on server and on each client
            using (var ctx = new DataContext(this.Server))
            {
                var products = await ctx.Product.AsNoTracking().ToListAsync();
                foreach (var p in products)
                    Assert.AreEqual(20000, p.ThumbNailPhoto.Length);

            }

            foreach (var client in Clients)
            {
                using var cliCtx = new DataContext(client, this.UseFallbackSchema);

                var products = await cliCtx.Product.AsNoTracking().ToListAsync();
                foreach (var p in products)
                    Assert.AreEqual(20000, p.ThumbNailPhoto.Length);
            }
        }

    }
}
