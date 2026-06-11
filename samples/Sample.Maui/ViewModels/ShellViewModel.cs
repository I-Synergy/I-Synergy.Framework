using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Utilities;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Extensions;

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

    public AsyncRelayCommand SlideshowCommand { get; private set; }
    public AsyncRelayCommand InfoCommand { get; private set; }
    public AsyncRelayCommand CustomControlsCommand { get; private set; }
    public AsyncRelayCommand MauiControlsCommand { get; private set; }
    public AsyncRelayCommand SyncCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="settingsService"></param>
    /// <param name="scopeFactory"></param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        IServiceScopeFactory scopeFactory,
        ILogger<ShellViewModel> logger)
        : base(commonServices, dialogService, navigationService, settingsService, scopeFactory, logger)
    {
        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        CustomControlsCommand = new AsyncRelayCommand(OpenCustomControlsAsync);
        MauiControlsCommand = new AsyncRelayCommand(OpenMauiControlsAsync);
        SlideshowCommand = new AsyncRelayCommand(OpenSlideshowAsync);
        SyncCommand = new AsyncRelayCommand(OpenSyncAsync);

        PopulateNavigationMenuItems();
    }

    public override async Task ShellLoadedAsync()
    {
        await Task.Delay(100);

        if (ServiceLocator.Default.GetRequiredService<IContext>().IsAuthenticated && PrimaryItems?.Count > 0)
        {
            if (PrimaryItems[0].Command!.CanExecute(PrimaryItems[0].CommandParameter))
                PrimaryItems[0].Command!.Execute(PrimaryItems[0].CommandParameter);

            SelectedItem = PrimaryItems[0];
        }

        await InitializeFirstRunAsync();
    }

    private void PopulateNavigationMenuItems()
    {
        var primaryColor = _settingsService.LocalSettings.Color ?? string.Empty;

        if (ServiceLocator.Default.GetRequiredService<IContext>().IsAuthenticated) // NOSONAR
        {
            PrimaryItems?.Clear();

            PrimaryItems?.Add(new NavigationItem("Info", FluentUI.info_20_regular ?? string.Empty, primaryColor, InfoCommand)); // NOSONAR
            PrimaryItems?.Add(new NavigationItem("CustomControls", FluentUI.control_button_20_regular ?? string.Empty, primaryColor, CustomControlsCommand)); // NOSONAR
            PrimaryItems?.Add(new NavigationItem("MAUI Controls", FluentUI.app_generic_20_regular ?? string.Empty, primaryColor, MauiControlsCommand)); // NOSONAR
            PrimaryItems?.Add(new NavigationItem("SlideShow", FluentUI.slide_layout_20_regular ?? string.Empty, primaryColor, SlideshowCommand)); // NOSONAR
            PrimaryItems?.Add(new NavigationItem("Sync", FluentUI.cloud_sync_20_regular ?? string.Empty, primaryColor, SyncCommand)); // NOSONAR

            SecondaryItems?.Clear();
            SecondaryItems?.Add(new NavigationItem("Help", FluentUI.chat_help_20_regular ?? string.Empty, primaryColor, HelpCommand)); // NOSONAR
            SecondaryItems?.Add(new NavigationItem("Language", FluentUI.local_language_20_regular ?? string.Empty, primaryColor, LanguageCommand)); // NOSONAR
            SecondaryItems?.Add(new NavigationItem("Theme", FluentUI.dark_theme_20_regular ?? string.Empty, primaryColor, ThemeCommand)); // NOSONAR
            SecondaryItems?.Add(new NavigationItem("Settings", FluentUI.settings_20_regular ?? string.Empty, primaryColor, SettingsCommand)); // NOSONAR
        }

        SecondaryItems?.Add(new NavigationItem(ServiceLocator.Default.GetRequiredService<IContext>().IsAuthenticated ? "Logout" : "Login", ResourceUtility.FindResource<string>("user2") ?? string.Empty, primaryColor, SignInCommand));
    }

    protected override Task SignOutAsync() =>
        ServiceLocator.Default.GetRequiredService<Sample.Abstractions.Services.IAuthenticationService>().SignOutAsync();

    public override async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings!.IsFirstRun)
        {
            if (await _dialogService.ShowMessageAsync(
                _commonServices.LanguageService.GetString("ChangeLanguage"),
                _commonServices.LanguageService.GetString("Language"),
                MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
            {
                using var scope = _scopeFactory.CreateScope();
                var languageVM = scope.ServiceProvider.GetRequiredService<LanguageViewModel>();
                languageVM.Submitted += (s, e) =>
                {
                    _settingsService.LocalSettings.Language = e.Result.Name;
                    _settingsService.SaveLocalSettings();
                    e.Result.SetLocalizationLanguage();
                    _commonServices.RestartApplication();
                };
                await _dialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
            }

            using var settingsScope = _scopeFactory.CreateScope();
            var wizardVM = settingsScope.ServiceProvider.GetRequiredService<SettingsViewModel>();
            wizardVM.Submitted += (s, e) =>
            {
                _commonServices.RestartApplication();
            };
            wizardVM.Cancelled += (s, e) =>
            {
                _commonServices.RestartApplication();
            };

            await _navigationService.OpenBladeAsync(this, wizardVM);
        }
    }

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        _navigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the custom controls page.
    /// </summary>
    private Task OpenCustomControlsAsync() =>
        _navigationService.NavigateAsync<CustomControlsViewModel>();

    /// <summary>
    /// Opens the MAUI controls showcase page.
    /// </summary>
    private Task OpenMauiControlsAsync() =>
        _navigationService.NavigateAsync<MauiControlsViewModel>();

    /// <summary>
    /// Opens the display asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenSlideshowAsync() =>
        _navigationService.NavigateAsync<SlideShowViewModel>();

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        _navigationService.NavigateModalAsync<SettingsViewModel>();

    /// <summary>
    /// Opens the sync asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenSyncAsync() =>
        _navigationService.NavigateAsync<SyncViewModel>();
}

