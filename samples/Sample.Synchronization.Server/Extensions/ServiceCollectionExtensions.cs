using ISynergy.Framework.AspNetCore.Synchronization.Extensions;
using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Providers;
using Sample.Synchronization.Common.Options;
using System.Reflection;

namespace Sample.Synchronization.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSyncService(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));
            services.Configure<ServerSynchronizationOptions>(configuration.GetSection(nameof(ServerSynchronizationOptions)));

            // Tables involved in the sync process:
            var connectionString = configuration.GetConnectionString("ConnectionString");
            logger.LogDebug(connectionString);

            var options = configuration.GetSection(nameof(ServerSynchronizationOptions)).Get<ServerSynchronizationOptions>();
            var tables = options.Tables;

            logger.LogDebug(tables.ToString());

            // Create standard Setup and Options
            var syncOptions = new SyncOptions
            {
                BatchSize = 2000
            };

            var versionService = new VersionService(Assembly.GetAssembly(typeof(Startup)));
            var syncSetup = new SyncSetup(tables);

            // create provider.
            var syncProvider = new SqlSyncChangeTrackingProvider(connectionString);

            // This method is useful if you want to provision by yourself the server database
            // You will need to :
            // - Create a remote orchestrator with the correct setup to create
            // - Provision everything

            // Create a server orchestrator used to Deprovision and Provision only tables
            var webserverOrchestrator = new WebServerOrchestrator(versionService, syncProvider, syncOptions, syncSetup);

            webserverOrchestrator.DeprovisionAsync().GetAwaiter().GetResult();

            // Provision everything needed (sp, triggers, tracking tables)
            // Internally provision will fectch the schema a will return it to the caller.
            var newSchema = webserverOrchestrator.ProvisionAsync().GetAwaiter().GetResult();

            /// Add a SqlSyncProvider acting as the server hub.
            return services.AddSyncServer(versionService, webserverOrchestrator);
        }
    }
}
