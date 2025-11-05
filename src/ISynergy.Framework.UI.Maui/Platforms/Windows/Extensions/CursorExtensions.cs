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
        ArgumentNullException.ThrowIfNull(mauiContext);

        // Wait for handler to be fully initialized before accessing the platform view
        void SetupCursor()
        {
            try
            {
                var view = visualElement.ToPlatform(mauiContext);
                view.PointerEntered += ViewOnPointerEntered;
                view.PointerExited += ViewOnPointerExited;

                void ViewOnPointerExited(object? sender, PointerRoutedEventArgs e)
                {
                    view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(GetCursor(CursorIcons.Arrow), 1)));
                }

                void ViewOnPointerEntered(object? sender, PointerRoutedEventArgs e)
                {
                    view.ChangeCursor(InputCursor.CreateFromCoreCursor(new CoreCursor(GetCursor(cursor), 1)));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set custom cursor: {ex.Message}");
            }
        }

        if (visualElement.Handler is not null && visualElement.Handler.MauiContext is not null)
        {
            // Handler is ready, setup cursor immediately
            SetupCursor();
        }
        else
        {
            // Handler is not ready yet, defer until HandlerChanged event
            visualElement.HandlerChanged += HandleHandlerChanged;

            void HandleHandlerChanged(object? sender, EventArgs e)
            {
                visualElement.HandlerChanged -= HandleHandlerChanged;
                SetupCursor();
            }
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
