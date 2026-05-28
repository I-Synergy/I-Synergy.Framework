using ISynergy.Framework.AspNetCore.Globalization.Constraints;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using ISynergy.Framework.AspNetCore.Globalization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;
public static class ServiceCollectionExtensions
{
    [RequiresUnreferencedCode("Calls services.Configure<GlobalizationOptions> which uses ConfigurationBinder.Bind with reflection.")]
    [RequiresDynamicCode("Calls services.Configure<GlobalizationOptions> which requires dynamic code generation at runtime.")]
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
