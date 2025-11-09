using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
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
using Sample.Abstractions.Services;
using Sample.Extensions;
using Timer = System.Timers.Timer;

namespace Sample.ViewModels;

public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly Timer _clockTimer;

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
    public AsyncRelayCommand ControlsCommand { get; private set; }
    public AsyncRelayCommand SyncCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<ShellViewModel> logger)
        : base(commonServices, dialogService, navigationService, logger)
    {
        SetClock();

        _clockTimer = new Timer(TimeSpan.FromSeconds(30));
        _clockTimer.Elapsed += ClockTimerCallBack;
        _clockTimer.Start();

        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        ControlsCommand = new AsyncRelayCommand(OpenControlsAsync);
        SlideshowCommand = new AsyncRelayCommand(OpenSlideshowAsync);
        SyncCommand = new AsyncRelayCommand(OpenSyncAsync);

        PopulateNavigationMenuItems();
    }

    public override async Task ShellLoadedAsync()
    {
        await Task.Delay(100);

        if (_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated && PrimaryItems?.Count > 0)
        {
            if (PrimaryItems[0].Command!.CanExecute(PrimaryItems[0].CommandParameter))
                PrimaryItems[0].Command!.Execute(PrimaryItems[0].CommandParameter);

            SelectedItem = PrimaryItems[0];
        }

        await InitializeFirstRunAsync();
    }

    private void PopulateNavigationMenuItems()
    {
        var primaryColor = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color ?? string.Empty;

        if (_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated)
        {
            PrimaryItems.Clear();

            PrimaryItems.Add(new NavigationItem("Info", FluentUI.info_20_regular ?? string.Empty, primaryColor, InfoCommand));
            PrimaryItems.Add(new NavigationItem("Controls", FluentUI.control_button_20_regular ?? string.Empty, primaryColor, ControlsCommand));
            PrimaryItems.Add(new NavigationItem("SlideShow", FluentUI.slide_layout_20_regular ?? string.Empty, primaryColor, SlideshowCommand));
            PrimaryItems.Add(new NavigationItem("Sync", FluentUI.cloud_sync_20_regular ?? string.Empty, primaryColor, SyncCommand));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", FluentUI.chat_help_20_regular ?? string.Empty, primaryColor, HelpCommand));
            SecondaryItems.Add(new NavigationItem("Language", FluentUI.local_language_20_regular ?? string.Empty, primaryColor, LanguageCommand));
            SecondaryItems.Add(new NavigationItem("Theme", FluentUI.dark_theme_20_regular ?? string.Empty, primaryColor, ColorCommand));
            SecondaryItems.Add(new NavigationItem("Settings", FluentUI.settings_20_regular ?? string.Empty, primaryColor, SettingsCommand));
        }

        SecondaryItems.Add(new NavigationItem(_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated ? "Logout" : "Login", ResourceUtility.FindResource<string>("user2") ?? string.Empty, primaryColor, SignInCommand));
    }

    protected override Task SignOutAsync() =>
        _commonServices.ScopedContextService.GetRequiredService<IAuthenticationService>().SignOutAsync();

    public override async Task InitializeFirstRunAsync()
    {
        if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().GlobalSettings!.IsFirstRun)
        {
            if (await _dialogService.ShowMessageAsync(
                LanguageService.Default.GetString("ChangeLanguage"),
                LanguageService.Default.GetString("Language"),
                MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
            {
                var languageVM = _commonServices.ScopedContextService.GetRequiredService<LanguageViewModel>();
                languageVM.Submitted += (s, e) =>
                {
                    _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Language = e.Result;
                    _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();
                    e.Result.SetLocalizationLanguage();
                    _commonServices.RestartApplication();
                };
                await _dialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
            }

            var wizardVM = _commonServices.ScopedContextService.GetRequiredService<SettingsViewModel>();
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

    private void ClockTimerCallBack(object? sender, System.Timers.ElapsedEventArgs e) => SetClock();

    private void SetClock()
    {
        if (_commonServices.ScopedContextService.GetRequiredService<IContext>() is Context context)
            base.Title = $"{_commonServices.InfoService.ProductName} v{_commonServices.InfoService.ProductVersion} ({Environment.GetEnvironmentVariable(nameof(Environment))}) - {DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";
    }

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        _navigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the controls page asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenControlsAsync() =>
        _navigationService.NavigateAsync<ControlsViewModel>();

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
