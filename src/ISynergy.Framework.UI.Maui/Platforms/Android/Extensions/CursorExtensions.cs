using Android.Views;
using ISynergy.Framework.UI.Enumerations;
using Microsoft.Maui.Platform;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;

public static class CursorExtensions
{
    public static void SetCustomCursor(this VisualElement visualElement, CursorIcons cursor, IMauiContext? mauiContext)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            ArgumentNullException.ThrowIfNull(mauiContext);
            var view = visualElement.ToPlatform(mauiContext);

            // Only apply custom cursor if the element is enabled, otherwise use default
            var cursorToApply = visualElement.IsEnabled ? cursor : CursorIcons.Arrow;
            view.PointerIcon = PointerIcon.GetSystemIcon(Android.App.Application.Context, GetCursor(cursorToApply));

            static PointerIconType GetCursor(CursorIcons cursor)
            {
                return cursor switch
                {
                    CursorIcons.Hand => PointerIconType.Hand,
                    CursorIcons.IBeam => PointerIconType.AllScroll,
                    CursorIcons.Cross => PointerIconType.Crosshair,
                    CursorIcons.Arrow => PointerIconType.Arrow,
                    CursorIcons.SizeAll => PointerIconType.TopRightDiagonalDoubleArrow,
                    CursorIcons.Wait => PointerIconType.Wait,
                    _ => PointerIconType.Default,
                };
            }
        }
    }

    /// <summary>
    /// No-op on Android — <see cref="SetCustomCursor"/> only sets a property
    /// (<see cref="Android.Views.View.PointerIcon"/>) on the native view.
    /// No event handlers, interactions, or gesture recognizers to clean up.
    /// </summary>
    public static void RemoveCustomCursor(this VisualElement visualElement, IMauiContext? mauiContext)
    {
        // No-op: Android cursor is a simple property set, not a retained reference
    }

}
