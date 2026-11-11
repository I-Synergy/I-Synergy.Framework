namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Windows-specific half of <see cref="PageExtensions"/>: mirrors the culture onto the native title
/// bar once the page is attached to a window.
/// </summary>
public static partial class PageExtensions
{
    static void ApplyWindowsFlowDirection<T>(T page, string cultureName) where T : Page
    {
        // In a constructor GetParentWindow() returns null because the page is not in the visual tree
        // yet, so the native work is deferred to Loaded (or applied now when it is already loaded).
        void OnLoaded(object? sender, EventArgs e)
        {
            page.Loaded -= OnLoaded;
            page.GetAppWindow()?.ApplyFlowDirectionToTitleBar(cultureName);
        }

        if (page.IsLoaded)
            page.GetAppWindow()?.ApplyFlowDirectionToTitleBar(cultureName);
        else
            page.Loaded += OnLoaded;
    }
}
