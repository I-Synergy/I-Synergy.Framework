using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.SqlServer.Orchestrations.Tests
{
    public partial class BaseOrchestratorTests
    {
        [Ignore]
        public async Task BaseOrchestrator_StoredProcedure_ShouldCreate()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                StoredProceduresPrefix = "sp_",
                StoredProceduresSuffix = "_sp"
            };

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, false);

            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges));

            // Adding a filter to check if stored procedures "with filters" are also generated
            setup.Filters.Add("Product", "ProductCategoryID", "SalesLT");

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChangesWithFilters, false);

            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChangesWithFilters));



            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task BaseOrchestrator_StoredProcedure_ShouldOverwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                StoredProceduresPrefix = "sp_",
                StoredProceduresSuffix = "_sp"
            };

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var storedProcedureSelectChanges = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_changes";

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, false);

            var assertOverWritten = false;

            localOrchestrator.On(new Action<StoredProcedureCreatingArgs>(args =>
            {
                assertOverWritten = true;
            }));

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, true);

            Assert.IsTrue(assertOverWritten);

            _databaseHelper.DropDatabase(dbName);
        }


        [Ignore]
        public async Task BaseOrchestrator_StoredProcedure_ShouldNotOverwrite()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                StoredProceduresPrefix = "sp_",
                StoredProceduresSuffix = "_sp"
            };

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var storedProcedureSelectChanges = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_changes";

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, false);

            var assertOverWritten = false;

            localOrchestrator.On(new Action<StoredProcedureCreatingArgs>(args =>
            {
                assertOverWritten = true;
            }));

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, false);

            Assert.IsFalse(assertOverWritten);

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_StoredProcedure_Exists()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                StoredProceduresPrefix = "sp_",
                StoredProceduresSuffix = "_sp"
            };

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            var storedProcedureSelectChanges = $"SalesLT.{setup.StoredProceduresPrefix}Product{setup.StoredProceduresSuffix}_changes";

            await localOrchestrator.CreateStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges, false);

            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges));
            Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.UpdateRow));

            _databaseHelper.DropDatabase(dbName);
        }

        [Ignore]
        public async Task BaseOrchestrator_StoredProcedures_ShouldCreate()
        {
            var dbName = _databaseHelper.GetRandomName("tcp_lo_");
            await _databaseHelper.CreateDatabaseAsync(dbName, true);

            var cs = _databaseHelper.GetConnectionString(dbName);
            var sqlProvider = new SqlSyncProvider(cs);

            // Create default table
            var ctx = new DataContext((dbName, sqlProvider), true, false);
            await ctx.Database.EnsureCreatedAsync();

            var scopeName = "scope";

            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "SalesLT.Product" })
            {
                StoredProceduresPrefix = "sp_",
                StoredProceduresSuffix = "_sp"
            };

            var localOrchestrator = new LocalOrchestrator(_versionService, sqlProvider, options, setup, scopeName);

            await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"]);

            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.BulkDeleteRows));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.BulkTableType));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.BulkUpdateRows));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.DeleteMetadata));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.DeleteRow));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.Reset));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChanges));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectInitializedChanges));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectRow));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.UpdateRow));

            Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChangesWithFilters));
            Assert.IsFalse(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectInitializedChangesWithFilters));

            // Adding a filter to check if stored procedures "with filters" are also generated
            setup.Filters.Add("Product", "ProductCategoryID", "SalesLT");

            await localOrchestrator.CreateStoredProceduresAsync(setup.Tables["Product", "SalesLT"], false);

            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectChangesWithFilters));
            Assert.IsTrue(await localOrchestrator.ExistStoredProcedureAsync(setup.Tables["Product", "SalesLT"], DbStoredProcedureType.SelectInitializedChangesWithFilters));


            _databaseHelper.DropDatabase(dbName);
        }



    }
}
