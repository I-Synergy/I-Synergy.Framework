using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NugetUnlister.ViewModels;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel
{
    private readonly IFileService<FileResult> _fileService;
    private readonly IBladeService _bladeService;
    private readonly BladeHostViewModel _bladeHost;

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

    /// <summary>
    /// Gets the glyph of the theme toggle: moon while the light theme is active, sun while the dark
    /// theme is active (the same mapping as Sample.Maui).
    /// </summary>
    public string ThemeIcon
    {
        get => GetValue<string>() ?? string.Empty;
        private set => SetValue(value);
    }

    public AsyncRelayCommand InfoCommand { get; private set; }
    public AsyncRelayCommand BrowseCommand { get; private set; }

    public AsyncRelayCommand EditableComboCommand { get; private set; }
    public AsyncRelayCommand UnitConversionCommand { get; private set; }
    public AsyncRelayCommand ValidationTestCommand { get; private set; }
    public AsyncRelayCommand NugetUnlisterCommand { get; private set; }
    public AsyncRelayCommand DetailsCommand { get; private set; }
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
    /// <param name="bladeService">The blade service that opens blades into the shell-owned pane.</param>
    /// <param name="bladeHost">The shell-level owner of the blade pane.</param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        ILogger<ShellViewModel> logger,
        IFileService<FileResult> fileService,
        IThemeService themeService,
        IBladeService bladeService,
        BladeHostViewModel bladeHost)
        : base(commonServices, dialogService, navigationService, settingsService, themeService, logger)
    {
        _fileService = fileService;
        _bladeService = bladeService;
        _bladeHost = bladeHost;

        Title = _commonServices.InfoService.ProductName;
        Version = _commonServices.InfoService.ProductVersion;
        ThemeIcon = GetThemeIcon(_themeService.Theme);
        _themeService.ThemeChanged += OnThemeChanged;

        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        BrowseCommand = new AsyncRelayCommand(BrowseFileAsync);
        EditableComboCommand = new AsyncRelayCommand(OpenEditableComboAsync);
        ValidationTestCommand = new AsyncRelayCommand(OpenValidationTestAsync);
        UnitConversionCommand = new AsyncRelayCommand(OpenUnitConversionAsync);
        NugetUnlisterCommand = new AsyncRelayCommand(UnlistNugetAsync);
        DetailsCommand = new AsyncRelayCommand(OpenDetailsAsync);
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
            PrimaryItems?.Add(new NavigationItem("Blades", Application.Current.Resources["info"], _settingsService.LocalSettings.Color, DetailsCommand));
        }


        if (PrimaryItems?.FirstOrDefault() is { } firstItem)
            SelectedItem = firstItem;
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
    /// Opens the blade sample page, which hosts the blade pane inside the page itself.
    /// </summary>
    /// <returns></returns>
    private Task OpenDetailsAsync() =>
        _navigationService.NavigateAsync<DetailsViewModel>();

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
    /// Opens the settings blade, where language, theme and accent color are applied live.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        _bladeService.OpenBladeAsync<SettingsViewModel, object>(_bladeHost);

    public override Task ShellLoadedAsync() => Task.CompletedTask;

    public override Task InitializeFirstRunAsync() => Task.CompletedTask;

    protected override Task SignOutAsync() => Task.CompletedTask;

    private static string GetThemeIcon(Themes theme) =>
        theme == Themes.Light ? "\uEE45" : "\uEE4D";

    private void OnThemeChanged(object? sender, EventArgs e) =>
        ThemeIcon = GetThemeIcon(_themeService.Theme);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _themeService.ThemeChanged -= OnThemeChanged;

        base.Dispose(disposing);
    }
}

