using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Files.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Synchronization.Client.Services;
using Sample.Synchronization.Common.Abstractions;
using Sample.Synchronization.Common.Options;
using System.Reflection;

namespace Sample.Synchronization.Client.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSyncService(this IServiceCollection services, IConfigurationRoot config)
        {
            services.TryAddSingleton(s => config.Get<ClientSynchronizationOptions>());
            services.TryAddSingleton<IFileSynchronizationOptions>(s => config.Get<FileSynchronizationOptions>());
            services.TryAddSingleton(s => MessageService.Default);
            services.TryAddSingleton<IVersionService>(s => new VersionService(Assembly.GetAssembly(typeof(Program))));
            services.TryAddSingleton<IContext, Context>();
            services.TryAddSingleton<ISynchronizationService, SynchronizationService>();
            services.TryAddSingleton<IFileSynchronizationService, FileSynchronizationService>();

            return services;
        }
    }
}
