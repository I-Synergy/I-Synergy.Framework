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

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly ICommonServices _commonServices;
    private readonly DispatcherTimer _clockTimer;

    public AsyncRelayCommand InitializeFirstRunCommand { get; private set; }
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
    /// <param name="themeService">The theme selector service.</param>
    public ShellViewModel(
        IContext context,
        ICommonServices commonServices,
        INavigationService navigationService,
        ISettingsService settingsService,
        IAuthenticationService authenticationService,
        ILogger logger,
        IThemeService themeService)
        : base(context, commonServices, settingsService, authenticationService, logger, themeService)
    {
        _commonServices = commonServices;

        SetClock();

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _clockTimer.Tick += ClockTimerCallBack;
        _clockTimer.Start();

        InitializeFirstRunCommand = new AsyncRelayCommand(InitializeFirstRunAsync);
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

        MessageService.Default.Register<ShellLoadedMessage>(this, (m) =>
        {
            if (context.IsAuthenticated && PrimaryItems?.Count > 0)
            {
                if (PrimaryItems[0].Command.CanExecute(PrimaryItems[0].CommandParameter))
                    PrimaryItems[0].Command.Execute(PrimaryItems[0].CommandParameter);

                SelectedItem = PrimaryItems[0];
            }
        });
    }

    private void PopulateNavigationMenuItems()
    {
        if (Context.IsAuthenticated)
        {
            PrimaryItems.Clear();
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["info"] as string, _themeService.Style.Color, InfoCommand));
            PrimaryItems.Add(new NavigationItem("SlideShow", Application.Current.Resources["kiosk"] as string, _themeService.Style.Color, DisplayCommand));
            PrimaryItems.Add(new NavigationItem("Browse", Application.Current.Resources["search"] as string, _themeService.Style.Color, BrowseCommand));
            PrimaryItems.Add(new NavigationItem("Converters", Application.Current.Resources["products"] as string, _themeService.Style.Color, ConverterCommand));
            PrimaryItems.Add(new NavigationItem("Selection", Application.Current.Resources["multiselect"] as string, _themeService.Style.Color, SelectionTestCommand));
            PrimaryItems.Add(new NavigationItem("ListView", Application.Current.Resources["products"] as string, _themeService.Style.Color, ListViewTestCommand));
            PrimaryItems.Add(new NavigationItem("Validation", Application.Current.Resources["Validation"] as string, _themeService.Style.Color, ValidationTestCommand));
            PrimaryItems.Add(new NavigationItem("TreeView", Application.Current.Resources["TreeView"] as string, _themeService.Style.Color, TreeNodeTestCommand));
            PrimaryItems.Add(new NavigationItem("Charts", Application.Current.Resources["chart"] as string, _themeService.Style.Color, ChartCommand));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"] as string, _themeService.Style.Color, HelpCommand));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["flag"] as string, _themeService.Style.Color, LanguageCommand));
            SecondaryItems.Add(new NavigationItem("Theme", Application.Current.Resources["color"] as string, _themeService.Style.Color, ColorCommand));

            if (Context.IsAuthenticated && Context.Profile is Profile)
                SecondaryItems.Add(new NavigationItem("Settings", Application.Current.Resources["settings"] as string, _themeService.Style.Color, SettingsCommand));
        }

        SecondaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["user2"] as string, _themeService.Style.Color, SignInCommand));
    }

    protected override async Task SignOutAsync()
    {
        try
        {
            if (!Context?.IsDisposed ?? false)
            {
                // Clear profile before base sign out
                await base.SignOutAsync();

                if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser))
                {
                    _settingsService.LocalSettings.IsAutoLogin = false;
                    _settingsService.SaveLocalSettings();
                }
            }
        }
        catch (ObjectDisposedException)
        {
            // Context already disposed, nothing to sign out
        }
    }

    private async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings.IsFirstRun)
        {
            if (await _commonServices.DialogService.ShowMessageAsync(
                _commonServices.LanguageService.GetString("ChangeLanguage"),
                _commonServices.LanguageService.GetString("Language"),
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var languageVM = new LanguageViewModel(Context, BaseCommonServices, Logger, _settingsService.LocalSettings.Language);
                languageVM.Submitted += (s, e) =>
                {
                    _settingsService.LocalSettings.Language = e.Result;
                    _settingsService.SaveLocalSettings();
                    e.Result.SetLocalizationLanguage(Context);
                    _commonServices.RestartApplication();
                };
                await _commonServices.DialogService.ShowDialogAsync(typeof(ILanguageWindow), languageVM);
            }

            var wizardVM = new SettingsViewModel(Context, _commonServices, _settingsService, Logger);
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

    private void SetClock() => Title = $"{BaseCommonServices.InfoService.Title} - {DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";

    private Task OpenChartTestAsync() =>
        BaseCommonServices.NavigationService.NavigateAsync<ChartsViewModel>();

    private Task OpenTreenNodeTestAsync() =>
        BaseCommonServices.NavigationService.NavigateAsync<TreeNodeViewModel, ITreeNodeView>();

    /// <summary>
    /// Opens the validation test asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenValidationTestAsync() =>
        BaseCommonServices.NavigationService.NavigateAsync<ValidationViewModel>();

    /// <summary>
    /// Opens the ListView test asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenListViewTestAsync() =>
        BaseCommonServices.NavigationService.NavigateAsync<TestItemsListViewModel>();

    /// <summary>
    /// Opens the converters asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenConvertersAsync() =>
        BaseCommonServices.NavigationService.NavigateAsync<ConvertersViewModel>();

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
        BaseCommonServices.NavigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the display asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenDisplayAsync() =>
        BaseCommonServices.NavigationService.NavigateAsync<SlideShowViewModel>();

    /// <summary>
    /// Opens the selection test asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenSelectionTestAsync() =>
       BaseCommonServices.NavigationService.NavigateAsync<SelectionTestViewModel>();

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        BaseCommonServices.NavigationService.NavigateModalAsync<SettingsViewModel>();

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
