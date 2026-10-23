using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NugetUnlister.ViewModels;
using Sample.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel
{
    private readonly IFileService<FileResult> _fileService;
    private readonly IThemeService _themeService;

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
    /// Gets or sets the Items property value.
    /// </summary>
    /// <value>The items.</value>
    public ObservableCollection<TestItem> Items
    {
        get => GetValue<ObservableCollection<TestItem>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the selected test items.
    /// </summary>
    /// <value>The selected test items.</value>
    public ObservableCollection<TestItem> SelectedTestItems { get; set; } = [];

    public AsyncRelayCommand InfoCommand { get; private set; }
    public AsyncRelayCommand BrowseCommand { get; private set; }

    public AsyncRelayCommand EditableComboCommand { get; private set; }
    public AsyncRelayCommand UnitConversionCommand { get; private set; }
    public AsyncRelayCommand ValidationTestCommand { get; private set; }
    public AsyncRelayCommand NugetUnlisterCommand { get; private set; }
    public AsyncRelayCommand SelectSingleCommand { get; private set; }
    public AsyncRelayCommand SelectMultipleCommand { get; private set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="settingsService"></param>
    /// <param name="logger"></param>
    /// <param name="fileService"></param>
    /// <param name="themeService">The theme service for managing theme and accent color.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        ILogger<ShellViewModel> logger,
        IFileService<FileResult> fileService,
        IThemeService themeService)
        : base(commonServices, dialogService, navigationService, settingsService, logger)
    {
        _fileService = fileService;
        _themeService = themeService;

        Title = _commonServices.InfoService.ProductName;
        Version = _commonServices.InfoService.ProductVersion;

        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        BrowseCommand = new AsyncRelayCommand(BrowseFileAsync);
        EditableComboCommand = new AsyncRelayCommand(OpenEditableComboAsync);
        ValidationTestCommand = new AsyncRelayCommand(OpenValidationTestAsync);
        UnitConversionCommand = new AsyncRelayCommand(OpenUnitConversionAsync);
        NugetUnlisterCommand = new AsyncRelayCommand(UnlistNugetAsync);
        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);

        Items =
        [
            new TestItem { Id = 1, Description = "Test 1" },
            new TestItem { Id = 2, Description = "Test 2" },
            new TestItem { Id = 3, Description = "Test 3" },
            new TestItem { Id = 4, Description = "Test 4" },
            new TestItem { Id = 5, Description = "Test 5" }
        ];

        PopulateNavigationMenuItems();
    }

    private void PopulateNavigationMenuItems()
    {
        PrimaryItems?.Clear();

        if (Application.Services.GetRequiredService<IContext>().IsAuthenticated)
        {
            PrimaryItems?.Add(new NavigationItem("Info", Application.Current.Resources["info"], _settingsService.LocalSettings.Color, InfoCommand));
            PrimaryItems?.Add(new NavigationItem("Browse", Application.Current.Resources["search"], _settingsService.LocalSettings.Color, BrowseCommand));
            PrimaryItems?.Add(new NavigationItem("Editable Combobox", Application.Current.Resources["combobox"], _settingsService.LocalSettings.Color, EditableComboCommand));
            PrimaryItems?.Add(new NavigationItem("Validation", Application.Current.Resources["validation"], _settingsService.LocalSettings.Color, ValidationTestCommand));
            PrimaryItems?.Add(new NavigationItem("Unit Conversion", Application.Current.Resources["weight"], _settingsService.LocalSettings.Color, UnitConversionCommand));
            PrimaryItems?.Add(new NavigationItem("Select single item", Application.Current.Resources["info"], _settingsService.LocalSettings.Color, SelectSingleCommand));
            PrimaryItems?.Add(new NavigationItem("Select multiple items", Application.Current.Resources["info"], _settingsService.LocalSettings.Color, SelectMultipleCommand));
            PrimaryItems?.Add(new NavigationItem("Nuget Unlister", Application.Current.Resources["info"], _settingsService.LocalSettings.Color, NugetUnlisterCommand));
        }

        PrimaryItems?.Add(new NavigationItem("Theme", Application.Current.Resources["color"], _settingsService.LocalSettings.Color, ThemeCommand));
        PrimaryItems?.Add(new NavigationItem(Application.Services.GetRequiredService<IContext>().IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["user2"], _settingsService.LocalSettings.Color, SignInCommand));
    }

    private Task SelectSingleAsync() =>
        _dialogService.ShowDialogAsync<ISingleSelectionWindow, ViewModelSingleSelectionDialog<TestItem>, TestItem>(
            async item => await _dialogService.ShowInformationAsync($"{item.Description} selected."),
            initialization: vm => vm.SetItems(Items));

    private Task SelectMultipleAsync() =>
        _dialogService.ShowDialogAsync<IMultipleSelectionWindow, ViewModelMultipleSelectionDialog<TestItem>, List<TestItem>>(
            SelectedTestItems.ToList(),
            async items =>
            {
                SelectedTestItems = new ObservableCollection<TestItem>();
                SelectedTestItems.AddRange(items);
                await _dialogService.ShowInformationAsync($"{string.Join(", ", items.Select(s => s.Description))} selected.");
            },
            initialization: vm => vm.SetItems(Items));

    private Task UnlistNugetAsync() =>
        _navigationService.NavigateAsync<NugetViewModel>();

    /// <summary>
    /// Opens the Unit conversion view asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenUnitConversionAsync() =>
        _navigationService.NavigateAsync<UnitConversionViewModel>();


    /// <summary>
    /// Opens the validation test asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenValidationTestAsync() =>
        _navigationService.NavigateAsync<ValidationViewModel>();

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        _navigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the editable combobox sample.
    /// </summary>
    /// <returns></returns>
    private Task OpenEditableComboAsync() =>
        _navigationService.NavigateAsync<EditableComboViewModel>();

    /// <summary>
    /// browse file as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task BrowseFileAsync()
    {
        string imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

        if (await _fileService.BrowseFileAsync(imageFilter) is { } files && files.Count > 0)
            await _dialogService.ShowInformationAsync($"File '{files[0].FileName}' is selected.");
    }

    /// <summary>
    /// Opens the settings asynchronous.
    /// Demonstrates live theme and language switching using framework services.
    /// </summary>
    /// <returns>Task.</returns>
    protected override async Task OpenSettingsAsync()
    {
        // Simple implementation: toggle between light and dark themes live using IThemeService
        var nextTheme = _themeService.Theme == Themes.Light ? Themes.Dark : Themes.Light;
        _themeService.SetTheme(nextTheme);

        // Show confirmation with current settings
        await _dialogService.ShowInformationAsync(
            $"Theme: {nextTheme}\nAccent: {_themeService.AccentColor}");
    }

    /// <summary>
    /// Opens the theme/color selection asynchronously.
    /// Overrides the base implementation to use live theme switching without restart.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenThemesAsync()
    {
        // Toggle between light and dark theme live using IThemeService
        var nextTheme = _themeService.Theme == Themes.Light ? Themes.Dark : Themes.Light;
        _themeService.SetTheme(nextTheme);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Opens the language selection asynchronously.
    /// Overrides the base implementation to use live language switching without restart.
    /// </summary>
    /// <returns>Task.</returns>
    protected override async Task OpenLanguageAsync()
    {
        // Simple live language switch: toggle between English and Dutch
        var currentLanguage = CultureInfo.CurrentCulture.Name;
        var nextLanguage = currentLanguage.StartsWith("nl") ? "en-US" : "nl-NL";

        var culture = new CultureInfo(nextLanguage);
        culture.ApplyCulture(); // This calls LocalizedStrings.Default.Refresh() automatically

        _settingsService.LocalSettings.Language = nextLanguage;
        _settingsService.SaveLocalSettings();

        await _dialogService.ShowInformationAsync(
            $"Language changed to: {culture.DisplayName}");
    }

    public override Task ShellLoadedAsync() => Task.CompletedTask;

    public override Task InitializeFirstRunAsync() => Task.CompletedTask;

    protected override Task SignOutAsync() => Task.CompletedTask;
}

