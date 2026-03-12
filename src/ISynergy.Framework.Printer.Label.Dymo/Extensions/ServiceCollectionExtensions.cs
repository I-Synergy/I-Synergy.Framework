using ISynergy.Framework.Printer.Label.Abstractions.Services;
using ISynergy.Framework.Printer.Label.Dymo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Printer.Label.Dymo.Extensions;

/// <summary>
/// Service collection extensions for Dymo label printer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Dymo label printer integration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> This method registers <see cref="LabelPrinterService"/>, which
    /// depends on DymoSDK. DymoSDK uses COM interop and reflection for printer enumeration and label access
    /// and is not compatible with Native AOT publishing. This service is Windows-only. Applications targeting
    /// <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> cannot use this library.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("Registers LabelPrinterService which depends on DymoSDK COM interop. Not compatible with AOT publishing.")]
    [RequiresDynamicCode("DymoSDK requires dynamic code generation.")]
    public static IServiceCollection AddPrinterLabelDymoIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<ILabelPrinterService, LabelPrinterService>();
        return services;
    }
}
