using ISynergy.Framework.UI.Enumerations;
using Microsoft.Maui.Platform;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Reflection;
using Windows.UI.Core;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI.Extensions;
public static class CursorExtensions
{
    public static void SetCustomCursor(this VisualElement visualElement, CursorIcons cursor, IMauiContext? mauiContext)
    {
        if (mauiContext is null)
        {
            System.Diagnostics.Debug.WriteLine("MauiContext is null, cannot set custom cursor");
            return;
        }

        // Wait for handler to be fully initialized before accessing the platform view
        void SetupCursor()
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

                // Remove existing event handlers to prevent duplicates
                view.PointerEntered -= ViewOnPointerEntered;
                view.PointerExited -= ViewOnPointerExited;

                void ViewOnPointerExited(object? sender, PointerRoutedEventArgs e)
                {
                    try
                    {
                        view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(CoreCursorType.Arrow, 1)));
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
                        view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(cursorType, 1)));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to change cursor on pointer enter: {ex.Message}");
                    }
                }

                // Attach event handlers
                view.PointerEntered += ViewOnPointerEntered;
                view.PointerExited += ViewOnPointerExited;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set custom cursor: {ex.Message}");
            }
        }

        if (visualElement.Handler is not null)
        {
            // Handler is ready, setup cursor immediately
            SetupCursor();
        }
        else
        {
            // Handler is not ready yet, defer until HandlerChanged event
            void HandleHandlerChanged(object? sender, EventArgs e)
            {
                visualElement.HandlerChanged -= HandleHandlerChanged;
                
                // Only setup if handler is now available
                if (visualElement.Handler is not null)
                {
                    SetupCursor();
                }
            }
            
            visualElement.HandlerChanged += HandleHandlerChanged;
        }
    }

    static void ChangeCursor(this UIElement uiElement, InputCursor cursor)
    {
        Type type = typeof(UIElement);
        type.InvokeMember("ProtectedCursor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, uiElement, new object[] { cursor });
    }

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
