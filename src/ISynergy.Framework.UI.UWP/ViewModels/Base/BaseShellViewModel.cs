using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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

namespace ISynergy.Framework.UI.ViewModels.Base;

/// <summary>
/// Class ShellViewModelBase.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IShellViewModel" />
/// </summary>
/// <seealso cref="ViewModel" />
/// <seealso cref="IShellViewModel" />
public abstract class BaseShellViewModel : ViewModelBladeView<NavigationItem>, IShellViewModel
{
    protected readonly INavigationService _navigationService;
    protected readonly IDialogService _dialogService;

    /// <summary>
    /// Gets or sets the IsBackEnabled property value.
    /// </summary>
    public bool IsBackEnabled
    {
        get => GetValue<bool>();
        private set => SetValue(value);
    }

    public AsyncRelayCommand GoBackCommand { get; private set; }
    public AsyncRelayCommand RestartUpdateCommand { get; private set; }
    public AsyncRelayCommand SignInCommand { get; private set; }
    public AsyncRelayCommand LanguageCommand { get; private set; }
    public AsyncRelayCommand ColorCommand { get; private set; }
    public AsyncRelayCommand HelpCommand { get; private set; }
    public AsyncRelayCommand SettingsCommand { get; private set; }
    public AsyncRelayCommand BackgroundCommand { get; private set; }
    public AsyncRelayCommand FeedbackCommand { get; private set; }

    /// <summary>
    /// Gets or sets the PrimaryItems property value.
    /// </summary>
    /// <value>The primary items.</value>
    public ObservableCollection<NavigationItem> PrimaryItems
    {
        get => GetValue<ObservableCollection<NavigationItem>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the SecondaryItems property value.
    /// </summary>
    /// <value>The primary items.</value>
    public ObservableCollection<NavigationItem> SecondaryItems
    {
        get => GetValue<ObservableCollection<NavigationItem>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="logger"></param>
    protected BaseShellViewModel(ICommonServices commonServices, IDialogService dialogService, INavigationService navigationService, ILogger<BaseShellViewModel> logger)
        : base(commonServices, logger)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;

        _navigationService.BackStackChanged += NavigationService_BackStackChanged;

        // Initialize IsBackEnabled with the current state
        IsBackEnabled = _navigationService.CanGoBack;

        PrimaryItems = new ObservableCollection<NavigationItem>();
        SecondaryItems = new ObservableCollection<NavigationItem>();

        GoBackCommand = new AsyncRelayCommand(async () => await _navigationService.GoBackAsync());
        RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
        SignInCommand = new AsyncRelayCommand(SignOutAsync);
        LanguageCommand = new AsyncRelayCommand(OpenLanguageAsync);
        ColorCommand = new AsyncRelayCommand(OpenColorsAsync);
        HelpCommand = new AsyncRelayCommand(OpenHelpAsync);
        FeedbackCommand = new AsyncRelayCommand(OpenFeedbackAsync);
        SettingsCommand = new AsyncRelayCommand(OpenSettingsAsync);
        BackgroundCommand = new AsyncRelayCommand(OpenBackgroundAsync);
    }

    public abstract Task ShellLoadedAsync();

    public abstract Task InitializeFirstRunAsync();


    private void NavigationService_BackStackChanged(object? sender, EventArgs e) =>
        IsBackEnabled = _navigationService.CanGoBack;

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected abstract Task OpenSettingsAsync();

    /// <summary>
    /// Sets the background of the application asynchronous.
    /// </summary>
    /// <returns></returns>
    protected abstract Task OpenBackgroundAsync();

    /// <summary>
    /// Sign out.
    /// </summary>
    /// <returns></returns>
    protected abstract Task SignOutAsync();

    /// <summary>
    /// Shows the dialog restart after update asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected Task ShowDialogRestartAfterUpdateAsync() =>
        _dialogService.ShowInformationAsync(_commonServices.LanguageService.GetString("UpdateRestart"));

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
        var languageVM = _commonServices.ScopedContextService.GetRequiredService<LanguageViewModel>();
        languageVM.Submitted += LanguageVM_Submitted;
        return _dialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
    }

    /// <summary>
    /// Languages the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void LanguageVM_Submitted(object? sender, SubmitEventArgs<Languages> e)
    {
        if (sender is LanguageViewModel vm)
            vm.Submitted -= LanguageVM_Submitted;

        _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Language = e.Result;
        _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();

        e.Result.SetLocalizationLanguage();

        if (await _dialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("WarningLanguageChange") +
                    Environment.NewLine +
                    _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    _commonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
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
        var themeVM = _commonServices.ScopedContextService.GetRequiredService<ThemeViewModel>();
        themeVM.Submitted += ThemeVM_Submitted;
        return _dialogService.ShowDialogAsync(typeof(IThemeWindow), themeVM);
    }

    /// <summary>
    /// Themes the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void ThemeVM_Submitted(object? sender, SubmitEventArgs<ThemeStyle> e)
    {
        if (sender is ThemeViewModel vm)
        {
            vm.Submitted -= ThemeVM_Submitted;

            _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Theme = e.Result.Theme;
            _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color = (e.Result.Color ?? string.Empty).ToLowerInvariant();

            if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings() &&
                await _dialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("WarningColorChange") +
                    Environment.NewLine +
                    _commonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                    _commonServices.LanguageService.GetString("TitleQuestion"),
                    MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
            {
                _commonServices.RestartApplication();
            }
        }
    }

    /// <summary>
    /// Opens the help asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Default implementation does nothing. Override in derived classes to provide help functionality.
    /// </remarks>
    protected virtual Task OpenHelpAsync() => Task.CompletedTask;

    /// <summary>
    /// Opens the feedback dialog asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Default implementation does nothing. Override in derived classes to provide feedback functionality.
    /// </remarks>
    protected virtual Task OpenFeedbackAsync() => Task.CompletedTask;

    /// <summary>
    /// Adds a new item asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Default implementation does nothing. Override in derived classes to provide add functionality.
    /// </remarks>
    public override Task AddAsync() => Task.CompletedTask;

    /// <summary>
    /// Edits the specified item asynchronously.
    /// </summary>
    /// <param name="e">The item to edit.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Default implementation does nothing. Override in derived classes to provide edit functionality.
    /// </remarks>
    public override Task EditAsync(NavigationItem e) => Task.CompletedTask;

    /// <summary>
    /// Removes the specified item asynchronously.
    /// </summary>
    /// <param name="e">The item to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Default implementation does nothing. Override in derived classes to provide remove functionality.
    /// </remarks>
    public override Task RemoveAsync(NavigationItem e) => Task.CompletedTask;

    /// <summary>
    /// Searches for items asynchronously.
    /// </summary>
    /// <param name="e">The search criteria.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Default implementation does nothing. Override in derived classes to provide search functionality.
    /// </remarks>
    public override Task SearchAsync(object e) => Task.CompletedTask;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Remove navigation service event handlers first
            if (_navigationService is not null)
                _navigationService.BackStackChanged -= NavigationService_BackStackChanged;

            Validator = null;

            PrimaryItems?.Clear();
            SecondaryItems?.Clear();

            RestartUpdateCommand?.Dispose();
            SignInCommand?.Dispose();
            LanguageCommand?.Dispose();
            ColorCommand?.Dispose();
            HelpCommand?.Dispose();
            SettingsCommand?.Dispose();
            BackgroundCommand?.Dispose();
            FeedbackCommand?.Dispose();

            // Finally dispose context and other resources
            base.Dispose(disposing);
        }
    }
}
