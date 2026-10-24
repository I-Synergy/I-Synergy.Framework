using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Services;
using System.Globalization;

namespace Sample.ViewModels;

public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly SettingsDrawerState _settingsDrawerState;
    private readonly IThemeService _themeService;

    /// <summary>
    /// Gets the command that flips between the light and dark theme (header button).
    /// </summary>
    public RelayCommand ToggleThemeCommand { get; }

    /// <summary>
    /// Gets the command to navigate to a specific route.
    /// </summary>
    public RelayCommand<string> NavigateToCommand { get; }

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
    /// <param name="settingsDrawerState">The shared open/closed state of the settings drawer.</param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        IThemeService themeService,
        SettingsDrawerState settingsDrawerState,
        ILogger<ShellViewModel> logger)
        : base(commonServices, dialogService, navigationService, settingsService, logger)
    {
        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

        _themeService = themeService;
        _settingsDrawerState = settingsDrawerState;

        ToggleThemeCommand = new RelayCommand(ToggleTheme);
        NavigateToCommand = new RelayCommand<string>(NavigateTo);

        // Initialize breadcrumb, route, and footer text
        NavigateTo("dashboard");
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
        await InitializeFirstRunAsync();
    }

    private void ToggleTheme()
    {
        var next = _themeService.Theme == Themes.Light ? Themes.Dark : Themes.Light;
        _themeService.SetTheme(next);
        UpdateThemeIcon(next);
    }

    private void UpdateThemeIcon(Themes theme)
    {
        // Moon glyph when light theme, sun glyph when dark theme
        ThemeIcon = theme == Themes.Light ? "" : "";
    }

    private void NavigateTo(string? route)
    {
        CurrentRoute = route;
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
            // First run: offer language, theme and accent in the settings drawer (applied live,
            // no restart), then remember that the introduction has been shown.
            _settingsDrawerState.Open();

            globalSettings.IsFirstRun = false;
            await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSettings);
        }
    }

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync()
    {
        _settingsDrawerState.Open();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Opens the help page as a modal.
    /// </summary>
    protected override Task OpenHelpAsync() =>
        _navigationService.NavigateModalAsync<HelpViewModel>();

    /// <summary>
    /// Opens the settings drawer to select language, theme, and accent color live.
    /// </summary>
    protected override async Task OpenLanguageAsync()
    {
        _settingsDrawerState.Open();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Opens the settings drawer to select language, theme, and accent color live.
    /// </summary>
    protected override async Task OpenThemesAsync()
    {
        _settingsDrawerState.Open();
        await Task.CompletedTask;
    }
}
