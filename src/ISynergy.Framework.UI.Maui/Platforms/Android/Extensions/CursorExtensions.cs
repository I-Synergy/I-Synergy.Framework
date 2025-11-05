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
            view.PointerIcon = PointerIcon.GetSystemIcon(Android.App.Application.Context, GetCursor(cursor));

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

}
