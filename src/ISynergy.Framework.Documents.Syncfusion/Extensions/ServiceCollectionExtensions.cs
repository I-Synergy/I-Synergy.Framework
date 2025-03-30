using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Documents.Abstractions.Services;
using ISynergy.Framework.Documents.Configuration;
using ISynergy.Framework.Documents.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Documents.Extensions;

/// <summary>
/// Service collection extensions for Syncfusion document service.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Syncfusion document service integration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static IServiceCollection AddDocumentsSyncfusionIntegration(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.AddOptions();
        services.Configure<SyncfusionLicenseOptions>(configuration.GetSection($"{prefix}{nameof(SyncfusionLicenseOptions)}").BindWithReload);
        services.TryAddSingleton<ILanguageService, LanguageService>();
        services.TryAddSingleton<IDocumentService, DocumentService>();

        return services;
    }
}
