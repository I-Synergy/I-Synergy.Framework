#if MACOS
using AppKit;
using Foundation;
using ISynergy.Framework.UI.Enumerations;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// AppKit cursor support for the native macOS target.
/// </summary>
/// <remarks>
/// The Mac Catalyst implementation drives cursors from a <c>UIHoverGestureRecognizer</c>, which
/// does not exist on AppKit. The AppKit equivalent is an <see cref="NSTrackingArea"/> whose owner
/// receives <c>mouseEntered:</c>/<c>mouseExited:</c> and sets the cursor directly.
/// </remarks>
public static class CursorExtensions
{
    private static readonly ConditionalWeakTable<NSView, TrackingAreaRegistration> Registrations = new();

    public static void SetCustomCursor(this VisualElement visualElement, CursorIcons cursor, IMauiContext? mauiContext)
    {
        ArgumentNullException.ThrowIfNull(mauiContext);

        if (visualElement.Handler?.PlatformView is not NSView view)
            return;

        RemoveTrackingArea(view);

        var tracker = new CursorTracker(GetNSCursor(cursor));
        var trackingArea = new NSTrackingArea(
            view.Bounds,
            NSTrackingAreaOptions.MouseEnteredAndExited
                | NSTrackingAreaOptions.ActiveInKeyWindow
                | NSTrackingAreaOptions.InVisibleRect,
            tracker,
            null);

        view.AddTrackingArea(trackingArea);
        Registrations.AddOrUpdate(view, new TrackingAreaRegistration(trackingArea, tracker));
    }

    public static void RemoveCustomCursor(this VisualElement visualElement, IMauiContext? mauiContext)
    {
        if (mauiContext is null) return;

        try
        {
            if (visualElement.Handler?.PlatformView is NSView view)
                RemoveTrackingArea(view);
        }
        catch { /* best-effort cleanup */ }
    }

    private static void RemoveTrackingArea(NSView view)
    {
        if (!Registrations.TryGetValue(view, out var registration))
            return;

        view.RemoveTrackingArea(registration.TrackingArea);
        Registrations.Remove(view);
    }

    private static NSCursor GetNSCursor(CursorIcons cursor)
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

    /// <summary>
    /// Owner of the tracking area. AppKit sends the responder messages to the object passed as the
    /// owner, so it has to be an <see cref="NSObject"/> exposing <c>mouseEntered:</c>/<c>mouseExited:</c>.
    /// </summary>
    [Register(nameof(CursorTracker))]
    private sealed class CursorTracker : NSObject
    {
        private readonly NSCursor _cursor;

        public CursorTracker(NSCursor cursor) => _cursor = cursor;

        [Export("mouseEntered:")]
        public void MouseEntered(NSObject? trackingArea) => _cursor.Set();

        [Export("mouseExited:")]
        public void MouseExited(NSObject? trackingArea) => NSCursor.ArrowCursor.Set();
    }

    private sealed record TrackingAreaRegistration(NSTrackingArea TrackingArea, CursorTracker Tracker);
}
#endif
