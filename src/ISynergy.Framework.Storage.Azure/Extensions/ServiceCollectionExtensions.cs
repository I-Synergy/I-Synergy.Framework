using ISynergy.Framework.Storage.Abstractions.Options;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Storage.Azure.Extensions;

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
    /// <returns></returns>
    public static IServiceCollection AddStorageAzureIntegration<TStorageOptions>(this IServiceCollection services, IConfiguration configuration)
        where TStorageOptions : class, IStorageOptions, new()
    {
        services.TryAddSingleton<IStorageService, StorageService<TStorageOptions>>();
        return services;
    }
}
