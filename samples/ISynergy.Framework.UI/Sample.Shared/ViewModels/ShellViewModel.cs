using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Navigation;
using Sample.Abstractions.Services;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using CommunityToolkit.Mvvm.Input;

#if WINDOWS_UWP || HAS_UNO
using Windows.UI.Xaml;
#endif

using Microsoft.UI.Xaml;

namespace Sample.ViewModels
{
    /// <summary>
    /// Class ShellViewModel.
    /// </summary>
    public class ShellViewModel : ShellViewModelBase, IShellViewModel
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
        public IBaseApplicationSettingsService SettingsService { get; }

        /// <summary>
        /// Gets or sets the display command.
        /// </summary>
        /// <value>The display command.</value>
        public RelayCommand Display_Command { get; set; }
        
        /// <summary>
        /// Gets or sets the information command.
        /// </summary>
        /// <value>The information command.</value>
        public RelayCommand Info_Command { get; set; }

        /// <summary>
        /// Gets or sets the browse command.
        /// </summary>
        /// <value>The browse command.</value>
        public AsyncRelayCommand Browse_Command { get; set; }

        /// <summary>
        /// Gets or sets the converter command.
        /// </summary>
        /// <value>The converter command.</value>
        public RelayCommand Converter_Command { get; set; }

        /// <summary>
        /// Gets or sets the selection test command.
        /// </summary>
        /// <value>The selection test command.</value>
        public RelayCommand SelectionTest_Command { get; set; }

        /// <summary>
        /// Gets or sets the ListView test command.
        /// </summary>
        /// <value>The ListView test command.</value>
        public RelayCommand ListViewTest_Command { get; set; }

        /// <summary>
        /// Gets or sets the Validation test command.
        /// </summary>
        public RelayCommand ValidationTest_Command { get; set; }

        /// <summary>
        /// Gets or sets the TreeNode test command.
        /// </summary>
        public RelayCommand TreeNodeTest_Command { get; set; }

        public RelayCommand Chart_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="settingsService">The settings services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="themeService">The theme selector service.</param>
        /// <param name="localizationFunctions">The localization functions.</param>
        public ShellViewModel(
            IContext context,
            ICommonServices commonServices,
            IBaseApplicationSettingsService settingsService,
            ILogger logger,
            IThemeService themeService,
            ILocalizationService localizationFunctions)
            : base(context, commonServices, settingsService, logger, themeService, localizationFunctions)
        {
            CommonServices = commonServices;
            SettingsService = settingsService;

            Title = commonServices.InfoService.ProductName;
            Version = commonServices.InfoService.ProductVersion;
            DisplayName = "User";

            Display_Command = new RelayCommand(OpenDisplay);
            Info_Command = new RelayCommand(OpenInfo);
            Browse_Command = new AsyncRelayCommand(async () => await BrowseFileAsync());
            Converter_Command = new RelayCommand(OpenConverters);
            SelectionTest_Command = new RelayCommand(OpenSelectionTest);
            ListViewTest_Command = new RelayCommand(OpenListViewTest);
            ValidationTest_Command = new RelayCommand(OpenValidationTest);
            TreeNodeTest_Command = new RelayCommand(OpenTreenNodeTest);
            Chart_Command = new RelayCommand(OpenChartTest);
            Login_Command = new RelayCommand(PopulateNavItems);

            PopulateNavItems();
        }

        private void OpenChartTest() =>
            CommonServices.NavigationService.Navigate<ChartsViewModel>();

        private void OpenTreenNodeTest() =>
            CommonServices.NavigationService.Navigate<TreeNodeViewModel>();

        /// <summary>
        /// Opens the validation test asynchronous.
        /// </summary>
        /// <returns></returns>
        private void OpenValidationTest() =>
            CommonServices.NavigationService.Navigate<ValidationViewModel>();

        /// <summary>
        /// Opens the ListView test asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private void OpenListViewTest() =>
            CommonServices.NavigationService.Navigate<TestItemsListViewModel>();

        /// <summary>
        /// Opens the converters asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private void OpenConverters() =>
            CommonServices.NavigationService.Navigate<ConvertersViewModel>();

        /// <summary>
        /// browse file as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task BrowseFileAsync()
        {
            var imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

            if (await CommonServices.FileService.BrowseFileAsync(imageFilter) is FileResult file)
                await CommonServices.DialogService.ShowInformationAsync($"File '{file.FileName}' is selected.");
        }

        /// <summary>
        /// Opens the information asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private void OpenInfo() =>
            CommonServices.NavigationService.Navigate<InfoViewModel>();

        /// <summary>
        /// Opens the display asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private void OpenDisplay() =>
            CommonServices.NavigationService.Navigate<SlideShowViewModel>();

        /// <summary>
        /// Opens the selection test asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private void OpenSelectionTest() =>
           CommonServices.NavigationService.Navigate<SelectionTestViewModel>();

        /// <summary>
        /// initialize as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }

        /// <summary>
        /// Populates the nav items.
        /// </summary>
        protected override void PopulateNavItems()
        {
            PrimaryItems.Clear();
            PrimaryItems.Add(new NavigationItem("SlideShow", Application.Current.Resources["kiosk"] as string, ForegroundColor, Display_Command));
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["info"] as string, ForegroundColor, Info_Command));
            PrimaryItems.Add(new NavigationItem("Browse", Application.Current.Resources["search"] as string, ForegroundColor, Browse_Command));
            PrimaryItems.Add(new NavigationItem("Converters", Application.Current.Resources["products"] as string, ForegroundColor, Converter_Command));
            PrimaryItems.Add(new NavigationItem("Selection", Application.Current.Resources["multiselect"] as string, ForegroundColor, SelectionTest_Command));
            PrimaryItems.Add(new NavigationItem("ListView", Application.Current.Resources["products"] as string, ForegroundColor, ListViewTest_Command));
            PrimaryItems.Add(new NavigationItem("Validation", Application.Current.Resources["Validation"] as string, ForegroundColor, ValidationTest_Command));
            PrimaryItems.Add(new NavigationItem("TreeView", Application.Current.Resources["TreeView"] as string, ForegroundColor, TreeNodeTest_Command));
            PrimaryItems.Add(new NavigationItem("Charts", Application.Current.Resources["chart"] as string, ForegroundColor, Chart_Command));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"] as string, ForegroundColor, Help_Command));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["flag"] as string, ForegroundColor, Language_Command));
            SecondaryItems.Add(new NavigationItem("Theme", Application.Current.Resources["color"] as string, ForegroundColor, Color_Command));
            SecondaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["user2"] as string, ForegroundColor, Login_Command));
        }

        /// <summary>
        /// Creates the feedback asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected override Task CreateFeedbackAsync() => 
            ThrowFeatureNotEnabledWarning();

        /// <summary>
        /// Opens the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected override Task OpenSettingsAsync() => 
            ThrowFeatureNotEnabledWarning();

        public override void SetRootFrame(object frame) =>
            CommonServices.NavigationService.Frame = frame;
    }
}
