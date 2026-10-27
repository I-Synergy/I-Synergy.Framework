using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Behaviors;
public static class CursorBehavior
{
    public static readonly BindableProperty CursorProperty = BindableProperty.CreateAttached("Cursor", typeof(CursorIcons), typeof(CursorBehavior), CursorIcons.Arrow, propertyChanged: CursorChanged);

    private static void CursorChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is VisualElement visualElement)
        {
            // Setup cursor and IsEnabled monitoring
            SetupCursorBehavior(visualElement, (CursorIcons)newvalue);
        }
    }

    private static void SetupCursorBehavior(VisualElement visualElement, CursorIcons cursor)
    {
        // Unsubscribe all existing handlers first to prevent duplicates
        visualElement.PropertyChanged -= OnVisualElementPropertyChanged;
        visualElement.HandlerChanged -= OnHandlerChanged;
        visualElement.Unloaded -= OnVisualElementUnloaded;
        visualElement.Loaded -= OnVisualElementLoaded;

        // Listen to IsEnabled property changes
        visualElement.PropertyChanged += OnVisualElementPropertyChanged;

        // Listen to handler changes persistently (not one-shot) so the cursor is
        // re-applied when the handler is disconnected and reconnected. This happens
        // during modal navigation: CleanupPage sets Handler=null on non-Singleton pages,
        // then a new handler is created when the page is reused.
        visualElement.HandlerChanged += OnHandlerChanged;

        // Clean up platform cursor when element is removed from the visual tree
        visualElement.Unloaded += OnVisualElementUnloaded;

        // Re-apply cursor when element is re-added to the visual tree after
        // a prior Unloaded (e.g. ShellView is replaced as root page during
        // modal navigation, then re-set on return).
        visualElement.Loaded += OnVisualElementLoaded;

        // Apply cursor based on current state
        ApplyCursorBasedOnState(visualElement, cursor);
    }

    private static void OnVisualElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is VisualElement visualElement && e.PropertyName == nameof(VisualElement.IsEnabled))
        {
            var cursor = GetCursor(visualElement);
            ApplyCursorBasedOnState(visualElement, cursor);
        }
    }

    private static void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is VisualElement visualElement && visualElement.Handler is not null)
        {
            var cursor = GetCursor(visualElement);
            ApplyCursorBasedOnState(visualElement, cursor);
        }
    }

    private static void OnVisualElementLoaded(object? sender, EventArgs e)
    {
        if (sender is VisualElement visualElement)
        {
            var cursor = GetCursor(visualElement);
            // Only re-apply the cursor — do NOT call SetupCursorBehavior here.
            // SetupCursorBehavior re-subscribes to Loaded, and if MAUI fires
            // Loaded synchronously on subscription for already-loaded elements,
            // that causes infinite recursion (StackOverflow).
            ApplyCursorBasedOnState(visualElement, cursor);
        }
    }

    private static void OnVisualElementUnloaded(object? sender, EventArgs e)
    {
        if (sender is VisualElement visualElement)
        {
            // Clean up the platform cursor but keep MAUI event subscriptions
            // (PropertyChanged, HandlerChanged, Loaded, Unloaded) intact.
            // The element may be re-loaded when the page is re-attached to
            // the visual tree (e.g. Singleton ShellView reused after modal
            // navigation). The static handler delegates do not prevent GC
            // of the element — when the element becomes unreachable it is
            // collected and the subscriptions die with it.

            // Get MauiContext for platform cleanup
            var mauiContext = visualElement.Handler?.MauiContext
                ?? FindMauiContextFromParent(visualElement)
                ?? Application.Current?.Windows.LastOrDefault()?.Page?.Handler?.MauiContext;

            if (mauiContext is not null)
            {
                try
                {
                    visualElement.RemoveCustomCursor(mauiContext);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to remove custom cursor on unload: {ex.Message}");
                }
            }
        }
    }

    private static void ApplyCursorBasedOnState(VisualElement visualElement, CursorIcons cursor) // NOSONAR
    {
        // Use Arrow cursor if element is disabled, otherwise use the specified cursor
        var cursorToApply = visualElement.IsEnabled ? cursor : CursorIcons.Arrow;

        // If the handler isn't ready yet, OnHandlerChanged will re-apply when it arrives.
        if (visualElement.Handler is null)
            return;

        // Try to get MauiContext from the element's handler first (most reliable)
        var mauiContext = visualElement.Handler.MauiContext;

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
            mauiContext = Application.Current.Windows.Reverse()
                .Select(window => window.Page?.Handler?.MauiContext)
                .FirstOrDefault(ctx => ctx is not null);
        }

        // Only set cursor if we have a valid MauiContext
        if (mauiContext is not null)
        {
            try
            {
                visualElement.SetCustomCursor(cursorToApply, mauiContext);
            }
            catch (Exception ex)
            {
                // Log but don't throw - cursor setup failure shouldn't break the app
                System.Diagnostics.Debug.WriteLine($"Failed to set cursor: {ex.Message}");
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
