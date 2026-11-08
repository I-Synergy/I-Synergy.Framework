namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Adds the desktop menu bar to a shell page.
/// </summary>
/// <remarks>
/// Menu bars are a desktop affordance; MAUI renders <see cref="Page.MenuBarItems"/> on Mac Catalyst
/// only, so this helper decides that in one place instead of leaving <c>#if</c> blocks in applications.
/// </remarks>
public static class PageMenuExtensions
{
    /// <summary>
    /// Gets a value indicating whether the current head renders a window menu bar.
    /// </summary>
    public static bool SupportsMenuBar => DeviceInfo.Platform == DevicePlatform.MacCatalyst;

    /// <summary>
    /// Adds a menu with its entries to the page when the head supports menu bars; otherwise it is a
    /// no-op, so callers stay platform-neutral.
    /// </summary>
    /// <param name="page">The shell page.</param>
    /// <param name="title">Menu title, e.g. "File".</param>
    /// <param name="items">Entries to add (<see cref="MenuFlyoutItem"/>, <see cref="MenuFlyoutSeparator"/> or a sub-menu).</param>
    /// <returns>The page instance for method chaining.</returns>
    public static Page AddShellMenuBar(this Page page, string title, params BindableObject[] items)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        if (!SupportsMenuBar || items.Length == 0)
            return page;

        var menu = new MenuBarItem { Text = title };

        foreach (var item in items)
        {
            switch (item)
            {
                // MenuFlyoutSeparator derives from MenuFlyoutItem, so one case covers both.
                case MenuFlyoutItem menuItem:
                    menu.Add(menuItem);
                    break;
            }
        }

        page.MenuBarItems.Add(menu);
        return page;
    }
}
