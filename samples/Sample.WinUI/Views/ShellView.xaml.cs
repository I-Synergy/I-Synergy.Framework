using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sample.ViewModels;


namespace Sample.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellView : View, IShellView
{
    private readonly ICommonServices _commonServices;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private bool _isLoaded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellView"/> class.
    /// </summary>
    public ShellView(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService)
    {
        InitializeComponent();

        _commonServices = commonServices;
        _dialogService = dialogService;
        _navigationService = navigationService;

        Loaded += OnViewLoaded;
        Unloaded += OnViewUnloaded;
        ActualThemeChanged += (_, _) => ApplyTitleBarColors();
    }

    private async void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            await Task.Delay(100);

            ApplyCustomTitleBar();

            // Blades are rendered by this shell pane, so they cover the navigation pane the way the
            // settings blade does instead of being confined to the page that opened them.
            var bladeHost = Application.Services.GetRequiredService<BladeHostViewModel>();
            if (bladeHost.Blades is not null)
                BladeDetails.ItemsSource = bladeHost.Blades;

            if (DataContext is ShellViewModel viewModel)
                await viewModel.ShellLoadedAsync();
        }
    }

    private void ApplyCustomTitleBar()
    {
        if (WindowHelper.GetWindowForElement(this) is not { } window)
            return;

        window.ExtendsContentIntoTitleBar = true;

        // Tall caption area (48px) instead of the default 32px: the system then centres the
        // minimise/maximise/close buttons in the same 48px row the theme and settings buttons live in,
        // so all title bar buttons line up vertically.
        WindowHelper.GetAppWindow(window).TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        window.SetTitleBar(AppTitleBar);

        ApplyTitleBarColors();
    }

    private void ApplyTitleBarColors()
    {
        if (WindowHelper.GetWindowForElement(this) is not { } window)
            return;

        var dark = ActualTheme == ElementTheme.Dark;
        var titleBar = WindowHelper.GetAppWindow(window).TitleBar;

        titleBar.BackgroundColor = Colors.Transparent;
        titleBar.InactiveBackgroundColor = Colors.Transparent;
        titleBar.ForegroundColor = dark
            ? Windows.UI.Color.FromArgb(255, 0xEE, 0xF1, 0xEA)
            : Windows.UI.Color.FromArgb(255, 0x14, 0x21, 0x1E);
        titleBar.InactiveForegroundColor = dark
            ? Windows.UI.Color.FromArgb(255, 0x9C, 0xB0, 0xA8)
            : Windows.UI.Color.FromArgb(255, 0x55, 0x66, 0x5F);

        titleBar.ButtonBackgroundColor = Colors.Transparent;
        titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        titleBar.ButtonForegroundColor = titleBar.ForegroundColor;
        titleBar.ButtonInactiveForegroundColor = titleBar.InactiveForegroundColor;
        titleBar.ButtonHoverBackgroundColor = dark
            ? Windows.UI.Color.FromArgb(255, 0x1C, 0x2A, 0x25)
            : Windows.UI.Color.FromArgb(255, 0xE8, 0xED, 0xE9);
        titleBar.ButtonHoverForegroundColor = titleBar.ForegroundColor;
        titleBar.ButtonPressedBackgroundColor = dark
            ? Windows.UI.Color.FromArgb(255, 0x26, 0x35, 0x2F)
            : Windows.UI.Color.FromArgb(255, 0xE1, 0xEE, 0xEA);
        titleBar.ButtonPressedForegroundColor = titleBar.ForegroundColor;
    }

    private void OnViewUnloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnViewLoaded;
        Unloaded -= OnViewUnloaded;

        _isLoaded = false;
    }

    private async void RootNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        await _navigationService.GoBackAsync();
}
