using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.UI.Services;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// MAUI navigation host: the equivalent of the content <c>Frame</c> in the WinUI and WPF shells.
/// The navigation service replaces this host's content instead of pushing pages onto the window.
/// </summary>
/// <remarks>
/// MAUI views are <see cref="ContentPage"/> instances and a page cannot be a child of a layout,
/// so the host detaches each page's <see cref="ContentPage.Content"/> and reparents it here. The
/// detached body is cached per page, so navigating back to an earlier frame restores the same
/// page instance (and its view model) exactly as the frame-based navigation service expects.
/// </remarks>
public class NavigationHostView : ContentView, INavigationHost
{
    private readonly ConditionalWeakTable<ContentPage, Microsoft.Maui.Controls.View> _bodies = new();

    /// <inheritdoc />
    public IView? CurrentPage { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationHostView"/> class.
    /// </summary>
    public NavigationHostView()
    {
        OnParentSet();
        HandlerChanged += (_, _) => RegisterHost();
        HandlerChanging += (_, e) =>
        {
            if (e.NewHandler is null)
                UnregisterHost();
        };
    }

    /// <inheritdoc />
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent is null)
            UnregisterHost();
        else
            RegisterHost();
    }

    /// <inheritdoc />
    public void SetContent(IView page)
    {
        ArgumentNullException.ThrowIfNull(page);

        if (page is not ContentPage contentPage)
            throw new InvalidOperationException(
                $"A navigation host can only show a ContentPage; '{page.GetType().Name}' is not one.");

        var body = contentPage.Content;

        if (body is not null)
        {
            // First time this page is shown: take its content so it can live inside the shell grid.
            contentPage.Content = null;
            _bodies.AddOrUpdate(contentPage, body);
        }
        else if (!_bodies.TryGetValue(contentPage, out body))
        {
            throw new InvalidOperationException(
                $"Page '{contentPage.GetType().Name}' has no content to show.");
        }

        BindingContext = page.ViewModel;
        Content = body;
        CurrentPage = page;
    }

    private static INavigationHostRegistry? ResolveRegistry() =>
        IPlatformApplication.Current?.Services?.GetService<INavigationHostRegistry>();

    private void RegisterHost()
    {
        try
        {
            ResolveRegistry()?.Register(this);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"NavigationHostView failed to register itself: {ex}");
        }
    }

    private void UnregisterHost()
    {
        try
        {
            ResolveRegistry()?.Unregister(this);
        }
        catch (Exception ex)
        {
            Trace.TraceError($"NavigationHostView failed to unregister itself: {ex}");
        }
    }
}
