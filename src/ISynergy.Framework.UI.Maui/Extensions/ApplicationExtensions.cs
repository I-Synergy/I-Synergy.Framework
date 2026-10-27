using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.UI.Abstractions.Styles;
using ISynergy.Framework.UI.Styles;
using Microsoft.Extensions.Logging;

#if WINDOWS
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
#endif

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for MAUI Application to manage dynamic theme switching.
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Sets the application color theme dynamically.
    /// </summary>
    /// <param name="application">The MAUI application instance.</param>
    /// <param name="color">The hex color string (e.g., "#FFB900").</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    /// <returns>The application instance for method chaining.</returns>
    /// <remarks>
    /// Theme detection uses the <see cref="Abstractions.Styles.IThemeResourceDictionary"/>
    /// marker interface to identify existing theme resource dictionaries. This is AOT-safe and robust
    /// under IL trimming because the interface check relies on a static type test rather than a
    /// fragile <c>GetType().Namespace</c> string comparison.
    /// </remarks>
    public static Microsoft.Maui.Controls.Application SetApplicationColor(
        this Microsoft.Maui.Controls.Application application,
        string color,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentException.ThrowIfNullOrWhiteSpace(color);

        logger?.LogInformation("Setting application color to {Color}", color);

        // Remove existing theme dictionaries using the IThemeResourceDictionary marker interface.
        // This is AOT-safe: an interface check on an allocated object is always preserved by the
        // linker, unlike a fragile GetType().Namespace string comparison.
        var themesToRemove = application.Resources.MergedDictionaries
            .Where(dict => dict is IThemeResourceDictionary)
            .ToList();

        foreach (var theme in themesToRemove)
        {
            application.Resources.MergedDictionaries.Remove(theme);
            logger?.LogTrace("Removed existing theme dictionary: {ThemeType}", theme.GetType().Name);
        }

        var primaryColor = Color.FromArgb(color);
        var themeDict = new DynamicThemeResourceDictionary(primaryColor);
        application.Resources.MergedDictionaries.Add(themeDict);
        logger?.LogInformation("Applied dynamic theme for color {Color}", color);

        return application;
    }


#if WINDOWS
    /// <summary>
    /// Attempts to read the environment name from the process launch arguments.
    /// Parses <c>--environment &lt;name&gt;</c> or <c>-e &lt;name&gt;</c>.
    /// </summary>
    /// <returns>The environment name if found in launch args; otherwise <see langword="null"/>.</returns>
    public static string? TryGetEnvironmentFromLaunchArgs()
    {
        try
        {
            var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (activationArgs.Kind == ExtendedActivationKind.Launch
                && activationArgs.Data is LaunchActivatedEventArgs launchArgs
                && !string.IsNullOrEmpty(launchArgs.Arguments))
            {
                var parsed = launchArgs.Arguments.ParseCommandLineArgs();
                return parsed.TryGetValue("environment", out var env) ? env
                    : parsed.TryGetValue("e", out var e) ? e
                    : null;
            }
        }
        catch
        {
            // AppInstance may not be available this early in the lifecycle
        }
        return null;
    }
#else
    public static string? TryGetEnvironmentFromLaunchArgs() => null;
#endif
}
