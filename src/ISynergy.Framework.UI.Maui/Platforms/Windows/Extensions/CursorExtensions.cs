using ISynergy.Framework.UI.Enumerations;
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.UI.Core;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;
public static class CursorExtensions
{
    private static readonly ConditionalWeakTable<UIElement, CursorHandlers> _cursorStates = new();

    private sealed class CursorHandlers
    {
        public PointerEventHandler? EnteredHandler;
        public PointerEventHandler? ExitedHandler;
    }

    public static void SetCustomCursor(this VisualElement visualElement, CursorIcons cursor, IMauiContext? mauiContext)
    {
        if (mauiContext is null)
        {
            System.Diagnostics.Debug.WriteLine("MauiContext is null, cannot set custom cursor");
            return;
        }

        if (visualElement.Handler is not null)
        {
            // Handler is ready, setup cursor immediately
            SetupCursor(visualElement, cursor, mauiContext);
            return;
        }

        // Handler is not ready yet, defer until HandlerChanged event
        void HandleHandlerChanged(object? sender, EventArgs e)
        {
            visualElement.HandlerChanged -= HandleHandlerChanged;

            // Only setup if handler is now available
            if (visualElement.Handler is not null)
            {
                SetupCursor(visualElement, cursor, mauiContext);
            }
        }

        visualElement.HandlerChanged += HandleHandlerChanged;
    }

    /// <summary>
    /// Resolves the platform view and wires the pointer entered/exited handlers that drive the cursor.
    /// </summary>
    private static void SetupCursor(VisualElement visualElement, CursorIcons cursor, IMauiContext? mauiContext)
    {
        try
        {
            // Use the element's own handler's MauiContext if available (most reliable)
            var contextToUse = visualElement.Handler?.MauiContext ?? mauiContext;

            if (contextToUse is null)
            {
                System.Diagnostics.Debug.WriteLine("No valid MauiContext available for cursor setup");
                return;
            }

            var view = visualElement.ToPlatform(contextToUse);

            if (view is null)
            {
                System.Diagnostics.Debug.WriteLine("Failed to get platform view for cursor setup");
                return;
            }

            // Remove existing event handlers using stored exact delegates to prevent accumulation
            if (_cursorStates.TryGetValue(view, out var existingHandlers))
            {
                view.PointerEntered -= existingHandlers.EnteredHandler;
                view.PointerExited -= existingHandlers.ExitedHandler;
                _cursorStates.Remove(view);
            }

            void ViewOnPointerExited(object? sender, PointerRoutedEventArgs e)
            {
                try
                {
                    SetProtectedCursor(view, InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 1)));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to change cursor on pointer exit: {ex.Message}");
                }
            }

            void ViewOnPointerEntered(object? sender, PointerRoutedEventArgs e)
            {
                try
                {
                    // Only apply custom cursor if the element is enabled
                    var cursorType = visualElement.IsEnabled ? GetCursor(cursor) : CoreCursorType.Arrow;
                    SetProtectedCursor(view, InputCursor.CreateFromCoreCursor(new CoreCursor(cursorType, 1)));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to change cursor on pointer enter: {ex.Message}");
                }
            }

            // Store exact delegate instances for later cleanup
            var handlers = new CursorHandlers
            {
                EnteredHandler = ViewOnPointerEntered,
                ExitedHandler = ViewOnPointerExited
            };
            _cursorStates.Add(view, handlers);

            // Attach event handlers using stored instances (guarantees correct -= on next call)
            view.PointerEntered += handlers.EnteredHandler;
            view.PointerExited += handlers.ExitedHandler;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to set custom cursor: {ex.Message}");
        }
    }

    public static void RemoveCustomCursor(this VisualElement visualElement, IMauiContext? mauiContext)
    {
        if (mauiContext is null) return;
        try
        {
            var view = visualElement.ToPlatform(mauiContext);
            if (view is not null && _cursorStates.TryGetValue(view, out var handlers))
            {
                view.PointerEntered -= handlers.EnteredHandler;
                view.PointerExited -= handlers.ExitedHandler;
                _cursorStates.Remove(view);
            }
        }
        catch { /* best-effort cleanup */ }
    }

    // UIElement.ProtectedCursor is protected; UnsafeAccessor replaces the former reflection
    // bypass (S3011) with a compile-time-checked accessor that has no runtime reflection cost.
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_ProtectedCursor")]
    static extern void SetProtectedCursor(UIElement uiElement, InputCursor cursor);

    static CoreCursorType GetCursor(CursorIcons cursor)
    {
        return cursor switch
        {
            CursorIcons.Hand => CoreCursorType.Hand,
            CursorIcons.IBeam => CoreCursorType.IBeam,
            CursorIcons.Cross => CoreCursorType.Cross,
            CursorIcons.Arrow => CoreCursorType.Arrow,
            CursorIcons.SizeAll => CoreCursorType.SizeAll,
            CursorIcons.Wait => CoreCursorType.Wait,
            _ => CoreCursorType.Arrow,
        };
    }
}
