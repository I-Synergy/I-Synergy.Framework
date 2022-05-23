using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Storage.Abstractions.Options;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Storage.Azure.Extensions
{
    /// <summary>
    /// Service collection extensions for Azure storage.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Azure storage integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static IServiceCollection AddStorageAzureIntegration<TStorageOptions>(this IServiceCollection services, IConfiguration configuration, string containerName)
            where TStorageOptions : class, IStorageOptions, new()
        {
            services.AddSingleton<IStorageService<TStorageOptions>, StorageService<TStorageOptions>>(e =>
                new StorageService<TStorageOptions>(e.GetService<IOptions<TStorageOptions>>(), containerName));

            return services;
        }

        /// <summary>
        /// Adds tenant-aware Azure storage integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantStorageAzureIntegration<TStorageOptions>(this IServiceCollection services, IConfiguration configuration)
            where TStorageOptions : class, IStorageOptions, new()
        {
            services.AddSingleton<IStorageService<TStorageOptions>, StorageService<TStorageOptions>>(e =>
                new StorageService<TStorageOptions>(e.GetService<IOptions<TStorageOptions>>(), e.GetService<ITenantService>().TenantId.ToString()));

            return services;
        }
    }
}
