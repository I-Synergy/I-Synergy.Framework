using ISynergy.Framework.AspNetCore.Globalization.Constraints;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using ISynergy.Framework.AspNetCore.Globalization.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;

/// <summary>
/// Service collection extensions for globalization service
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds globalization integration.
    /// Don't use AddLocalization() in your Startup.cs, use this extension method instead.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddGlobalization(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<GlobalizationOptions>(builder.Configuration.GetSection(nameof(GlobalizationOptions)));
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.TryAddSingleton<ILanguageService, LanguageService>();
        builder.Services.TryAddSingleton<RouteDataRequestCultureProvider>();
        builder.Services.TryAddSingleton<CultureRouteConstraint>();
        builder.Services.AddLocalization();
        return builder;
    }


}
