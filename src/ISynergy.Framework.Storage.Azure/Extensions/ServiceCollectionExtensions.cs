using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Options;
using ISynergy.Framework.Storage.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Storage.Azure.Extensions;

/// <summary>
/// Service collection extensions for Azure storage.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure Blob Storage integration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    [RequiresUnreferencedCode("Calls services.Configure<AzureStorageOptions> which uses ConfigurationBinder.Bind with reflection.")]
    [RequiresDynamicCode("Calls services.Configure<AzureStorageOptions> which requires dynamic code generation at runtime.")]
    public static IServiceCollection AddAzureStorageIntegration(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
    {
        Argument.IsNotNull(services);
        Argument.IsNotNull(configuration);

        services.AddOptions();
        services.Configure<AzureStorageOptions>(configuration.GetSection($"{prefix}{nameof(AzureStorageOptions)}"));
        services.TryAddSingleton<IStorageService, AzureStorageService>();
        return services;
    }
}
