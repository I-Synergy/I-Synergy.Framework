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
using ISynergy.Framework.UI.Extensions;
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
        get => _commonServices.NavigationService.CanGoBack;
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
    protected BaseShellViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        ISettingsService settingsService,
        IAuthenticationService authenticationService,
        ILogger logger,
        IThemeService themeService)
        : base(context, commonServices, logger)
    {
        _commonServices.NavigationService.BackStackChanged += NavigationService_BackStackChanged;

        PrimaryItems = new ObservableCollection<NavigationItem>();
        SecondaryItems = new ObservableCollection<NavigationItem>();

        _settingsService = settingsService;
        _authenticationService = authenticationService;
        _themeService = themeService;

        RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
        SignInCommand = new AsyncRelayCommand(SignOutAsync);
        LanguageCommand = new AsyncRelayCommand(OpenLanguageAsync);
        ColorCommand = new AsyncRelayCommand(OpenColorsAsync);
        HelpCommand = new AsyncRelayCommand(OpenHelpAsync);
        FeedbackCommand = new AsyncRelayCommand(OpenFeedbackAsync);
        SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
    }

    private void NavigationService_BackStackChanged(object sender, EventArgs e) => OnPropertyChanged(nameof(IsBackEnabled));

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
        _commonServices.DialogService.ShowInformationAsync(_commonServices.LanguageService.GetString("UpdateRestart"));

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
        var languageVM = new LanguageViewModel(_context, _commonServices, _logger, _settingsService.LocalSettings.Language);
        languageVM.Submitted += LanguageVM_Submitted;
        return _commonServices.DialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
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

        e.Result.SetLocalizationLanguage(_context);

        if (await _commonServices.DialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("WarningLanguageChange") +
                    Environment.NewLine +
                    _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    _commonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            _commonServices.RestartApplication();
        }
    }

    /// <summary>
    /// Opens the colors asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected virtual Task OpenColorsAsync()
    {
        var themeVM = new ThemeViewModel(_context, _commonServices, _settingsService, _logger);
        themeVM.Submitted += ThemeVM_Submitted;
        return _commonServices.DialogService.ShowDialogAsync(typeof(IThemeWindow), themeVM);
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

            if (await _commonServices.DialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("WarningColorChange") +
                    Environment.NewLine +
                    _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    _commonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _commonServices.RestartApplication();
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
        if (disposing)
        {
            // Remove navigation service event handlers first
            if (_commonServices.NavigationService is not null)
                _commonServices.NavigationService.BackStackChanged -= NavigationService_BackStackChanged;

            Validator = null;

            (RestartUpdateCommand as IDisposable)?.Dispose();
            (SignInCommand as IDisposable)?.Dispose();
            (LanguageCommand as IDisposable)?.Dispose();
            (ColorCommand as IDisposable)?.Dispose();
            (HelpCommand as IDisposable)?.Dispose();
            (SettingsCommand as IDisposable)?.Dispose();
            (BackgroundCommand as IDisposable)?.Dispose();
            (FeedbackCommand as IDisposable)?.Dispose();

            // Finally dispose context and other resources
            base.Dispose(disposing);
        }
    }
}
