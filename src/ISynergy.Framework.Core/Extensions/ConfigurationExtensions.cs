using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class ConfigurationBinderExtensions.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Binds the with reload.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="instance">The instance.</param>
    public static void BindWithReload(this IConfiguration configuration, object instance)
    {
        try
        {
            // Use BinderOptions to ignore errors
            var options = new BinderOptions
            {
                ErrorOnUnknownConfiguration = false
            };

            configuration.Bind(instance, options => options.ErrorOnUnknownConfiguration = false);
            configuration.GetReloadToken().RegisterChangeCallback((_) =>
            {
                try
                {
                    configuration.Bind(instance, options => options.ErrorOnUnknownConfiguration = false);
                }
                catch
                {
                    // Ignore binding errors during reload
                }
            }, null);
        }
        catch
        {
            // Ignore binding errors during initial binding
        }
    }

    public static LogLevel GetDefaultLogLevel(this IConfiguration configuration) =>
        configuration.GetLogLevel("Default");

    public static LogLevel GetLogLevel(this IConfiguration configuration, string category) =>
        configuration.GetSection($"LogLevel:{category}").Get<LogLevel>();
}
