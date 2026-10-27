using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Storage.Abstractions.Services;
using ISynergy.Framework.Storage.S3.Options;
using ISynergy.Framework.Storage.S3.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Storage.S3.Extensions;

/// <summary>
/// Service collection extensions for S3 storage.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds S3 storage integration supporting AWS S3, MinIO, and other S3-compatible providers.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> The <c>AWSSDK.S3</c> dependency uses reflection-based
    /// HTTP marshalling internally and does not currently support Native AOT publishing or aggressive trimming.
    /// Applications targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should use Azure Blob Storage
    /// (<c>ISynergy.Framework.Storage.Azure</c>) or another storage provider with AOT-compatible SDK support
    /// until AWS SDK AOT support is available.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddS3StorageIntegration(
        this IServiceCollection services,
        IConfiguration configuration,
        string prefix = "")
    {
        Argument.IsNotNull(services);
        Argument.IsNotNull(configuration);

        services.AddOptions();
        services.Configure<S3StorageOptions>(configuration.GetSection($"{prefix}{nameof(S3StorageOptions)}"));
        services.TryAddSingleton<IStorageService, S3StorageService>();
        return services;
    }
}
