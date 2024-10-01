using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
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
    /// Gets the common services.
    /// </summary>
    /// <value>The common services.</value>
    public ICommonServices CommonServices { get; }

    public ISettingsService SettingsService { get; }

    /// <summary>
    /// Gets or sets the display command.
    /// </summary>
    /// <value>The display command.</value>
    public AsyncRelayCommand DisplayCommand { get; private set; }

    /// <summary>
    /// Gets or sets the information command.
    /// </summary>
    /// <value>The information command.</value>
    public AsyncRelayCommand InfoCommand { get; private set; }

    /// <summary>
    /// Gets or sets the browse command.
    /// </summary>
    /// <value>The browse command.</value>
    public AsyncRelayCommand BrowseCommand { get; private set; }

    /// <summary>
    /// Gets or sets the converter command.
    /// </summary>
    /// <value>The converter command.</value>
    public AsyncRelayCommand ConverterCommand { get; private set; }

    /// <summary>
    /// Gets or sets the selection test command.
    /// </summary>
    /// <value>The selection test command.</value>
    public AsyncRelayCommand SelectionTestCommand { get; private set; }

    /// <summary>
    /// Gets or sets the ListView test command.
    /// </summary>
    /// <value>The ListView test command.</value>
    public AsyncRelayCommand ListViewTestCommand { get; private set; }

    /// <summary>
    /// Gets or sets the Validation test command.
    /// </summary>
    public AsyncRelayCommand ValidationTestCommand { get; private set; }

    /// <summary>
    /// Gets or sets the TreeNode test command.
    /// </summary>
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
    /// <param name="localizationService">The localization functions.</param>
    public ShellViewModel(
        IContext context,
        ICommonServices commonServices,
        INavigationService navigationService,
        ISettingsService settingsService,
        IAuthenticationService authenticationService,
        ILogger logger,
        IThemeService themeService,
        ILocalizationService localizationService)
        : base(context, commonServices, settingsService, authenticationService, logger, themeService, localizationService)
    {
        CommonServices = commonServices;
        SettingsService = settingsService;

        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

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

    private void PopulateNavigationMenuItems()
    {
        PrimaryItems.Clear();
        SecondaryItems.Clear();

        if (Context.IsAuthenticated)
        {
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["info"] as string, _themeService.Style.Color, InfoCommand));
            PrimaryItems.Add(new NavigationItem("SlideShow", Application.Current.Resources["kiosk"] as string, _themeService.Style.Color, DisplayCommand));
            PrimaryItems.Add(new NavigationItem("Browse", Application.Current.Resources["search"] as string, _themeService.Style.Color, BrowseCommand));
            PrimaryItems.Add(new NavigationItem("Converters", Application.Current.Resources["products"] as string, _themeService.Style.Color, ConverterCommand));
            PrimaryItems.Add(new NavigationItem("Selection", Application.Current.Resources["multiselect"] as string, _themeService.Style.Color, SelectionTestCommand));
            PrimaryItems.Add(new NavigationItem("ListView", Application.Current.Resources["products"] as string, _themeService.Style.Color, ListViewTestCommand));
            PrimaryItems.Add(new NavigationItem("Validation", Application.Current.Resources["Validation"] as string, _themeService.Style.Color, ValidationTestCommand));
            PrimaryItems.Add(new NavigationItem("TreeView", Application.Current.Resources["TreeView"] as string, _themeService.Style.Color, TreeNodeTestCommand));
            PrimaryItems.Add(new NavigationItem("Charts", Application.Current.Resources["chart"] as string, _themeService.Style.Color, ChartCommand));

            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"] as string, _themeService.Style.Color, HelpCommand));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["flag"] as string, _themeService.Style.Color, LanguageCommand));
            SecondaryItems.Add(new NavigationItem("Theme", Application.Current.Resources["color"] as string, _themeService.Style.Color, ColorCommand));
            SecondaryItems.Add(new NavigationItem("Settings", Application.Current.Resources["settings"] as string, _themeService.Style.Color, SettingsCommand));
        }

        SecondaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["user2"] as string, _themeService.Style.Color, SignInCommand));
    }

    protected override async Task SignOutAsync()
    {
        if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser))
        {
            _settingsService.LocalSettings.IsAutoLogin = false;
            _settingsService.SaveLocalSettings();
        }

        await BaseCommonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
    }

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

        if (await CommonServices.FileService.BrowseFileAsync(imageFilter) is { } files && files.Count > 0)
            await CommonServices.DialogService.ShowInformationAsync($"File '{files[0].FileName}' is selected.");
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
}
