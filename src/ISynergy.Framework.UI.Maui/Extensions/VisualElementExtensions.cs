using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for applying flow direction based on culture.
/// </summary>
public static class VisualElementExtensions
{
    /// <summary>
    /// Applies the appropriate <see cref="FlowDirection"/> to the element based on the specified culture name.
    /// </summary>
    /// <typeparam name="T">The type of the visual element.</typeparam>
    /// <param name="element">The visual element to apply flow direction to.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    /// <returns>The element instance for method chaining.</returns>
    public static T ApplyFlowDirection<T>(this T element, string cultureName) where T : VisualElement
    {
        element.FlowDirection = CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

        return element;
    }

    /// <summary>
    /// Applies the appropriate <see cref="FlowDirection"/> to the window based on the specified culture name.
    /// <see cref="Microsoft.Maui.Controls.Window"/> defines its own FlowDirection property
    /// and does not inherit from <see cref="VisualElement"/>.
    /// </summary>
    /// <param name="window">The MAUI window to apply flow direction to.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    public static void ApplyFlowDirection(this Microsoft.Maui.Controls.Window window, string cultureName)
    {
        window.FlowDirection = CultureInfo.GetCultureInfo(cultureName).TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;
    }
}
