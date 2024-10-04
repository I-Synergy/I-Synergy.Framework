using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Windows;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.ViewModels.Base;

/// <summary>
/// Class BaseShellViewModel.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IShellViewModel" />
/// </summary>
/// <seealso cref="ViewModel" />
/// <seealso cref="IShellViewModel" />
public abstract class BaseShellViewModel : ViewModelBladeView<NavigationItem>, IShellViewModel
{
    /// <summary>
    /// Gets or sets the IsBackEnabled property value.
    /// </summary>
    public bool IsBackEnabled
    {
        get => BaseCommonServices.NavigationService.CanGoBack;
    }

    /// <summary>
    /// Gets or sets the restart update command.
    /// </summary>
    /// <value>The restart update command.</value>
    public AsyncRelayCommand RestartUpdateCommand { get; private set; }

    /// <summary>
    /// Gets or sets the login command.
    /// </summary>
    /// <value>The login command.</value>
    public AsyncRelayCommand SignInCommand { get; private set; }

    /// <summary>
    /// Gets or sets the language command.
    /// </summary>
    /// <value>The language command.</value>
    public AsyncRelayCommand LanguageCommand { get; private set; }

    /// <summary>
    /// Gets or sets the color command.
    /// </summary>
    /// <value>The color command.</value>
    public AsyncRelayCommand ColorCommand { get; private set; }

    /// <summary>
    /// Gets or sets the help command.
    /// </summary>
    /// <value>The help command.</value>
    public AsyncRelayCommand HelpCommand { get; private set; }

    /// <summary>
    /// Gets or sets the settings command.
    /// </summary>
    /// <value>The settings command.</value>
    public AsyncRelayCommand SettingsCommand { get; private set; }

    /// <summary>
    /// Gets or sets the background command.
    /// </summary>
    /// <value>The background command.</value>
    public AsyncRelayCommand BackgroundCommand { get; private set; }

    /// <summary>
    /// Gets or sets the feedback command.
    /// </summary>
    /// <value>The feedback command.</value>
    public AsyncRelayCommand FeedbackCommand { get; private set; }

    /// <summary>
    /// Authentication service.
    /// </summary>
    protected readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// The settings service.
    /// </summary>
    protected readonly ISettingsService _settingsService;

    /// <summary>
    /// The theme selector
    /// </summary>
    protected readonly IThemeService _themeService;

    /// <summary>
    /// The localization functions
    /// </summary>
    protected readonly ILocalizationService _localizationService;


    /// <summary>
    /// Gets or sets the PrimaryItems property value.
    /// </summary>
    /// <value>The primary items.</value>
    public ObservableCollection<NavigationItem> PrimaryItems
    {
        get => GetValue<ObservableCollection<NavigationItem>>();
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the SecondaryItems property value.
    /// </summary>
    /// <value>The primary items.</value>
    public ObservableCollection<NavigationItem> SecondaryItems
    {
        get => GetValue<ObservableCollection<NavigationItem>>();
        private set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseShellViewModel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="settingsService">The settings services.</param>
    /// <param name="authenticationService"></param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="themeService">The theme selector service.</param>
    /// <param name="LocalizationService">The localization functions.</param>
    protected BaseShellViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ISettingsService settingsService,
        IAuthenticationService authenticationService,
        ILogger logger,
        IThemeService themeService,
        ILocalizationService LocalizationService)
        : base(context, commonServices, logger)
    {
        BaseCommonServices.NavigationService.BackStackChanged += (s, e) => OnPropertyChanged(nameof(IsBackEnabled));

        PrimaryItems = new ObservableCollection<NavigationItem>();
        SecondaryItems = new ObservableCollection<NavigationItem>();

        _settingsService = settingsService;
        _authenticationService = authenticationService;
        _themeService = themeService;
        _localizationService = LocalizationService;

        RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
        SignInCommand = new AsyncRelayCommand(SignOutAsync);
        LanguageCommand = new AsyncRelayCommand(OpenLanguageAsync);
        ColorCommand = new AsyncRelayCommand(OpenColorsAsync);
        HelpCommand = new AsyncRelayCommand(OpenHelpAsync);
        FeedbackCommand = new AsyncRelayCommand(OpenFeedbackAsync);
        SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
    }

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected abstract Task OpenSettingsAsync();

    /// <summary>
    /// Sign out.
    /// </summary>
    /// <returns></returns>
    protected virtual Task SignOutAsync() => _authenticationService.SignOutAsync();

    /// <summary>
    /// Shows the dialog restart after update asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected Task ShowDialogRestartAfterUpdateAsync() =>
        BaseCommonServices.DialogService.ShowInformationAsync(BaseCommonServices.LanguageService.GetString("UpdateRestart"));

    /// <summary>
    /// Gets or sets the LastSelectedItem property value.
    /// </summary>
    /// <value>The last selected item.</value>
    public NavigationItem LastSelectedItem
    {
        get => GetValue<NavigationItem>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Query property value.
    /// </summary>
    /// <value>The query.</value>
    public string Query
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Caption property value.
    /// </summary>
    /// <value>The caption.</value>
    public string Caption
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the IsUpdateAvailable property value.
    /// </summary>
    /// <value><c>true</c> if this instance is update available; otherwise, <c>false</c>.</value>
    public bool IsUpdateAvailable
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the Width property value.
    /// </summary>
    /// <value>The width.</value>
    public double Width
    {
        get => GetValue<double>();
        set => SetValue(value);
    }

    /// <summary>
    /// Opens the language asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected virtual Task OpenLanguageAsync()
    {
        var languageVM = new LanguageViewModel(Context, BaseCommonServices, Logger, _settingsService.LocalSettings.Language);
        languageVM.Submitted += LanguageVM_Submitted;
        return BaseCommonServices.DialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
    }

    /// <summary>
    /// Languages the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void LanguageVM_Submitted(object sender, SubmitEventArgs<Languages> e)
    {
        if (sender is LanguageViewModel vm)
            vm.Submitted -= LanguageVM_Submitted;

        _settingsService.LocalSettings.Language = e.Result;
        _settingsService.SaveLocalSettings();
        _localizationService.SetLocalizationLanguage(e.Result);

        if (await BaseCommonServices.DialogService.ShowMessageAsync(
                    BaseCommonServices.LanguageService.GetString("WarningLanguageChange") +
                    Environment.NewLine +
                    BaseCommonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    BaseCommonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            BaseCommonServices.RestartApplication();
        }
    }

    /// <summary>
    /// Opens the colors asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected virtual Task OpenColorsAsync()
    {
        var themeVM = new ThemeViewModel(Context, BaseCommonServices, _settingsService, Logger);
        themeVM.Submitted += ThemeVM_Submitted;
        return BaseCommonServices.DialogService.ShowDialogAsync(typeof(IThemeWindow), themeVM);
    }

    /// <summary>
    /// Themes the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void ThemeVM_Submitted(object sender, SubmitEventArgs<Style> e)
    {
        if (sender is ThemeViewModel vm)
            vm.Submitted -= ThemeVM_Submitted;

        if (e.Result is { } style)
        {
            _settingsService.LocalSettings.Theme = style.Theme;
            _settingsService.LocalSettings.Color = style.Color;
            _settingsService.SaveLocalSettings();

            if (await BaseCommonServices.DialogService.ShowMessageAsync(
                    BaseCommonServices.LanguageService.GetString("WarningColorChange") +
                    Environment.NewLine +
                    BaseCommonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    BaseCommonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                BaseCommonServices.RestartApplication();
            }
        }
    }

    protected virtual Task OpenHelpAsync() => 
        throw new NotImplementedException();

    protected virtual Task OpenFeedbackAsync() =>
        throw new NotImplementedException();

    public override Task AddAsync() =>
        throw new NotImplementedException();

    public override Task EditAsync(NavigationItem e) =>
        throw new NotImplementedException();

    public override Task RemoveAsync(NavigationItem e) =>
        throw new NotImplementedException();

    public override Task SearchAsync(object e) =>
        throw new NotImplementedException();

    protected override void Dispose(bool disposing)
    {
        Validator = null;

        RestartUpdateCommand?.Cancel();
        RestartUpdateCommand = null;
        SignInCommand?.Cancel();
        SignInCommand = null;
        LanguageCommand?.Cancel();
        LanguageCommand = null;
        ColorCommand?.Cancel();
        ColorCommand = null;
        HelpCommand?.Cancel();
        HelpCommand = null;
        SettingsCommand?.Cancel();
        SettingsCommand = null;
        BackgroundCommand?.Cancel();
        BackgroundCommand = null;
        FeedbackCommand?.Cancel();
        FeedbackCommand = null;

        base.Dispose(disposing);
    }
}
