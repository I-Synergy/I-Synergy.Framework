using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Views;
using Sample.ViewModels;

namespace Sample.Views;

/// <summary>
/// Root page of the sample shell: header, sidebar, navigation host and the blade pane overlay.
/// </summary>
/// <remarks>
/// A MAUI <see cref="Shell"/> owns the page tree and only accepts ShellContent/FlyoutContent/TitleView,
/// so there is nowhere above the pages to host the blade pane. This root page keeps the header, the
/// sidebar, the navigation host and the blade pane alive together in one grid instead.
/// <para>
/// Navigation follows the WinUI sample: the shell exposes a content host (the MAUI equivalent of a
/// WinUI <c>Frame</c>) and the framework navigation service replaces its content while owning the
/// backstack. The host registers itself, so this page does not navigate.
/// </para>
/// </remarks>
[Lifetime(Lifetimes.Singleton)]
public partial class RootView : IShellView
{
    /// <summary>
    /// Width of the sidebar column, and of the phone drawer when it slides in.
    /// </summary>
    private const double DrawerWidth = 300d;

    private readonly ShellViewModel _viewModel;
    private readonly bool _isPhone;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootView"/> class.
    /// </summary>
    /// <param name="viewModel">The shell view model.</param>
    public RootView(ShellViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = viewModel;

        _isPhone = DeviceInfo.Idiom == DeviceIdiom.Phone;

        if (_isPhone)
            SetUpPhoneDrawer();

        // Desktop starts expanded; phones start with the drawer closed.
        _viewModel.IsMenuOpen = !_isPhone;
        ApplyMenuState();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        // The framework never raises BaseShellViewModel.ShellLoadedAsync for this root page, so the
        // saved theme, the initial route and the first-run flow are started here, once the visual
        // tree (and therefore the navigation host) is available.
        Loaded += OnRootLoaded;

#if MACCATALYST
        var fileMenu = new MenuBarItem { Text = "File" };
        fileMenu.Add(new MenuFlyoutItem { Text = "Settings", Command = viewModel.SettingsCommand });
        fileMenu.Add(new MenuFlyoutSeparator());
        fileMenu.Add(new MenuFlyoutItem { Text = "Sign Out", Command = viewModel.SignInCommand });
        MenuBarItems.Add(fileMenu);

        var viewMenu = new MenuBarItem { Text = "View" };
        viewMenu.Add(new MenuFlyoutItem { Text = "Theme", Command = viewModel.ThemeCommand });
        MenuBarItems.Add(viewMenu);

        var helpMenu = new MenuBarItem { Text = "Help" };
        helpMenu.Add(new MenuFlyoutItem { Text = "Help", Command = viewModel.HelpCommand });
        helpMenu.Add(new MenuFlyoutItem { Text = "Feedback", Command = viewModel.FeedbackCommand });
        MenuBarItems.Add(helpMenu);
#endif
    }

    /// <summary>
    /// On phones the sidebar is a drawer above the content instead of a column beside it: the
    /// content spans both columns and the sidebar stays translated off-screen until the header's
    /// menu button opens it.
    /// </summary>
    private void SetUpPhoneDrawer()
    {
        Grid.SetColumnSpan(ContentHost, 2);
        Grid.SetColumnSpan(DrawerScrim, 2);

        Sidebar.TranslationX = -DrawerWidth;
        Sidebar.WidthRequest = DrawerWidth;
    }

    /// <summary>
    /// Applies the menu button's state: on phones it opens the drawer and its scrim, on desktop it
    /// collapses or restores the sidebar column so the content can use the extra width.
    /// </summary>
    private void ApplyMenuState()
    {
        if (_isPhone)
        {
            DrawerScrim.IsVisible = _viewModel.IsMenuOpen;
            Sidebar.TranslationX = _viewModel.IsMenuOpen ? 0d : -DrawerWidth;
            return;
        }

        Sidebar.IsVisible = _viewModel.IsMenuOpen;
        SidebarColumn.Width = _viewModel.IsMenuOpen ? new GridLength(DrawerWidth) : new GridLength(0d);
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShellViewModel.IsMenuOpen))
            ApplyMenuState();
    }


    private async void OnRootLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnRootLoaded;

        MergeHeaderIntoTitleBar();

        await _viewModel.ShellLoadedAsync();

#if WINDOWS
        // MAUI applies its own title-bar chrome during the first layout pass, so the shell colours
        // are applied afterwards. Failures here are cosmetic and must never break startup.
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(400), ApplyWindowChrome);
#endif
    }

#if WINDOWS
    /// <summary>
    /// Applies the shell colours to the native window chrome (caption background, hover and glyph
    /// colours) so the title bar matches the page background instead of the system surface.
    /// </summary>
    private void ApplyWindowChrome()
    {
        try
        {
            if (Window?.Handler?.PlatformView is not Microsoft.UI.Xaml.Window nativeWindow)
                return;

            var dark = Application.Current?.RequestedTheme == AppTheme.Dark;
            var titleBar = nativeWindow.AppWindow.TitleBar;

            var background = ToNativeColor(ResolveShellColor(
                dark ? "AppBgDark" : "AppBgLight",
                dark ? Color.FromArgb("#0D1614") : Color.FromArgb("#F4F6F3")));
            var hover = ToNativeColor(ResolveShellColor(
                dark ? "AppSurface2Dark" : "AppSurface2Light",
                dark ? Color.FromArgb("#1C2A25") : Color.FromArgb("#E8EDE9")));
            var foreground = dark
                ? Windows.UI.Color.FromArgb(255, 156, 176, 168)
                : Windows.UI.Color.FromArgb(255, 85, 102, 95);
            var hoverForeground = dark
                ? Windows.UI.Color.FromArgb(255, 238, 241, 234)
                : Windows.UI.Color.FromArgb(255, 20, 33, 30);

            titleBar.BackgroundColor = background;
            titleBar.InactiveBackgroundColor = background;
            titleBar.ButtonBackgroundColor = background;
            titleBar.ButtonInactiveBackgroundColor = background;
            titleBar.ButtonHoverBackgroundColor = hover;
            titleBar.ButtonPressedBackgroundColor = hover;
            titleBar.ButtonForegroundColor = foreground;
            titleBar.ButtonInactiveForegroundColor = foreground;
            titleBar.ButtonHoverForegroundColor = hoverForeground;
            titleBar.ButtonPressedForegroundColor = hoverForeground;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"Could not apply the window chrome colours: {ex}");
        }
    }

    private static Color ResolveShellColor(string key, Color fallback) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color
            : fallback;

    private static Windows.UI.Color ToNativeColor(Color color) =>
        Windows.UI.Color.FromArgb(255, (byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255));
#endif

    /// <summary>
    /// The window title bar already hosts a shell header (created by <c>App.SetTitleBar</c>). It
    /// only needs the shell view model; the in-page header row is then collapsed so the sidebar
    /// and the content start directly below the title bar. On platforms without a title bar
    /// (phones and tablets) the in-page header stays.
    /// </summary>
    private void MergeHeaderIntoTitleBar()
    {
        if (Window?.TitleBar is not TitleBar { Content: Sample.Views.Controls.AppHeaderView titleBarHeader })
            return;

        titleBarHeader.BindingContext = _viewModel;

        Header.IsVisible = false;
        HeaderRow.Height = new GridLength(0d);
    }



    #region IDisposable
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Loaded -= OnRootLoaded;
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _viewModel.Dispose();
        }

        base.Dispose(disposing);
    }
    #endregion
}
