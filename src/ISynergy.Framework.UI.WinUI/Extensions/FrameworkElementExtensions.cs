using ISynergy.Framework.Core.Enumerations;
using Microsoft.UI.Xaml;
using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for <see cref="FrameworkElement"/> to apply flow direction based on culture.
/// </summary>
public static class FrameworkElementExtensions
{
    /// <summary>
    /// Applies the appropriate <see cref="Microsoft.UI.Xaml.FlowDirection"/> to the element based on the specified culture name.
    /// A <c>null</c> or whitespace culture name is treated as left-to-right.
    /// </summary>
    /// <typeparam name="T">The type of the framework element.</typeparam>
    /// <param name="element">The framework element to apply flow direction to.</param>
    /// <param name="cultureName">The culture name (e.g., "ar-SA", "en-US"); may be <c>null</c>.</param>
    /// <returns>The element instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is <c>null</c>.</exception>
    /// <exception cref="CultureNotFoundException">Thrown when <paramref name="cultureName"/> is not a valid culture name.</exception>
    public static T ApplyFlowDirection<T>(this T element, string? cultureName) where T : FrameworkElement
    {
        ArgumentNullException.ThrowIfNull(element);

        element.FlowDirection = !string.IsNullOrWhiteSpace(cultureName)
            && CultureInfo.GetCultureInfo(cultureName!).TextInfo.IsRightToLeft
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;

        return element;
    }

    /// <summary>
    /// Applies the <see cref="ElementTheme"/> matching the specified <see cref="Themes"/> value.
    /// </summary>
    /// <remarks>
    /// <see cref="FrameworkElement.RequestedTheme"/> applies to a single root element and is not
    /// inherited across independent roots, so it has to be re-applied whenever the window content
    /// (or a new root) is created. A root left on <see cref="ElementTheme.Default"/> follows the
    /// operating system theme.
    /// </remarks>
    /// <typeparam name="T">The type of the framework element.</typeparam>
    /// <param name="element">The framework element to apply the theme to.</param>
    /// <param name="theme">The application theme.</param>
    /// <returns>The element instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is <c>null</c>.</exception>
    public static T ApplyRequestedTheme<T>(this T element, Themes theme) where T : FrameworkElement
    {
        ArgumentNullException.ThrowIfNull(element);

        element.RequestedTheme = theme switch
        {
            Themes.Light => ElementTheme.Light,
            Themes.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        return element;
    }
}
