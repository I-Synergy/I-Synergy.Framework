using ISynergy.Framework.Globalization.Constants;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for <see cref="Page"/> to apply culture-aware window configuration.
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Applies flow direction to both the MAUI page and (on Windows) the native title bar.
    /// The MAUI-level flow direction is applied immediately to the page and any
    /// <see cref="FlyoutPage"/> children (Flyout and Detail) which may not inherit
    /// the parent direction through MAUI's property system. The native title bar
    /// configuration is deferred until the page is loaded and attached to a window,
    /// which means it is safe to call this from a page constructor.
    /// </summary>
    /// <typeparam name="T">The page type.</typeparam>
    /// <param name="page">The MAUI page.</param>
    /// <param name="cultureName">The two-letter culture name (e.g., "ar", "en").</param>
    /// <returns>The page instance for method chaining.</returns>
    public static T ApplyWindowFlowDirection<T>(this T page, string cultureName) where T : Page
    {
        // Apply MAUI-level flow direction to the page itself
        page.ApplyFlowDirection(cultureName);

        // FlyoutPage children (Flyout and Detail NavigationPage) don't always
        // inherit FlowDirection from the parent, so apply explicitly.
        if (page is FlyoutPage flyout)
        {
            if (flyout.Flyout is Page flyoutPage)
                flyoutPage.ApplyFlowDirection(cultureName);
            if (flyout.Detail is Page detailPage)
                detailPage.ApplyFlowDirection(cultureName);
        }

        // Also propagate to any child pages in a NavigationPage stack
        if (page is NavigationPage navPage && navPage.CurrentPage is Page currentPage)
            currentPage.ApplyFlowDirection(cultureName);

#if WINDOWS
        // Defer native title bar configuration until the page is attached to a window.
        // In constructors GetParentWindow() returns null because the page isn't
        // in the visual tree yet. We use the Loaded event as a one-shot trigger.
        void OnLoaded(object? sender, EventArgs e)
        {
            page.Loaded -= OnLoaded;

            var appWindow = page.GetAppWindow();
            appWindow?.ApplyFlowDirectionToTitleBar(cultureName);
        }

        // If the page is already loaded (e.g., re-applying culture at runtime),
        // apply immediately. Otherwise defer via the Loaded event.
        if (page.IsLoaded)
        {
            var appWindow = page.GetAppWindow();
            appWindow?.ApplyFlowDirectionToTitleBar(cultureName);
        }
        else
        {
            page.Loaded += OnLoaded;
        }
#endif

        return page;
    }
}
