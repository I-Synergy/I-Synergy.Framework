using ISynergy.Framework.Update.Abstractions.Services;
using ISynergy.Framework.Update.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Update.Extensions;

/// <summary>
/// Service collection extensions for Microsoft Stor updates
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Microsoft Store Update integration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureStoreUpdateIntegration(this MauiAppBuilder builder)
    {
        builder.Services.TryAddSingleton<IUpdateService, UpdateService>();
        return builder;
    }
}