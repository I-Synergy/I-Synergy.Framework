using Azure.Core;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.Azure.Options;
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
    /// Adds Azure Blob Storage integration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Authentication is resolved in this order:
    /// </para>
    /// <list type="number">
    /// <item><description>
    /// A <c>BlobServiceClient</c> already registered in DI (for example by Aspire's
    /// <c>AddAzureBlobServiceClient</c>) is used as-is.
    /// </description></item>
    /// <item><description>
    /// Identity-based authentication when <c>ServiceEndpoint</c> or <c>AccountName</c>
    /// (without <c>AccountKey</c>) is configured.
    /// </description></item>
    /// <item><description>
    /// The configured <c>ConnectionString</c> (or <c>AccountName</c> plus <c>AccountKey</c>)
    /// as fallback.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <param name="credential">
    /// Optional credential used for identity-based authentication. When supplied, it is registered
    /// in DI and takes precedence over credential creation from
    /// <see cref="AzureStorageOptions.ManagedIdentityClientId"/>.
    /// </param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAzureStorageIntegration(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "",
        TokenCredential? credential = null)
    {
        Argument.IsNotNull(services);
        Argument.IsNotNull(configuration);

        services.AddOptions();
        services.Configure<AzureStorageOptions>(configuration.GetSection($"{prefix}{nameof(AzureStorageOptions)}"));

        if (credential is not null)
            services.TryAddSingleton(credential);

        services.TryAddSingleton<IStorageService, AzureStorageService>();
        return services;
    }
}
