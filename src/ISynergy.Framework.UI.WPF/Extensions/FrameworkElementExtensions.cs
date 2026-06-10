using System.Globalization;
using System.Windows;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for <see cref="FrameworkElement"/> to apply flow direction based on culture.
/// </summary>
public static class FrameworkElementExtensions
{
    /// <summary>
    /// Applies the appropriate <see cref="System.Windows.FlowDirection"/> to the element based on the specified culture name.
    /// </summary>
    /// <typeparam name="T">The type of the framework element.</typeparam>
    /// <param name="element">The framework element to apply flow direction to.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    /// <returns>The element instance for method chaining.</returns>
    public static T ApplyFlowDirection<T>(this T element, string cultureName) where T : FrameworkElement
    {
        element.FlowDirection = CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

        return element;
    }
}
