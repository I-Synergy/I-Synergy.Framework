using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace ISynergy.Framework.AspNetCore.Synchronization.Extensions
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the server provider (inherited from IProvider) and register in the DI a WebServerOrchestrator.
        /// Use the WebServerOrchestrator in your controller, by inject it.
        /// </summary>
        /// <param name="providerType">Provider inherited from IProvider (SqlSyncProvider, MySqlSyncProvider, OracleSyncProvider) Should have [CanBeServerProvider=true] </param>
        /// <param name="serviceCollection">services collections</param>
        /// <param name="versionService"></param>
        /// <param name="connectionString">Provider connection string</param>
        /// <param name="scopeName"></param>
        /// <param name="setup">Configuration server side. Adding at least tables to be synchronized</param>
        /// <param name="options">Options, not shared with client, but only applied locally. Can be null</param>

        public static IServiceCollection AddSyncServer(
            this IServiceCollection serviceCollection,
            IVersionService versionService,
            Type providerType,
            string connectionString, 
            string scopeName = SyncOptions.DefaultScopeName, 
            SyncSetup setup = null, 
            SyncOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            options ??= new SyncOptions();
            setup = setup ?? throw new ArgumentNullException(nameof(setup));

            // Create provider
            var provider = (IProvider)Activator.CreateInstance(providerType);
            provider.ConnectionString = connectionString;

            // Create orchestrator
            var webServerOrchestrator = new WebServerOrchestrator(versionService, provider, options, setup, scopeName);
            return serviceCollection.AddSyncServer(versionService, webServerOrchestrator);
        }

        /// <summary>
        /// Add the server provider (inherited from IProvider) and register in the DI a WebServerOrchestrator.
        /// Use the WebServerOrchestrator in your controller, by inject it.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="versionService"></param>
        /// <param name="webServerOrchestrator"></param>
        /// <returns></returns>
        public static IServiceCollection AddSyncServer(
            this IServiceCollection serviceCollection, 
            IVersionService versionService,
            WebServerOrchestrator webServerOrchestrator)
        {
            var assembly = Assembly.GetAssembly(typeof(ServiceCollectionExtensions));
            serviceCollection.TryAddSingleton<IVersionService>((s) => new VersionService(assembly));
            serviceCollection.AddSingleton(webServerOrchestrator);
            return serviceCollection;
        }

        /// <summary>
        /// Add the server provider (inherited from IProvider) and register in the DI a WebServerOrchestrator.
        /// Use the WebServerOrchestrator in your controller, by inject it.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="versionService"></param>
        /// <param name="connectionString"></param>
        /// <param name="scopeName"></param>
        /// <param name="setup"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddSyncServer<TProvider>(
            this IServiceCollection serviceCollection,
            IVersionService versionService,
            string connectionString, 
            string scopeName = SyncOptions.DefaultScopeName, 
            SyncSetup setup = null, 
            SyncOptions options = null) 
            where TProvider : IProvider, new()
        => serviceCollection.AddSyncServer(versionService, typeof(TProvider), connectionString, scopeName, setup, options);

        /// <summary>
        /// Add the server provider (inherited from IProvider) and register in the DI a WebServerOrchestrator.
        /// Use the WebServerOrchestrator in your controller, by inject it.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="versionService"></param>
        /// <param name="connectionString"></param>
        /// <param name="setup"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddSyncServer<TProvider>(
            this IServiceCollection serviceCollection,
            IVersionService versionService, 
            string connectionString, 
            SyncSetup setup = null, 
            SyncOptions options = null) 
            where TProvider : IProvider, new()
             => serviceCollection.AddSyncServer(versionService, typeof(TProvider), connectionString, SyncOptions.DefaultScopeName, setup, options);

        /// <summary>
        /// Add the server provider (inherited from IProvider) and register in the DI a WebServerOrchestrator.
        /// Use the WebServerOrchestrator in your controller, by inject it.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="versionService"></param>
        /// <param name="connectionString"></param>
        /// <param name="scopeName"></param>
        /// <param name="tables"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddSyncServer<TProvider>(
            this IServiceCollection serviceCollection,
            IVersionService versionService, 
            string connectionString, 
            string scopeName = SyncOptions.DefaultScopeName, 
            string[] tables = default, 
            SyncOptions options = null) 
            where TProvider : IProvider, new()
            => serviceCollection.AddSyncServer(versionService, typeof(TProvider), connectionString, scopeName, new SyncSetup(tables), options);

        /// <summary>
        /// Add the server provider (inherited from IProvider) and register in the DI a WebServerOrchestrator.
        /// Use the WebServerOrchestrator in your controller, by inject it.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="versionService"></param>
        /// <param name="connectionString"></param>
        /// <param name="tables"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddSyncServer<TProvider>(
            this IServiceCollection serviceCollection, 
            IVersionService versionService, 
            string connectionString, 
            string[] tables = default, 
            SyncOptions options = null) 
            where TProvider : IProvider, new()
            => serviceCollection.AddSyncServer(versionService, typeof(TProvider), connectionString, SyncOptions.DefaultScopeName, new SyncSetup(tables), options);
    }
}
