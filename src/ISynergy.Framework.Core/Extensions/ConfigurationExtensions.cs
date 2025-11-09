using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class ConfigurationBinderExtensions.
/// </summary>
public static class ConfigurationExtensions
{
    // Store reload tokens to prevent memory leaks
    // Using ConditionalWeakTable ensures tokens are cleaned up when instances are GC'd
    private static readonly ConditionalWeakTable<object, ReloadTokenWrapper> _reloadTokens = new();

    /// <summary>
    /// Wrapper class to store reload tokens and allow updates
    /// </summary>
    private sealed class ReloadTokenWrapper : IDisposable
    {
        private IDisposable? _token;
        private readonly object _lock = new();

        public void SetToken(IDisposable token)
        {
            lock (_lock)
            {
                _token?.Dispose();
                _token = token;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _token?.Dispose();
                _token = null;
            }
        }
    }

    /// <summary>
    /// Gets a logger instance, with fallback to null if not available.
    /// </summary>
    private static ILogger? GetLogger()
    {
        try
        {
            return ServiceLocator.Default.ServiceProvider.GetService<ILoggerFactory>()?
                .CreateLogger(typeof(ConfigurationExtensions));
        }
        catch
        {
            // ServiceLocator may not be initialized yet
            return null;
        }
    }

    /// <summary>
    /// Binds the with reload.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="instance">The instance.</param>
    public static void BindWithReload(this IConfiguration configuration, object instance)
    {
        var logger = GetLogger();
        var instanceType = instance.GetType().Name;

        try
        {
            // Use BinderOptions to ignore errors
            var options = new BinderOptions
            {
                ErrorOnUnknownConfiguration = false
            };

            configuration.Bind(instance, options => options.ErrorOnUnknownConfiguration = false);
            
            // Register change callback and store the token for disposal
            var reloadToken = configuration.GetReloadToken().RegisterChangeCallback((_) =>
            {
                try
                {
                    configuration.Bind(instance, options => options.ErrorOnUnknownConfiguration = false);
                    logger?.LogDebug("Configuration reloaded successfully for {InstanceType}", instanceType);
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Failed to reload configuration for {InstanceType} during configuration change", instanceType);
                }
            }, null);

            // Store the token for later disposal
            // Use GetOrCreateValue to handle cases where the same instance is bound multiple times
            var wrapper = _reloadTokens.GetOrCreateValue(instance);
            wrapper.SetToken(reloadToken);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to bind configuration for {InstanceType} during initial binding", instanceType);
        }
    }

    public static LogLevel GetDefaultLogLevel(this IConfiguration configuration) =>
        configuration.GetLogLevel("Default");

    public static LogLevel GetLogLevel(this IConfiguration configuration, string category)
    {
        var logLevel = configuration.GetSection($"LogLevel:{category}").Get<LogLevel>();
        
        // Return Information as default instead of None (which is default(LogLevel))
        if (logLevel == default(LogLevel))
        {
            var logger = GetLogger();
            logger?.LogDebug("LogLevel configuration for category '{Category}' not found, defaulting to Information", category);
            return LogLevel.Information;
        }

        return logLevel;
    }
}
