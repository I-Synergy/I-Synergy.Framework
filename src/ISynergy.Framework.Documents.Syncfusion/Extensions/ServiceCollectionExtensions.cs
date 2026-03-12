using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Documents.Abstractions.Services;
using ISynergy.Framework.Documents.Options;
using ISynergy.Framework.Documents.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Documents.Extensions;

/// <summary>
/// Service collection extensions for Syncfusion document service.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Syncfusion document service integration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="prefix">Optional configuration section prefix.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> This method registers <see cref="DocumentService"/>, which depends
    /// on Syncfusion DocIO, DocIORenderer, and XlsIO libraries. Those libraries use reflection internally for
    /// MailMerge and document processing and require dynamic code generation. Applications targeting
    /// <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> cannot use this library and should implement a native
    /// document generation alternative. Monitor Syncfusion release notes for future AOT/NativeAOT support.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("Registers DocumentService which requires Syncfusion reflection-based libraries. Not compatible with AOT publishing.")]
    [RequiresDynamicCode("Syncfusion libraries require dynamic code generation.")]
    public static IServiceCollection AddDocumentsSyncfusionIntegration(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.AddOptions();
        services.Configure<SyncfusionLicenseOptions>(configuration.GetSection($"{prefix}{nameof(SyncfusionLicenseOptions)}").BindWithReload);
        services.TryAddSingleton<ILanguageService, LanguageService>();
        services.TryAddSingleton<IDocumentService, DocumentService>();

        return services;
    }
}
