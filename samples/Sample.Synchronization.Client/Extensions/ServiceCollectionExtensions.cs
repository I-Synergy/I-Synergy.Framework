using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton(s => config.Get<ClientSynchronizationOptions>());
            services.AddSingleton(s => MessageService.Default);
            services.AddSingleton<IVersionService>(s => new VersionService(Assembly.GetAssembly(typeof(Program))));
            services.AddSingleton<IContext, Context>();
            services.AddSingleton<ISynchronizationService, SynchronizationService>();

            return services;
        }
    }
}
