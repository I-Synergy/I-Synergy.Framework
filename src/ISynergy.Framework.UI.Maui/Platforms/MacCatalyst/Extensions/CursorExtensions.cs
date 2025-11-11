using AppKit;
using ISynergy.Framework.UI.Enumerations;
using Microsoft.Maui.Platform;
using UIKit;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;
public static class CursorExtensions
{
    public static void SetCustomCursor(this VisualElement visualElement, CursorIcons cursor, IMauiContext? mauiContext)
    {
        ArgumentNullException.ThrowIfNull(mauiContext);
        var view = visualElement.ToPlatform(mauiContext);
        if (view.GestureRecognizers is not null)
        {
            foreach (var recognizer in view.GestureRecognizers.OfType<PointerUIHoverGestureRecognizer>())
            {
                view.RemoveGestureRecognizer(recognizer);
            }
        }

        view.AddGestureRecognizer(new PointerUIHoverGestureRecognizer(visualElement, cursor, r =>
        {
            switch (r.State)
            {
                case UIGestureRecognizerState.Began:
                    if (visualElement.IsEnabled)
                    {
                        GetNSCursor(cursor).Set();
                    }
                    break;
                case UIGestureRecognizerState.Ended:
                    NSCursor.ArrowCursor.Set();
                    break;
            }
        }));
    }

    static NSCursor GetNSCursor(CursorIcons cursor)
    {
        return cursor switch
        {
            CursorIcons.Hand => NSCursor.OpenHandCursor,
            CursorIcons.IBeam => NSCursor.IBeamCursor,
            CursorIcons.Cross => NSCursor.CrosshairCursor,
            CursorIcons.Arrow => NSCursor.ArrowCursor,
            CursorIcons.SizeAll => NSCursor.ResizeUpCursor,
            CursorIcons.Wait => NSCursor.OperationNotAllowedCursor,
            _ => NSCursor.ArrowCursor,
        };
    }

    class PointerUIHoverGestureRecognizer : UIHoverGestureRecognizer
    {
        private readonly VisualElement _visualElement;
        private readonly CursorIcons _cursor;

        public PointerUIHoverGestureRecognizer(VisualElement visualElement, CursorIcons cursor, Action<UIHoverGestureRecognizer> action)
            : base(action)
        {
            _visualElement = visualElement;
            _cursor = cursor;
        }
    }
}