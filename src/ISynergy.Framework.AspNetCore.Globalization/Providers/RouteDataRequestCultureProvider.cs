using ISynergy.Framework.AspNetCore.Globalization.Options;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.Globalization.Providers;

/// <summary>
/// Class RouteDataRequestCultureProvider.
/// Implements the <see cref="RequestCultureProvider" />
/// </summary>
/// <seealso cref="RequestCultureProvider" />
public class RouteDataRequestCultureProvider : RequestCultureProvider
{
    private readonly GlobalizationOptions _options;

    /// <summary>
    /// Constructor that takes RequestLocalizationOptions to access supported cultures
    /// </summary>
    /// <param name="options">The localization options containing supported cultures</param>
    public RouteDataRequestCultureProvider(IOptions<GlobalizationOptions> options)
    {
        Argument.IsNotNull(options);

        _options = options.Value;
    }

    /// <summary>
    /// Determines the provider culture result.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>Task&lt;ProviderCultureResult&gt;.</returns>
    /// <exception cref="ArgumentNullException">httpContext</exception>
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        Argument.IsNotNull(httpContext);

        // Create the default result task based on configuration
        var defaultResultTask = Task.FromResult<ProviderCultureResult?>(
            new ProviderCultureResult(_options.DefaultCulture, _options.DefaultCulture));

        // Get the path value
        var path = httpContext.Request.Path.Value;

        // Handle empty paths or root URL
        if (string.IsNullOrEmpty(path) || path == "/")
            return defaultResultTask;

        // Split the path and get the first segment
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // If no segments, return default
        if (segments.Length == 0)
            return defaultResultTask;

        // Get the culture from the first segment
        var culture = segments[0];

        // Check if the culture is supported
        if (_options.SupportedCultures == null || !_options.SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase))
            return defaultResultTask;

        try
        {
            var providerResultCulture = new ProviderCultureResult(culture, culture);
            return Task.FromResult<ProviderCultureResult?>(providerResultCulture);
        }
        catch (Exception)
        {
            return defaultResultTask;
        }
    }
}
