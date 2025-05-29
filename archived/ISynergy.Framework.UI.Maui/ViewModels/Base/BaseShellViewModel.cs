﻿using ISynergy.Framework.Core.Abstractions.Services;
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
/// Class BaseShellViewModel.
/// Implements the <see cref="ViewModel" />
/// Implements the <see cref="IShellViewModel" />
/// </summary>
/// <seealso cref="ViewModel" />
/// <seealso cref="IShellViewModel" />
public abstract class BaseShellViewModel : ViewModel, IShellViewModel
{
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
    public RelayCommand SignInCommand { get; private set; }
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
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    protected BaseShellViewModel(ICommonServices commonServices, ILogger<BaseShellViewModel> logger)
        : base(commonServices, logger)
    {
        _commonServices.NavigationService.BackStackChanged += NavigationService_BackStackChanged;

        PrimaryItems = new ObservableCollection<NavigationItem>();
        SecondaryItems = new ObservableCollection<NavigationItem>();

        GoBackCommand = new AsyncRelayCommand(async () => await _commonServices.NavigationService.GoBackAsync());
        RestartUpdateCommand = new AsyncRelayCommand(ShowDialogRestartAfterUpdateAsync);
        SignInCommand = new RelayCommand(SignOut);
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
        IsBackEnabled = _commonServices.NavigationService.CanGoBack;

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
    protected virtual void SignOut() => _commonServices.AuthenticationService.SignOut();

    /// <summary>
    /// Shows the dialog restart after update asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected Task ShowDialogRestartAfterUpdateAsync() =>
        _commonServices.DialogService.ShowInformationAsync(LanguageService.Default.GetString("UpdateRestart"));

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
        return _commonServices.DialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
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

        if (await _commonServices.DialogService.ShowMessageAsync(
                    LanguageService.Default.GetString("WarningLanguageChange") +
                    Environment.NewLine +
                    LanguageService.Default.GetString("WarningDoYouWantToDoItNow"),
                    LanguageService.Default.GetString("TitleQuestion"),
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
        return _commonServices.DialogService.ShowDialogAsync(typeof(IThemeWindow), themeVM);
    }

    /// <summary>
    /// Themes the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void ThemeVM_Submitted(object? sender, SubmitEventArgs<Style> e)
    {
        if (sender is ThemeViewModel vm)
            vm.Submitted -= ThemeVM_Submitted;

        if (e.Result is { } style)
        {
            _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Theme = style.Theme;
            _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color = style.Color;

            if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings() && await _commonServices.DialogService.ShowMessageAsync(
                    LanguageService.Default.GetString("WarningColorChange") +
                    Environment.NewLine +
                    LanguageService.Default.GetString("WarningDoYouWantToDoItNow"),
                    LanguageService.Default.GetString("TitleQuestion"),
                    MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
            {
                _commonServices.RestartApplication();
            }
        }
    }

    protected virtual Task OpenHelpAsync() =>
        throw new NotImplementedException();

    protected virtual Task OpenFeedbackAsync() =>
        throw new NotImplementedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Remove navigation service event handlers first
            if (_commonServices.NavigationService is not null)
                _commonServices.NavigationService.BackStackChanged -= NavigationService_BackStackChanged;

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
