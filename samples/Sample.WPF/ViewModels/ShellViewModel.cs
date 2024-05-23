using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using NugetUnlister.ViewModels;
using Sample.Abstractions;
using Sample.Models;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

/// <summary>
/// Class ShellViewModel.
/// </summary>
public class ShellViewModel : BaseShellViewModel, IShellViewModel
{
    private readonly IToastMessageService _toastMessageService;

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
    public ObservableCollection<TestItem> SelectedTestItems { get; set; }

    /// <summary>
    /// Gets the common services.
    /// </summary>
    /// <value>The common services.</value>
    public ICommonServices CommonServices { get; }
    public IApplicationSettingsService SettingsService { get; }

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

    public AsyncRelayCommand EditableComboCommand { get; private set; }

    /// <summary>
    /// gets or sets the Unit Conversion command.
    /// </summary>
    public AsyncRelayCommand UnitConversionCommand { get; private set; }

    /// <summary>
    /// Gets or sets the Validation test command.
    /// </summary>
    public AsyncRelayCommand ValidationTestCommand { get; private set; }

    public AsyncRelayCommand NugetUnlisterCommand { get; private set; }
    public AsyncRelayCommand SelectSingleCommand { get; private set; }
    public AsyncRelayCommand SelectMultipleCommand { get; private set; }
    public AsyncRelayCommand ShowToastMessageCommand { get; private set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="commonServices">The common services.</param>
    /// <param name="settingsService">The settings services.</param>
    /// <param name="authenticationService"></param>
    /// <param name="toastMessageService"></param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="themeService">The theme selector service.</param>
    /// <param name="localizationService">The localization functions.</param>
    public ShellViewModel(
        IContext context,
        ICommonServices commonServices,
        IApplicationSettingsService settingsService,
        IAuthenticationService authenticationService,
        IToastMessageService toastMessageService,
        ILogger logger,
        IThemeService themeService,
        ILocalizationService localizationService)
        : base(context, commonServices, settingsService, authenticationService, logger, themeService, localizationService)
    {
        CommonServices = commonServices;
        SettingsService = settingsService;

        _toastMessageService = toastMessageService;

        Title = commonServices.InfoService.ProductName;
        Version = commonServices.InfoService.ProductVersion;

        InfoCommand = new AsyncRelayCommand(OpenInfoAsync);
        BrowseCommand = new AsyncRelayCommand(BrowseFileAsync);
        EditableComboCommand = new AsyncRelayCommand(OpenEditableComboAsync);
        ValidationTestCommand = new AsyncRelayCommand(OpenValidationTestAsync);
        UnitConversionCommand = new AsyncRelayCommand(OpenUnitConversionAsync);
        NugetUnlisterCommand = new AsyncRelayCommand(UnlistNugetAsync);
        SelectSingleCommand = new AsyncRelayCommand(SelectSingleAsync);
        SelectMultipleCommand = new AsyncRelayCommand(SelectMultipleAsync);
        ShowToastMessageCommand = new AsyncRelayCommand(ShowToastMessageAsync);

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
        PrimaryItems.Clear();

        if (Context.IsAuthenticated)
        {
            PrimaryItems.Add(new NavigationItem("Info", (Application.Current.Resources["info"] as string).ToPath(), _themeService.Style.Color, InfoCommand));
            PrimaryItems.Add(new NavigationItem("Browse", (Application.Current.Resources["search"] as string).ToPath(), _themeService.Style.Color, BrowseCommand));
            PrimaryItems.Add(new NavigationItem("Editable Combobox", (Application.Current.Resources["combobox"] as string).ToPath(), _themeService.Style.Color, EditableComboCommand));
            PrimaryItems.Add(new NavigationItem("Validation", (Application.Current.Resources["validation"] as string).ToPath(), _themeService.Style.Color, ValidationTestCommand));
            PrimaryItems.Add(new NavigationItem("Unit Conversion", (Application.Current.Resources["weight"] as string).ToPath(), _themeService.Style.Color, UnitConversionCommand));
            PrimaryItems.Add(new NavigationItem("Select single item", (Application.Current.Resources["info"] as string).ToPath(), _themeService.Style.Color, SelectSingleCommand));
            PrimaryItems.Add(new NavigationItem("Select multiple items", (Application.Current.Resources["info"] as string).ToPath(), _themeService.Style.Color, SelectMultipleCommand));
            PrimaryItems.Add(new NavigationItem("Nuget Unlister", (Application.Current.Resources["info"] as string).ToPath(), _themeService.Style.Color, NugetUnlisterCommand));
            PrimaryItems.Add(new NavigationItem("Show toast message", (Application.Current.Resources["info"] as string).ToPath(), _themeService.Style.Color, ShowToastMessageCommand));
        }

        PrimaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", (Application.Current.Resources["user2"] as string).ToPath(), _themeService.Style.Color, SignInCommand));
    }

    private Task ShowToastMessageAsync()
    {
        _toastMessageService.ShowInformation("This is an informational message!");
        _toastMessageService.ShowSuccess("This is a success message!");
        _toastMessageService.ShowError("This is an error message!");
        _toastMessageService.ShowWarning("This is a warning message!");
        return Task.CompletedTask;
    }

    private Task SelectSingleAsync()
    {
        ViewModelSelectionDialog<TestItem> selectionVm = new ViewModelSelectionDialog<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Single);
        selectionVm.Submitted += SelectionVm_SingleSubmitted;
        return BaseCommonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
    }

    private async void SelectionVm_SingleSubmitted(object sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_SingleSubmitted;

        await BaseCommonServices.DialogService.ShowInformationAsync($"{e.Result.Single().Description} selected.");
    }

    private Task SelectMultipleAsync()
    {
        ViewModelSelectionDialog<TestItem> selectionVm = new ViewModelSelectionDialog<TestItem>(Context, BaseCommonServices, Logger, Items, SelectedTestItems, SelectionModes.Multiple);
        selectionVm.Submitted += SelectionVm_MultipleSubmitted;
        return BaseCommonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVm);
    }

    private async void SelectionVm_MultipleSubmitted(object sender, SubmitEventArgs<List<TestItem>> e)
    {
        if (sender is ViewModelSelectionDialog<TestItem> vm)
            vm.Submitted -= SelectionVm_MultipleSubmitted;

        SelectedTestItems = new ObservableCollection<TestItem>();
        SelectedTestItems.AddRange(e.Result);

        await BaseCommonServices.DialogService.ShowInformationAsync($"{string.Join(", ", e.Result.Select(s => s.Description))} selected.");
    }

    private Task UnlistNugetAsync() =>
        CommonServices.NavigationService.NavigateAsync<NugetViewModel>();

    /// <summary>
    /// Opens the Unit conversion view asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenUnitConversionAsync() =>
        CommonServices.NavigationService.NavigateAsync<UnitConversionViewModel>();


    /// <summary>
    /// Opens the validation test asynchronous.
    /// </summary>
    /// <returns></returns>
    private Task OpenValidationTestAsync() =>
        CommonServices.NavigationService.NavigateAsync<ValidationViewModel>();

    /// <summary>
    /// Opens the information asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    private Task OpenInfoAsync() =>
        CommonServices.NavigationService.NavigateAsync<InfoViewModel>();

    /// <summary>
    /// Opens the editable combobox sample.
    /// </summary>
    /// <returns></returns>
    private Task OpenEditableComboAsync() =>
        CommonServices.NavigationService.NavigateAsync<EditableComboViewModel>();

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
    /// Opens the settings asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    protected override Task OpenSettingsAsync() => throw new NotImplementedException();
}
