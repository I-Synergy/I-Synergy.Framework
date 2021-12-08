using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using ISynergy.Framework.AspNetCore.Synchronization.Tests.TestServer;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Abstractions.Tests;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Setup;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Tests.Http.Base
{
    public abstract class BaseTests : IDisposable
    {
        /// <summary>
        /// Gets the sync tables involved in the tests
        /// </summary>
        public abstract string[] Tables { get; }

        /// <summary>
        /// Gets the clients type we want to tests
        /// </summary>
        public abstract List<ProviderType> ClientsType { get; }

        /// <summary>
        /// Gets the server type we want to test
        /// </summary>
        public abstract ProviderType ServerType { get; }

        /// <summary>
        /// Gets if fiddler is in use
        /// </summary>
        public abstract bool UseFiddler { get; }

        /// <summary>
        /// Service Uri provided by kestrell when starts
        /// </summary>
        public string ServiceUri { get; protected set; }

        /// <summary>
        /// Gets the Web Server Orchestrator used for the tests
        /// </summary>
        public WebServerOrchestrator WebServerOrchestrator { get; protected set; }

        /// <summary>
        /// Get the server rows count
        /// </summary>
        public abstract int GetServerDatabaseRowsCount((string DatabaseName, ProviderType ProviderType, CoreProvider Provider) t);

        /// <summary>
        /// Create a provider
        /// </summary>
        public abstract CoreProvider CreateProvider(ProviderType providerType, string dbName);

        /// <summary>
        /// Create database, seed it, with or without schema
        /// </summary>
        protected abstract Task EnsureDatabaseSchemaAndSeedAsync((string DatabaseName,
            ProviderType ProviderType, CoreProvider Provider) t, bool useSeeding = false, bool useFallbackSchema = false);


        /// <summary>
        /// Create an empty database
        /// </summary>
        protected abstract Task CreateDatabaseAsync(ProviderType providerType, string dbName, bool recreateDb = true);

        protected KestrelTestServer kestrel;

        protected readonly IDatabaseHelper _databaseHelper;
        protected readonly IVersionService _versionService;

        /// <summary>
        /// Gets the remote orchestrator and its database name
        /// </summary>
        public (string DatabaseName, ProviderType ProviderType, CoreProvider Provider) Server { get; protected set; }

        /// <summary>
        /// Gets the dictionary of all local orchestrators with database name as key
        /// </summary>
        public List<(string DatabaseName, ProviderType ProviderType, CoreProvider Provider)> Clients { get; set; }

        /// <summary>
        /// Gets a bool indicating if we should generate the schema for tables
        /// </summary>
        public bool UseFallbackSchema => ServerType == ProviderType.Sql;

        /// <summary>
        /// ctor
        /// </summary>
        public BaseTests()
        {
            var versionServiceMock = new Mock<IVersionService>();
            versionServiceMock.Setup(x => x.ProductVersion).Returns(new Version(1, 0, 0));
            _versionService = versionServiceMock.Object;

            // Since we are creating a lot of databases
            // each database will have its own pool
            // Droping database will not clear the pool associated
            // So clear the pools on every start of a new test
            _databaseHelper.ClearAllPools();

            // get the server provider (and db created) without seed
            var serverDatabaseName = _databaseHelper.GetRandomName("http_sv_");

            var serverProvider = this.CreateProvider(this.ServerType, serverDatabaseName);

            // create web remote orchestrator
            this.WebServerOrchestrator = new WebServerOrchestrator(_versionService, serverProvider, new SyncOptions(), new SyncSetup());

            // public property
            this.Server = (serverDatabaseName, this.ServerType, serverProvider);

            // Create a kestrell server
            this.kestrel = new KestrelTestServer(this.WebServerOrchestrator, this.UseFiddler);

            // start server and get uri
            this.ServiceUri = this.kestrel.Run();

            // Get all clients providers
            Clients = new List<(string, ProviderType, CoreProvider)>(this.ClientsType.Count);

            // Generate Client database
            foreach (var clientType in this.ClientsType)
            {
                var dbCliName = _databaseHelper.GetRandomName("http_cli_");
                var localProvider = this.CreateProvider(clientType, dbCliName);
                this.Clients.Add((dbCliName, clientType, localProvider));
            }
        }

        /// <summary>
        /// Drop all databases used for the tests
        /// </summary>
        public virtual void Dispose()
        {
            _databaseHelper.DropDatabase(Server.DatabaseName);

            foreach (var client in Clients)
                _databaseHelper.DropDatabase(client.DatabaseName);

            this.kestrel.Dispose();
        }
    }
}
