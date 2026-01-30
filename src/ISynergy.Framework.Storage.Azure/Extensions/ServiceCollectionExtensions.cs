using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Services;
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
    /// <returns></returns>
    public static IServiceCollection AddAzureStorageIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<IStorageService, AzureStorageService>();
        return services;
    }
}
