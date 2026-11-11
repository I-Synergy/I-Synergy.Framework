using Microsoft.Maui.Handlers;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.UI.Maui.Platforms.Windows.Extensions;

/// <summary>
/// Applies the shell chrome that WinUI needs but MAUI does not express: it removes the system focus
/// rectangle from the header icon buttons (it reads as a permanent "selected" state) and gives the
/// tappable shell controls the hand cursor.
/// </summary>
/// <remarks>
/// The controls opt in through <see cref="HeaderIconButtonStyleId"/>, <see cref="SidebarRowStyleId"/>
/// and <see cref="LinkLabelStyleId"/>; shells set those <c>StyleId</c> values in XAML, so a head-specific
/// handler registration does not have to live in the application.
/// </remarks>
public static class ShellChromeStyler
{
    /// <summary>Style identifier for the header icon buttons (menu, back, theme, settings).</summary>
    public const string HeaderIconButtonStyleId = "HeaderIconButton";

    /// <summary>Style identifier for a sidebar navigation row.</summary>
    public const string SidebarRowStyleId = "SidebarRow";

    /// <summary>Style identifier for a tappable breadcrumb label.</summary>
    public const string LinkLabelStyleId = "LinkLabel";

    private static bool _isRegistered;

    /// <summary>
    /// Registers the WinUI mappers. Safe to call more than once.
    /// </summary>
    public static void Register()
    {
        if (_isRegistered)
            return;

        _isRegistered = true;

        ButtonHandler.Mapper.AppendToMapping(
            $"{nameof(ShellChromeStyler)}FocusVisual",
            (handler, view) =>
            {
                if (view is Button { StyleId: HeaderIconButtonStyleId })
                    handler.PlatformView.UseSystemFocusVisuals = false;
            });

        // MAUI has no cursor API, so the cursor is set on the platform element.
        ButtonHandler.Mapper.AppendToMapping(
            $"{nameof(ShellChromeStyler)}Cursor",
            (handler, view) => ApplyHandCursor(handler.PlatformView));

        BorderHandler.Mapper.AppendToMapping(
            $"{nameof(ShellChromeStyler)}Cursor",
            (handler, view) =>
            {
                if (view is Border { StyleId: SidebarRowStyleId })
                    ApplyHandCursor(handler.PlatformView);
            });

        LabelHandler.Mapper.AppendToMapping(
            $"{nameof(ShellChromeStyler)}Cursor",
            (handler, view) =>
            {
                if (view is Label { StyleId: LinkLabelStyleId })
                    ApplyHandCursor(handler.PlatformView);
            });
    }

    // UIElement.ProtectedCursor is protected; UnsafeAccessor replaces the former reflection
    // bypass (S3011) with a compile-time-checked accessor that has no runtime reflection cost.
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_ProtectedCursor")]
    private static extern void SetProtectedCursor(Microsoft.UI.Xaml.UIElement uiElement, Microsoft.UI.Input.InputCursor cursor);

    private static void ApplyHandCursor(Microsoft.UI.Xaml.UIElement? element)
    {
        if (element is null)
            return;

        try
        {
            SetProtectedCursor(
                element,
                Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand));
        }
        catch (Exception)
        {
            // A missing cursor is cosmetic; never fail the handler mapping over it.
        }
    }
}
