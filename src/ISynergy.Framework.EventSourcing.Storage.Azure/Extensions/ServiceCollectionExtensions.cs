using Azure.Storage.Blobs;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.Azure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.EventSourcing.Storage.Azure.Extensions;

/// <summary>
/// Extension methods for registering the Azure Blob Storage event archive back-end.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="AzureBlobEventArchiveStorage"/> as the <see cref="IEventArchiveStorage"/>
    /// implementation using the provided <see cref="BlobServiceClient"/>.
    /// </summary>
    /// <remarks>
    /// Prerequisite: <see cref="BlobServiceClient"/> must already be registered in DI
    /// (e.g. via <c>builder.AddAzureBlobServiceClient("blobs")</c> in Aspire, or
    /// <c>services.AddSingleton(new BlobServiceClient(connectionString))</c> directly).
    /// Each tenant's events are isolated in a separate container named after the tenant GUID.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <returns>The <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddAzureEventArchiveStorage(
        this IServiceCollection services)
    {
        services.AddScoped<IEventArchiveStorage>(sp =>
            new AzureBlobEventArchiveStorage(
                sp.GetRequiredService<BlobServiceClient>(),
                sp.GetRequiredService<ILogger<AzureBlobEventArchiveStorage>>()));

        return services;
    }
}
