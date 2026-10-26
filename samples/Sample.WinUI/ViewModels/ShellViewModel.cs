using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    private readonly IFileService<FileResult> _fileService;
    private readonly IBladeService _bladeService;

    /// <summary>
    /// Gets the glyph of the theme toggle: moon while the light theme is active, sun while the dark
    /// theme is active (same mapping as Sample.Maui and Sample.WPF).
    /// </summary>
    public string ThemeIcon
    {
        get => GetValue<string>() ?? string.Empty;
        private set => SetValue(value);
    }

    public AsyncRelayCommand DisplayCommand { get; private set; }
    public AsyncRelayCommand InfoCommand { get; private set; }
    public AsyncRelayCommand BrowseCommand { get; private set; }
    public AsyncRelayCommand ConverterCommand { get; private set; }
    public AsyncRelayCommand SelectionTestCommand { get; private set; }
    public AsyncRelayCommand ListViewTestCommand { get; private set; }
    public AsyncRelayCommand ValidationTestCommand { get; private set; }
    public AsyncRelayCommand TreeNodeTestCommand { get; private set; }
    public AsyncRelayCommand ChartCommand { get; private set; }

    /// <summary>
    /// Gets the version text shown in the sidebar footer.
    /// </summary>
    public string VersionText => $"v{_commonServices.InfoService.ProductVersion}";

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="dialogService"></param>
    /// <param name="navigationService"></param>
    /// <param name="settingsService"></param>
    /// <param name="themeService">The theme service used to toggle light/dark.</param>
    /// <param name="fileService"></param>
    /// <param name="bladeService"></param>
    /// <param name="logger"></param>
    public ShellViewModel(
        ICommonServices commonServices,
        IDialogService dialogService,
        INavigationService navigationService,
        ISettingsService settingsService,
        IThemeService themeService,
        IFileService<FileResult> fileService,
        IBladeService bladeService,
        ILogger<ShellViewModel> logger)
        : base(commonServices, dialogService, navigationService, settingsService, themeService, logger)
    {
        SetClock();

        _fileService = fileService;
        _bladeService = bladeService;

        ThemeIcon = GetThemeIcon(_themeService.Theme);
        _themeService.ThemeChanged += OnThemeChanged;

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
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

        if (Application.Services.GetRequiredService<IContext>().IsAuthenticated && PrimaryItems?.Count > 0)
        {
            if (PrimaryItems[0].Command!.CanExecute(PrimaryItems[0].CommandParameter))
                PrimaryItems[0].Command!.Execute(PrimaryItems[0].CommandParameter);

            SetSelectedItem(PrimaryItems[0]);
        }

        await InitializeFirstRunAsync();
    }

    private void PopulateNavigationMenuItems()
    {
        if (Application.Services.GetRequiredService<IContext>().IsAuthenticated)
        {
            PrimaryItems?.Clear();
            PrimaryItems?.Add(new NavigationItem("Info", Application.Current.Resources["info"], _settingsService.LocalSettings.Color, InfoCommand));
            PrimaryItems?.Add(new NavigationItem("SlideShow", Application.Current.Resources["kiosk"], _settingsService.LocalSettings.Color, DisplayCommand));
            PrimaryItems?.Add(new NavigationItem("Browse", Application.Current.Resources["search"], _settingsService.LocalSettings.Color, BrowseCommand));
            PrimaryItems?.Add(new NavigationItem("Converters", Application.Current.Resources["products"], _settingsService.LocalSettings.Color, ConverterCommand));
            PrimaryItems?.Add(new NavigationItem("Selection", Application.Current.Resources["multiselect"], _settingsService.LocalSettings.Color, SelectionTestCommand));
            PrimaryItems?.Add(new NavigationItem("ListView", Application.Current.Resources["products"], _settingsService.LocalSettings.Color, ListViewTestCommand));
            PrimaryItems?.Add(new NavigationItem("Validation", Application.Current.Resources["Validation"], _settingsService.LocalSettings.Color, ValidationTestCommand));
            PrimaryItems?.Add(new NavigationItem("TreeView", Application.Current.Resources["TreeView"], _settingsService.LocalSettings.Color, TreeNodeTestCommand));
            PrimaryItems?.Add(new NavigationItem("Charts", Application.Current.Resources["chart"], _settingsService.LocalSettings.Color, ChartCommand));

            SecondaryItems?.Clear();
        }

    }

    protected override Task SignOutAsync() =>
        Application.Services.GetRequiredService<Sample.Abstractions.Services.IAuthenticationService>().SignOutAsync();

    public override async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings is { IsFirstRun: true } globalSettings)
        {
            // The settings blade is opened from the header; do not force it on first run.
            globalSettings.IsFirstRun = false;
            await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSettings);
        }
    }

    private void ClockTimerCallBack(object? sender, object e) => SetClock();

    private void SetClock()
    {
        if (Application.Services.GetRequiredService<IContext>() is not null)
            base.Title = $"{_commonServices.InfoService.ProductName} v{_commonServices.InfoService.ProductVersion} ({Environment.GetEnvironmentVariable(nameof(Environment))}) - {DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";
    }

    private Task OpenChartTestAsync() =>
        _navigationService.NavigateAsync<ChartsViewModel>();

    private Task OpenTreenNodeTestAsync() =>
        _navigationService.NavigateAsync<TreeNodeViewModel, ITreeNodeView>();

    /// <summary>
    /// Opens the validation test asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenValidationTestAsync() =>
        _navigationService.NavigateAsync<ValidationViewModel>();

    /// <summary>
    /// Opens the ListView test asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenListViewTestAsync() =>
        _navigationService.NavigateAsync<TestItemsListViewModel>();

    /// <summary>
    /// Opens the converters asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenConvertersAsync() =>
        _navigationService.NavigateAsync<ConvertersViewModel>();

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
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        _navigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the display asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenDisplayAsync() =>
        _navigationService.NavigateAsync<SlideShowViewModel>();

    /// <summary>
    /// Opens the selection test asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenSelectionTestAsync() =>
       _navigationService.NavigateAsync<SelectionTestViewModel>();

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        OpenSettingsBladeAsync();

    private Task OpenSettingsBladeAsync() =>
        _bladeService.OpenBladeAsync<SettingsViewModel, object>(
            // The shell-level host, so the settings pane spans the whole window (it is rendered by the
            // shell's BladeView rather than by the page that requested it).
            Application.Services.GetRequiredService<BladeHostViewModel>(),
            _ => Task.CompletedTask,
            () => Task.CompletedTask);

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



            _themeService.ThemeChanged -= OnThemeChanged;

            DisplayCommand?.Dispose();
            InfoCommand?.Dispose();
            BrowseCommand?.Dispose();
            ConverterCommand?.Dispose();
            SelectionTestCommand?.Dispose();
            ListViewTestCommand?.Dispose();
            ValidationTestCommand?.Dispose();
            TreeNodeTestCommand?.Dispose();
            ChartCommand?.Dispose();

            base.Dispose(disposing);
        }
    }

    private static string GetThemeIcon(Themes theme) =>
        theme == Themes.Light ? "\uEE45" : "\uEE4D";

    private void OnThemeChanged(object? sender, EventArgs e) =>
        ThemeIcon = GetThemeIcon(_themeService.Theme);
}

