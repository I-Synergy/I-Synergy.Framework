using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.Utilities;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;

namespace Sample.ViewModels;

public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
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
    /// Gets the common services.
    /// </summary>
    /// <value>The common services.</value>
    public ICommonServices CommonServices { get; }

    /// <summary>
    /// Gets the settings service.
    /// </summary>
    public IBaseSettingsService SettingsService { get; }

    /// <summary>
    /// Gets or sets the display command.
    /// </summary>
    /// <value>The display command.</value>
    public AsyncRelayCommand SlideshowCommand { get; private set; }

    /// <summary>
    /// Gets or sets the information command.
    /// </summary>
    /// <value>The information command.</value>
    public AsyncRelayCommand InfoCommand { get; private set; }

    /// <summary>
    /// Gets or sets the controls command.
    /// </summary>
    /// <value>The browse command.</value>
    public AsyncRelayCommand ControlsCommand { get; private set; }

    public AsyncRelayCommand SyncCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="settingsService">The settings services.</param>
    /// <param name="authenticationService"></param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="themeService">The theme selector service.</param>
    /// <param name="localizationService">The localization functions.</param>
    public ShellViewModel(
        IContext context,
        ICommonServices commonServices,
        IBaseSettingsService settingsService,
        IAuthenticationService authenticationService,
        ILogger logger,
        IThemeService themeService,
        ILocalizationService localizationService)
        : base(context, commonServices, settingsService, authenticationService, logger, themeService, localizationService)
    {
        CommonServices = commonServices;
        SettingsService = settingsService;

        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        ControlsCommand = new AsyncRelayCommand(OpenControlsAsync);
        SlideshowCommand = new AsyncRelayCommand(OpenSlideshowAsync);
        SyncCommand = new AsyncRelayCommand(OpenSyncAsync);

        PopulateNavigationMenuItems();
    }

    private void PopulateNavigationMenuItems()
    {
        PrimaryItems.Clear();
        SecondaryItems.Clear();

        if (Context.IsAuthenticated)
        {
            PrimaryItems.Add(new NavigationItem("Info", ResourceUtility.FindResource<string>("info"), _themeService.Style.Color, InfoCommand));
            PrimaryItems.Add(new NavigationItem("Controls", ResourceUtility.FindResource<string>("search"), _themeService.Style.Color, ControlsCommand));
            PrimaryItems.Add(new NavigationItem("SlideShow", ResourceUtility.FindResource<string>("info"), _themeService.Style.Color, SlideshowCommand));
            PrimaryItems.Add(new NavigationItem("Sync", ResourceUtility.FindResource<string>("Sync"), _themeService.Style.Color, SyncCommand));

            SecondaryItems.Add(new NavigationItem("Help", ResourceUtility.FindResource<string>("help"), _themeService.Style.Color, HelpCommand));
            SecondaryItems.Add(new NavigationItem("Language", ResourceUtility.FindResource<string>("language"), _themeService.Style.Color, LanguageCommand));
            SecondaryItems.Add(new NavigationItem("Theme", ResourceUtility.FindResource<string>("theme"), _themeService.Style.Color, ColorCommand));
            SecondaryItems.Add(new NavigationItem("Settings", ResourceUtility.FindResource<string>("settings"), _themeService.Style.Color, SettingsCommand));
        }

        SecondaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", ResourceUtility.FindResource<string>("signin"), _themeService.Style.Color, SignInCommand));
    }

    protected override async Task SignOutAsync()
    {
        await base.SignOutAsync();

        if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser))
        {
            _settingsService.LocalSettings.IsAutoLogin = false;
            _settingsService.SaveLocalSettings();
        }
    }

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        CommonServices.NavigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the controls page asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenControlsAsync() =>
        CommonServices.NavigationService.NavigateAsync<ControlsViewModel>();

    /// <summary>
    /// Opens the display asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenSlideshowAsync() =>
        CommonServices.NavigationService.NavigateAsync<SlideShowViewModel>();

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        CommonServices.NavigationService.NavigateModalAsync<SettingsViewModel>();

    /// <summary>
    /// Opens the sync asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenSyncAsync() =>
        CommonServices.NavigationService.NavigateAsync<SyncViewModel>();
}
