﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.UI.Xaml;
using Sample.Abstractions;
using Sample.Models;

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly DispatcherTimer _clockTimer;

    public AsyncRelayCommand? DisplayCommand { get; private set; }
    public AsyncRelayCommand? InfoCommand { get; private set; }
    public AsyncRelayCommand? BrowseCommand { get; private set; }
    public AsyncRelayCommand? ConverterCommand { get; private set; }
    public AsyncRelayCommand? SelectionTestCommand { get; private set; }
    public AsyncRelayCommand? ListViewTestCommand { get; private set; }
    public AsyncRelayCommand? ValidationTestCommand { get; private set; }
    public AsyncRelayCommand? TreeNodeTestCommand { get; private set; }
    public AsyncRelayCommand? ChartCommand { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    public ShellViewModel(ICommonServices commonServices)
        : base(commonServices)
    {
        _commonServices.AuthenticationService.SoftwareEnvironmentChanged += OnSoftwareEnvironmentChanged;

        SetClock();

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _clockTimer.Tick += ClockTimerCallBack;
        _clockTimer.Start();

        DisplayCommand = new AsyncRelayCommand(OpenDisplayAsync);
        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        BrowseCommand = new AsyncRelayCommand(BrowseFileAsync);
        ConverterCommand = new AsyncRelayCommand(OpenConvertersAsync);
        SelectionTestCommand = new AsyncRelayCommand(OpenSelectionTestAsync);
        ListViewTestCommand = new AsyncRelayCommand(OpenListViewTestAsync);
        ValidationTestCommand = new AsyncRelayCommand(OpenValidationTestAsync);
        TreeNodeTestCommand = new AsyncRelayCommand(OpenTreenNodeTestAsync);
        ChartCommand = new AsyncRelayCommand(OpenChartTestAsync);

        PopulateNavigationMenuItems();
    }

    public override async Task ShellLoadedAsync()
    {
        await Task.Delay(100);

        if (_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated && PrimaryItems?.Count > 0)
        {
            if (PrimaryItems[0].Command.CanExecute(PrimaryItems[0].CommandParameter))
                PrimaryItems[0].Command.Execute(PrimaryItems[0].CommandParameter);

            SelectedItem = PrimaryItems[0];
        }

        await InitializeFirstRunAsync();
    }

    private void OnSoftwareEnvironmentChanged(object? sender, ReturnEventArgs<SoftwareEnvironments> e) => SetClock();

    private void PopulateNavigationMenuItems()
    {
        if (_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated)
        {
            PrimaryItems.Clear();
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["info"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, InfoCommand));
            PrimaryItems.Add(new NavigationItem("SlideShow", Application.Current.Resources["kiosk"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, DisplayCommand));
            PrimaryItems.Add(new NavigationItem("Browse", Application.Current.Resources["search"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, BrowseCommand));
            PrimaryItems.Add(new NavigationItem("Converters", Application.Current.Resources["products"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, ConverterCommand));
            PrimaryItems.Add(new NavigationItem("Selection", Application.Current.Resources["multiselect"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, SelectionTestCommand));
            PrimaryItems.Add(new NavigationItem("ListView", Application.Current.Resources["products"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, ListViewTestCommand));
            PrimaryItems.Add(new NavigationItem("Validation", Application.Current.Resources["Validation"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, ValidationTestCommand));
            PrimaryItems.Add(new NavigationItem("TreeView", Application.Current.Resources["TreeView"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, TreeNodeTestCommand));
            PrimaryItems.Add(new NavigationItem("Charts", Application.Current.Resources["chart"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, ChartCommand));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, HelpCommand));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["flag"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, LanguageCommand));
            SecondaryItems.Add(new NavigationItem("Theme", Application.Current.Resources["color"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, ColorCommand));

            if (_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated && _commonServices.ScopedContextService.GetRequiredService<IContext>().Profile is Profile)
                SecondaryItems.Add(new NavigationItem("Settings", Application.Current.Resources["settings"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, SettingsCommand));
        }

        SecondaryItems.Add(new NavigationItem(_commonServices.ScopedContextService.GetRequiredService<IContext>().IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["user2"], _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Color, SignInCommand));
    }

    protected override void SignOut()
    {
        try
        {
            // Clear profile before base sign out
            base.SignOut();

            if (!string.IsNullOrEmpty(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser))
            {
                _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.IsAutoLogin = false;
                _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();
            }
        }
        catch (ObjectDisposedException)
        {
            // Context already disposed, nothing to sign out
        }
    }

    public override async Task InitializeFirstRunAsync()
    {
        if (_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().GlobalSettings!.IsFirstRun)
        {
            if (await _commonServices.DialogService.ShowMessageAsync(
                LanguageService.Default.GetString("ChangeLanguage"),
                LanguageService.Default.GetString("Language"),
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var languageVM = new LanguageViewModel(_commonServices);
                languageVM.Submitted += (s, e) =>
                {
                    _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.Language = e.Result;
                    _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().SaveLocalSettings();
                    e.Result.SetLocalizationLanguage();
                    _commonServices.RestartApplication();
                };
                await _commonServices.DialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
            }

            var wizardVM = new SettingsViewModel(_commonServices);
            wizardVM.Submitted += (s, e) =>
            {
                _commonServices.RestartApplication();
            };
            wizardVM.Cancelled += (s, e) =>
            {
                _commonServices.RestartApplication();
            };

            await _commonServices.NavigationService.OpenBladeAsync(this, wizardVM);
        }
    }

    private void ClockTimerCallBack(object? sender, object e) => SetClock();

    private void SetClock() => base.Title = $"{_commonServices.InfoService.Title} - {DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";

    private Task OpenChartTestAsync() =>
        base._commonServices.NavigationService.NavigateAsync<ChartsViewModel>();

    private Task OpenTreenNodeTestAsync() =>
        base._commonServices.NavigationService.NavigateAsync<TreeNodeViewModel, ITreeNodeView>();

    /// <summary>
    /// Opens the validation test asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenValidationTestAsync() =>
        base._commonServices.NavigationService.NavigateAsync<ValidationViewModel>();

    /// <summary>
    /// Opens the ListView test asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenListViewTestAsync() =>
        base._commonServices.NavigationService.NavigateAsync<TestItemsListViewModel>();

    /// <summary>
    /// Opens the converters asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenConvertersAsync() =>
        base._commonServices.NavigationService.NavigateAsync<ConvertersViewModel>();

    /// <summary>
    /// browse file as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task BrowseFileAsync()
    {
        string imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

        if (await _commonServices.FileService.BrowseFileAsync(imageFilter) is { } files && files.Count > 0)
            await _commonServices.DialogService.ShowInformationAsync($"File '{files[0].FileName}' is selected.");
    }

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        base._commonServices.NavigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the display asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenDisplayAsync() =>
        base._commonServices.NavigationService.NavigateAsync<SlideShowViewModel>();

    /// <summary>
    /// Opens the selection test asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenSelectionTestAsync() =>
       base._commonServices.NavigationService.NavigateAsync<SelectionTestViewModel>();

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        base._commonServices.NavigationService.NavigateModalAsync<SettingsViewModel>();

    protected override Task OpenBackgroundAsync() => throw new NotImplementedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_clockTimer is not null)
            {
                _clockTimer.Stop();
                _clockTimer.Tick -= ClockTimerCallBack;
            }

            if (_commonServices.AuthenticationService is not null)
                _commonServices.AuthenticationService.SoftwareEnvironmentChanged -= OnSoftwareEnvironmentChanged;

            DisplayCommand?.Dispose();
            DisplayCommand = null;
            InfoCommand?.Dispose();
            InfoCommand = null;
            BrowseCommand?.Dispose();
            BrowseCommand = null;
            ConverterCommand?.Dispose();
            ConverterCommand = null;
            SelectionTestCommand?.Dispose();
            SelectionTestCommand = null;
            ListViewTestCommand?.Dispose();
            ListViewTestCommand = null;
            ValidationTestCommand?.Dispose();
            ValidationTestCommand = null;
            TreeNodeTestCommand?.Dispose();
            TreeNodeTestCommand = null;
            ChartCommand?.Dispose();
            ChartCommand = null;

            base.Dispose(disposing);
        }
    }
}
