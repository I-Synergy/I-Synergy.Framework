using ISynergy.Framework.Printer.Label.Abstractions.Services;
using ISynergy.Framework.Printer.Label.Dymo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Printer.Label.Dymo.Extensions;

/// <summary>
/// Service collection extensions for Dymo label printer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Dymo label printer integration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPrinterLabelDymoIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<ILabelPrinterService, LabelPrinterService>();
        return services;
    }
}
