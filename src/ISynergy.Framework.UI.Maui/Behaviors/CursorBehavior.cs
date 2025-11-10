using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Maui.Controls;

namespace ISynergy.Framework.UI.Behaviors;
public class CursorBehavior
{
    public static readonly BindableProperty CursorProperty = BindableProperty.CreateAttached("Cursor", typeof(CursorIcons), typeof(CursorBehavior), CursorIcons.Arrow, propertyChanged: CursorChanged);

    private static void CursorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is VisualElement visualElement)
        {
            // Defer cursor setup until the element is loaded and has a handler
            // This ensures we have the correct MauiContext, especially for dialog windows
            void SetupCursorWhenReady()
            {
                // Try to get MauiContext from the element's handler first (most reliable)
                var mauiContext = visualElement.Handler?.MauiContext;
                
                // If not available, try to find it from the element's parent hierarchy
                if (mauiContext is null)
                {
                    mauiContext = FindMauiContextFromParent(visualElement);
                }
                
                // If still not available, try to get it from the parent page
                if (mauiContext is null)
                {
                    var parentPage = FindParentPage(visualElement);
                    if (parentPage is not null)
                    {
                        mauiContext = parentPage.Handler?.MauiContext;
                    }
                }
                
                // Fallback: try to get it from any available window
                if (mauiContext is null && Application.Current is not null)
                {
                    // Try all windows, starting with the last one (most likely to be the active dialog)
                    foreach (var window in Application.Current.Windows.Reverse())
                    {
                        if (window.Page?.Handler?.MauiContext is not null)
                        {
                            mauiContext = window.Page.Handler.MauiContext;
                            break;
                        }
                    }
                }
                
                // Only set cursor if we have a valid MauiContext
                if (mauiContext is not null)
                {
                    try
                    {
                        visualElement.SetCustomCursor((CursorIcons)newvalue, mauiContext);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't throw - cursor setup failure shouldn't break the app
                        System.Diagnostics.Debug.WriteLine($"Failed to set cursor: {ex.Message}");
                    }
                }
            }

            // If handler is ready, setup immediately
            if (visualElement.Handler is not null)
            {
                SetupCursorWhenReady();
            }
            else
            {
                // Wait for handler to be ready
                void OnHandlerChanged(object? sender, EventArgs e)
                {
                    visualElement.HandlerChanged -= OnHandlerChanged;
                    SetupCursorWhenReady();
                }
                visualElement.HandlerChanged += OnHandlerChanged;
            }
        }
    }

    private static IMauiContext? FindMauiContextFromParent(VisualElement element)
    {
        var parent = element.Parent;
        while (parent is not null)
        {
            if (parent is VisualElement visualParent && visualParent.Handler?.MauiContext is not null)
            {
                return visualParent.Handler.MauiContext;
            }
            parent = parent.Parent;
        }
        return null;
    }

    private static Page? FindParentPage(VisualElement element)
    {
        var parent = element.Parent;
        while (parent is not null)
        {
            if (parent is Page page)
            {
                return page;
            }
            parent = parent.Parent;
        }
        return null;
    }

    public static CursorIcons GetCursor(BindableObject view) => (CursorIcons)view.GetValue(CursorProperty);

    public static void SetCursor(BindableObject view, CursorIcons value) => view.SetValue(CursorProperty, value);
}