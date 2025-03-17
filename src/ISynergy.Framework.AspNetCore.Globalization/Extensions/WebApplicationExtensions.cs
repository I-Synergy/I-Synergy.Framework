using ISynergy.Framework.AspNetCore.Globalization.Enumerations;
using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.AspNetCore.Globalization.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds globalization and localization integration.
    /// Don't use app.UseRequestLocalization() in your Startup.cs, use this extension method instead.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseGlobalization(this WebApplication app)
    {
        var globalization = app.Services.GetRequiredService<IOptions<GlobalizationOptions>>().Value;

        app.UseRequestLocalization(options =>
        {
            // Set supported cultures
            var supportedCultures = globalization.SupportedCultures
                .Select(c => new CultureInfo(c))
                .ToArray();

            options.DefaultRequestCulture = new RequestCulture(globalization.DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;


            // Clear existing providers and add our custom provider first
            options.RequestCultureProviders.Clear();

            // Add providers based on configuration
            switch (globalization.ProviderType)
            {
                case RequestCultureProviderTypes.AcceptLanguageHeader:
                    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                    options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                    options.RequestCultureProviders.Add(app.Services.GetRequiredService<RouteDataRequestCultureProvider>());
                    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                    break;

                case RequestCultureProviderTypes.Cookie:
                    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                    options.RequestCultureProviders.Add(app.Services.GetRequiredService<RouteDataRequestCultureProvider>());
                    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                    options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                    break;

                case RequestCultureProviderTypes.QueryString:
                    options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                    options.RequestCultureProviders.Add(app.Services.GetRequiredService<RouteDataRequestCultureProvider>());
                    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                    break;

                case RequestCultureProviderTypes.Route:
                default:
                    options.RequestCultureProviders.Add(app.Services.GetRequiredService<RouteDataRequestCultureProvider>());
                    options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
                    options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                    options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                    break;
            }
        });

        return app;
    }
}
