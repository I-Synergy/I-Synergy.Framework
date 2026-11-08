using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
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
public partial class ShellView : IShellView
{
    /// <summary>
    /// Width of the sidebar column, and of the phone drawer when it slides in.
    /// </summary>
    private const double DrawerWidth = 300d;

    private readonly ShellViewModel _viewModel;
    private readonly bool _isPhone;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellView"/> class.
    /// </summary>
    /// <param name="viewModel">The shell view model.</param>
    public ShellView(ShellViewModel viewModel)
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

        // The framework decides which heads render a window menu bar (Mac Catalyst today); the
        // sample only supplies the entries.
        this
            .AddShellMenuBar(
                "File",
                new MenuFlyoutItem { Text = "Settings", Command = viewModel.SettingsCommand },
                new MenuFlyoutSeparator(),
                new MenuFlyoutItem { Text = "Sign Out", Command = viewModel.SignInCommand })
            .AddShellMenuBar(
                "View",
                new MenuFlyoutItem { Text = "Theme", Command = viewModel.ThemeCommand })
            .AddShellMenuBar(
                "Help",
                new MenuFlyoutItem { Text = "Help", Command = viewModel.HelpCommand },
                new MenuFlyoutItem { Text = "Feedback", Command = viewModel.FeedbackCommand });
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

        // The breadcrumb row is only a content-column row on desktop; on phones the header spans
        // the window and the sidebar drawer starts below it.
        Grid.SetColumn(Header, 0);
        Grid.SetColumnSpan(Header, 2);
        Grid.SetRow(Sidebar, 1);
        Grid.SetRowSpan(Sidebar, 1);

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

        await _viewModel.ShellLoadedAsync();

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
