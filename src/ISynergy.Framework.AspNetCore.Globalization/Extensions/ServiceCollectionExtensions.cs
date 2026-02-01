using ISynergy.Framework.AspNetCore.Globalization.Constraints;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using ISynergy.Framework.AspNetCore.Globalization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGlobalization(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GlobalizationOptions>(configuration.GetSection(nameof(GlobalizationOptions)));
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<ILanguageService, LanguageService>();
        services.TryAddSingleton<RouteDataRequestCultureProvider>();
        services.TryAddSingleton<CultureRouteConstraint>();
        services.AddLocalization();
        return services;
    }
}
