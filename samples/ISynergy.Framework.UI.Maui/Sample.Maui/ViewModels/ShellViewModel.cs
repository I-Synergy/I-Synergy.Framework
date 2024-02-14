using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using FileResult = ISynergy.Framework.Mvvm.Models.FileResult;

namespace Sample.ViewModels;

internal class ShellViewModel : BaseShellViewModel, IShellViewModel
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
    /// Gets or sets the IsBackEnabled property value.
    /// </summary>
    public bool IsBackEnabled
    {
        get => CommonServices.NavigationService.CanGoBack;
    }

    /// <summary>
    /// Gets the common services.
    /// </summary>
    /// <value>The common services.</value>
    public ICommonServices CommonServices { get; }

    /// <summary>
    /// Gets the settings service.
    /// </summary>
    public IBaseApplicationSettingsService SettingsService { get; }

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
    /// <param name="settingsService">The settings services.</param>
    /// <param name="authenticationService"></param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="themeService">The theme selector service.</param>
    /// <param name="localizationService">The localization functions.</param>
    public ShellViewModel(
        IContext context,
        ICommonServices commonServices,
        IBaseApplicationSettingsService settingsService,
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
        //ConverterCommand = new AsyncRelayCommand(OpenConvertersAsync);
        //SelectionTestCommand = new AsyncRelayCommand(OpenSelectionTestAsync);
        //ListViewTestCommand = new AsyncRelayCommand(OpenListViewTestAsync);
        //ValidationTestCommand = new AsyncRelayCommand(OpenValidationTestAsync);
        //TreeNodeTestCommand = new AsyncRelayCommand(OpenTreenNodeTestAsync);
        //ChartCommand = new AsyncRelayCommand(OpenChartTestAsync);
    }

    protected override async Task SignOutAsync()
    {
        await base.SignOutAsync();

        if (!string.IsNullOrEmpty(_applicationSettingsService.Settings.DefaultUser))
        {
            _applicationSettingsService.Settings.IsAutoLogin = false;
            _applicationSettingsService.SaveSettings();
        }
    }


    //private Task OpenChartTestAsync() =>
    //    CommonServices.NavigationService.NavigateAsync<ChartsViewModel>();

    //private Task OpenTreenNodeTestAsync() =>
    //    CommonServices.NavigationService.NavigateAsync<TreeNodeViewModel, ITreeNodeView>();

    ///// <summary>
    ///// Opens the validation test asynchronous.
    ///// </summary>
    ///// <returns></returns>
    //private Task OpenValidationTestAsync() =>
    //    CommonServices.NavigationService.NavigateAsync<ValidationViewModel>();

    ///// <summary>
    ///// Opens the ListView test asynchronous.
    ///// </summary>
    ///// <returns>Task.</returns>
    //private Task OpenListViewTestAsync() =>
    //    CommonServices.NavigationService.NavigateAsync<TestItemsListViewModel>();

    ///// <summary>
    ///// Opens the converters asynchronous.
    ///// </summary>
    ///// <returns>Task.</returns>
    //private Task OpenConvertersAsync() =>
    //    CommonServices.NavigationService.NavigateAsync<ConvertersViewModel>();

    /// <summary>
    /// browse file as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task BrowseFileAsync()
    {
        string imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

        if (await CommonServices.FileService.BrowseFileAsync(imageFilter) is List<FileResult> files && files.Count > 0)
            await CommonServices.DialogService.ShowInformationAsync($"File '{files.First().FileName}' is selected.");
    }

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        CommonServices.NavigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the display asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenDisplayAsync() =>
        CommonServices.NavigationService.NavigateAsync<SlideShowViewModel>();

    ///// <summary>
    ///// Opens the selection test asynchronous.
    ///// </summary>
    ///// <returns>Task.</returns>
    //private Task OpenSelectionTestAsync() =>
    //   CommonServices.NavigationService.NavigateAsync<SelectionTestViewModel>();

    /// <summary>
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() =>
        BaseCommonServices.NavigationService.NavigateModalAsync<SettingsViewModel>();

    /// <summary>
    /// Restarts the application asynchronous.
    /// </summary>
    /// <returns></returns>
    protected override Task RestartApplicationAsync()
    {
        Application.Current.Quit();
        return Task.CompletedTask;
    }
}
