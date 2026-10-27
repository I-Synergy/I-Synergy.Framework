using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels;

public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly IBladeService _bladeService;

    /// <summary>
    /// Set while a navigation started by this shell is running, so the forward
    /// <c>BackStackChanged</c> (raised before the new frame is active) does not reset the route.
    /// </summary>
    private bool _isNavigating;
    private readonly BladeHostViewModel _bladeHost;

    /// <summary>
    /// Gets the command to navigate to a specific route.
    /// </summary>
    public RelayCommand<string> NavigateToCommand { get; }

    /// <summary>
    /// Gets the command that opens or closes the phone sidebar drawer.
    /// </summary>
    public RelayCommand ToggleMenuCommand { get; }

    /// <summary>
    /// Gets the command that navigates one step back through the navigation service's stack.
    /// </summary>
    public AsyncRelayCommand BackCommand { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the phone sidebar drawer is open.
    /// </summary>
    public bool IsMenuOpen
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the glyph shown on the theme toggle (moon when light, sun when dark).
    /// </summary>
    public string ThemeIcon
    {
        get => GetValue<string>() ?? "";
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the current breadcrumb text (page name).
    /// </summary>
    public string CrumbCurrent
    {
        get => GetValue<string>() ?? "";
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the current route for sidebar active-row detection.
    /// </summary>
    public string CurrentRoute
    {
        get => GetValue<string>() ?? "";
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the version text shown in the sidebar footer.
    /// </summary>
    public string VersionText
    {
        get => GetValue<string>() ?? "";
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the copyright text shown in the sidebar footer.
    /// </summary>
    public string CopyrightText
    {
        get => GetValue<string>() ?? "";
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Version property value.
    /// </summary>
    /// <value>The version.</value>
    public Version Version
    {
        get { return GetValue<Version>(); }
        set { SetValue(value); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="settingsService"></param>
    /// <param name="themeService">The theme service for managing theme and accent color.</param>
    /// <param name="bladeService">The service that opens blades into the shell blade pane.</param>
    /// <param name="bladeHost">The shell-level owner of the blade pane.</param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        IThemeService themeService,
        IBladeService bladeService,
        BladeHostViewModel bladeHost,
        ILogger<ShellViewModel> logger)
        : base(commonServices, dialogService, navigationService, settingsService, themeService, logger)
    {
        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

        _bladeService = bladeService;
        _bladeHost = bladeHost;
        _themeService.ThemeChanged += OnThemeChanged;

        // Keep the breadcrumb and the sidebar selection in step when the user goes back.
        _navigationService.BackStackChanged += OnNavigationBackStackChanged;

        NavigateToCommand = new RelayCommand<string>(NavigateTo);
        ToggleMenuCommand = new RelayCommand(() => IsMenuOpen = !IsMenuOpen);
        BackCommand = new AsyncRelayCommand(() => _navigationService.GoBackAsync());

        // Initialize breadcrumb, route, and footer text. The first navigation happens on shell load,
        // when the navigation host exists; a navigation attempt here would find no host yet.
        SelectRoute("dashboard");
        VersionText = $"v{commonServices.InfoService.ProductVersion}";
        CopyrightText = $"© {DateTime.Now.Year} I-Synergy. All rights reserved.";

        // Show the icon that matches the saved theme before the shell has loaded.
        // Moon glyph when light theme, sun glyph when dark theme
        ThemeIcon = _themeService.Theme == Themes.Light ? "" : "";
    }

    public override async Task ShellLoadedAsync()
    {
        // Apply saved theme on shell loaded
        _themeService.ApplyTheme();

        // Show the initial section now that the shell's navigation host is attached.
        NavigateTo(CurrentRoute);

        await InitializeFirstRunAsync();
    }


    private void NavigateTo(string? route)
    {
        // On phones the sidebar is a drawer: selecting a section closes it. On desktop it is a
        // persistent column, so the menu state is left alone.
        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            IsMenuOpen = false;

        SelectRoute(route);

        if (!string.IsNullOrWhiteSpace(route))
            _ = NavigateAsync(route);
    }

    private void SelectRoute(string? route)
    {
        CurrentRoute = route ?? "dashboard";

        // Update breadcrumb based on the route
        CrumbCurrent = route switch
        {
            "dashboard" => LocalizedString("Dashboard"),
            "customcontrols" => LocalizedString("CustomControls"),
            "mauicontrols" => LocalizedString("MauiControls"),
            "slideshow" => LocalizedString("SlideShow"),
            _ => Title
        };
    }

    /// <summary>
    /// Mirrors the navigation stack into the breadcrumb and the sidebar selection. Going back to a
    /// section re-shows it without running a navigate command, so the shell has to read the current
    /// view model type from the navigation service.
    /// </summary>
    /// <remarks>
    /// Forward navigation also raises this event, but before the new frame is active, so the event
    /// would report the section being left. Those changes are ignored while a navigation started
    /// here is in flight; <see cref="NavigateTo"/> has already set the target route.
    /// </remarks>
    private void OnNavigationBackStackChanged(object? sender, EventArgs e)
    {
        if (_isNavigating)
            return;

        var route = _navigationService.CurrentViewModelType switch
        {
            var type when type == typeof(InfoViewModel) => "dashboard",
            var type when type == typeof(CustomControlsViewModel) => "customcontrols",
            var type when type == typeof(MauiControlsViewModel) => "mauicontrols",
            var type when type == typeof(SlideShowViewModel) => "slideshow",
            _ => null
        };

        if (route is not null)
            SelectRoute(route);
    }

    /// <summary>
    /// Navigates the shell's content host to the section of the route, the same way the WinUI
    /// sample navigates its content frame.
    /// </summary>
    private async Task NavigateAsync(string route)
    {
        _isNavigating = true;

        try
        {
            await (route switch
            {
                "dashboard" => _navigationService.NavigateAsync<InfoViewModel>(),
                "customcontrols" => _navigationService.NavigateAsync<CustomControlsViewModel>(),
                "mauicontrols" => _navigationService.NavigateAsync<MauiControlsViewModel>(),
                "slideshow" => _navigationService.NavigateAsync<SlideShowViewModel>(),
                _ => Task.CompletedTask
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"Navigation to '{route}' failed: {ex}");
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private string LocalizedString(string resourceKey)
    {
        try
        {
            // Get localized string from LocalizedStrings; fallback to key if not found
            return LocalizedStrings.Default[resourceKey] ?? resourceKey;
        }
        catch
        {
            return resourceKey;
        }
    }

    protected override Task SignOutAsync() =>
        IPlatformApplication.Current!.Services.GetRequiredService<Sample.Abstractions.Services.IAuthenticationService>().SignOutAsync();

    public override async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings is { IsFirstRun: true } globalSettings)
        {
            // First run: offer language, theme and accent in the settings blade (applied live,
            // no restart), then remember that the introduction has been shown.
            await OpenSettingsAsync();

            globalSettings.IsFirstRun = false;
            await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSettings);
        }
    }

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        _bladeService.OpenBladeAsync<SettingsViewModel, object>(_bladeHost);

    /// <summary>
    /// Opens the help page as a modal.
    /// </summary>
    protected override Task OpenHelpAsync() =>
        _navigationService.NavigateModalAsync<HelpViewModel>();

    private void OnThemeChanged(object? sender, EventArgs e) =>
        ThemeIcon = _themeService.Theme == Themes.Light ? "" : "";

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _themeService.ThemeChanged -= OnThemeChanged;
            _navigationService.BackStackChanged -= OnNavigationBackStackChanged;
            NavigateToCommand?.Dispose();
            ToggleMenuCommand?.Dispose();
            BackCommand?.Dispose();
        }

        base.Dispose(disposing);
    }
}
