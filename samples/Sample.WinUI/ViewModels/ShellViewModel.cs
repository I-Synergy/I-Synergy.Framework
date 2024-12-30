using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.ViewModels;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Sample.Abstractions;
using Sample.Messages;
using Sample.Services;

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly DispatcherTimer _clockTimer;

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
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="navigationService"></param>
    /// <param name="settingsService">The settings services.</param>
    /// <param name="authenticationService"></param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel(
        IContext context,
        ICommonServices commonServices,
        INavigationService navigationService,
        ISettingsService settingsService,
        IAuthenticationService authenticationService,
        ILogger<ShellViewModel> logger)
        : base(context, commonServices, settingsService, authenticationService, logger)
    {
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

        MessageService.Default.Register<ShellLoadedMessage>(this, async (m) =>
        {
            if (context.IsAuthenticated && PrimaryItems?.Count > 0)
            {
                if (PrimaryItems[0].Command.CanExecute(PrimaryItems[0].CommandParameter))
                    PrimaryItems[0].Command.Execute(PrimaryItems[0].CommandParameter);

                SelectedItem = PrimaryItems[0];
            }

            await InitializeFirstRunAsync();
        });
    }

    private void PopulateNavigationMenuItems()
    {
        if (_context.IsAuthenticated)
        {
            PrimaryItems.Clear();
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["info"] as string, _settingsService.LocalSettings.Color, InfoCommand));
            PrimaryItems.Add(new NavigationItem("SlideShow", Application.Current.Resources["kiosk"] as string, _settingsService.LocalSettings.Color, DisplayCommand));
            PrimaryItems.Add(new NavigationItem("Browse", Application.Current.Resources["search"] as string, _settingsService.LocalSettings.Color, BrowseCommand));
            PrimaryItems.Add(new NavigationItem("Converters", Application.Current.Resources["products"] as string, _settingsService.LocalSettings.Color, ConverterCommand));
            PrimaryItems.Add(new NavigationItem("Selection", Application.Current.Resources["multiselect"] as string, _settingsService.LocalSettings.Color, SelectionTestCommand));
            PrimaryItems.Add(new NavigationItem("ListView", Application.Current.Resources["products"] as string, _settingsService.LocalSettings.Color, ListViewTestCommand));
            PrimaryItems.Add(new NavigationItem("Validation", Application.Current.Resources["Validation"] as string, _settingsService.LocalSettings.Color, ValidationTestCommand));
            PrimaryItems.Add(new NavigationItem("TreeView", Application.Current.Resources["TreeView"] as string, _settingsService.LocalSettings.Color, TreeNodeTestCommand));
            PrimaryItems.Add(new NavigationItem("Charts", Application.Current.Resources["chart"] as string, _settingsService.LocalSettings.Color, ChartCommand));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"] as string, _settingsService.LocalSettings.Color, HelpCommand));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["flag"] as string, _settingsService.LocalSettings.Color, LanguageCommand));
            SecondaryItems.Add(new NavigationItem("Theme", Application.Current.Resources["color"] as string, _settingsService.LocalSettings.Color, ColorCommand));

            if (_context.IsAuthenticated && _context.Profile is Profile)
                SecondaryItems.Add(new NavigationItem("Settings", Application.Current.Resources["settings"] as string, _settingsService.LocalSettings.Color, SettingsCommand));
        }

        SecondaryItems.Add(new NavigationItem(_context.IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["user2"] as string, _settingsService.LocalSettings.Color, SignInCommand));
    }

    protected override async Task SignOutAsync()
    {
        try
        {
            // Clear profile before base sign out
            await base.SignOutAsync();

            if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser))
            {
                _settingsService.LocalSettings.IsAutoLogin = false;
                _settingsService.SaveLocalSettings();
            }
        }
        catch (ObjectDisposedException)
        {
            // Context already disposed, nothing to sign out
        }
    }

    private async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings.IsFirstRun && _commonServices is CommonServices commonServices)
        {
            if (await _commonServices.DialogService.ShowMessageAsync(
                LanguageService.Default.GetString("ChangeLanguage"),
                LanguageService.Default.GetString("Language"),
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var languageVM = new LanguageViewModel(_context, base._commonServices, _logger, _settingsService.LocalSettings.Language);
                languageVM.Submitted += (s, e) =>
                {
                    _settingsService.LocalSettings.Language = e.Result;
                    _settingsService.SaveLocalSettings();
                    e.Result.SetLocalizationLanguage(_context);
                    _commonServices.RestartApplication();
                };
                await _commonServices.DialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
            }

            var wizardVM = new SettingsViewModel(_context, commonServices, _settingsService, _logger);
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

    private void ClockTimerCallBack(object sender, object e) => SetClock();

    private void SetClock() => base.Title = $"{InfoService.Default.Title} - {DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";

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

        if (_commonServices is CommonServices commonServices &&
            await commonServices.FileService.BrowseFileAsync(imageFilter) is { } files && files.Count > 0)
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clockTimer.Stop();
            _clockTimer.Tick -= ClockTimerCallBack;

            (DisplayCommand as IDisposable)?.Dispose();
            (InfoCommand as IDisposable)?.Dispose();
            (BrowseCommand as IDisposable)?.Dispose();
            (ConverterCommand as IDisposable)?.Dispose();
            (SelectionTestCommand as IDisposable)?.Dispose();
            (ListViewTestCommand as IDisposable)?.Dispose();
            (ValidationTestCommand as IDisposable)?.Dispose();
            (TreeNodeTestCommand as IDisposable)?.Dispose();
            (ChartCommand as IDisposable)?.Dispose();

            MessageService.Default.Unregister<ShellLoadedMessage>(this);

            base.Dispose(disposing);
        }
    }
}
