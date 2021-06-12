using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Navigation;
using Sample.Abstractions.Services;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
        public string Version
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets the common services.
        /// </summary>
        /// <value>The common services.</value>
        public ICommonServices CommonServices { get; }

        /// <summary>
        /// Gets or sets the display command.
        /// </summary>
        /// <value>The display command.</value>
        public Command Display_Command { get; set; }
        /// <summary>
        /// Gets or sets the information command.
        /// </summary>
        /// <value>The information command.</value>
        public Command Info_Command { get; set; }
        /// <summary>
        /// Gets or sets the browse command.
        /// </summary>
        /// <value>The browse command.</value>
        public Command Browse_Command { get; set; }
        /// <summary>
        /// Gets or sets the converter command.
        /// </summary>
        /// <value>The converter command.</value>
        public Command Converter_Command { get; set; }
        /// <summary>
        /// Gets or sets the selection test command.
        /// </summary>
        /// <value>The selection test command.</value>
        public Command SelectionTest_Command { get; set; }
        /// <summary>
        /// Gets or sets the ListView test command.
        /// </summary>
        /// <value>The ListView test command.</value>
        public Command ListViewTest_Command { get; set; }

        /// <summary>
        /// Gets or sets the Validation test command.
        /// </summary>
        public Command ValidationTest_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="settingsService">The settings services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="themeSelectorService">The theme selector service.</param>
        /// <param name="localizationFunctions">The localization functions.</param>
        public ShellViewModel(
            IContext context,
            ICommonServices commonServices,
            ISettingsService settingsService,
            ILoggerFactory loggerFactory,
            IThemeSelectorService themeSelectorService,
            LocalizationFunctions localizationFunctions)
            : base(context, commonServices, settingsService, loggerFactory, themeSelectorService, localizationFunctions)
        {
            CommonServices = commonServices;

            Version = commonServices.InfoService.ProductVersion;
            DisplayName = "User";

            Display_Command = new Command(async () => await OpenDisplayAsync());
            Info_Command = new Command(async () => await OpenInfoAsync());
            Browse_Command = new Command(async () => await BrowseFileAsync());
            Converter_Command = new Command(async () => await OpenConvertersAsync());
            SelectionTest_Command = new Command(async () => await OpenSelectionTestAsync());
            ListViewTest_Command = new Command(async () => await OpenListViewTestAsync());
            ValidationTest_Command = new Command(async () => await OpenValidationTestAsync());

            PopulateNavItems();
        }

        /// <summary>
        /// Opens the validation test asynchronous.
        /// </summary>
        /// <returns></returns>
        private Task OpenValidationTestAsync() =>
            CommonServices.NavigationService.NavigateAsync<ValidationViewModel>();

        /// <summary>
        /// Opens the ListView test asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task OpenListViewTestAsync() =>
            CommonServices.NavigationService.NavigateAsync<TestItemsListViewModel>();

        /// <summary>
        /// Opens the converters asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task OpenConvertersAsync() =>
            CommonServices.NavigationService.NavigateAsync<ConvertersViewModel>();

        /// <summary>
        /// browse file as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task BrowseFileAsync()
        {
            var imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

            if (await CommonServices.FileService.BrowseFileAsync(imageFilter, 0) is FileResult file)
                await CommonServices.DialogService.ShowInformationAsync($"File '{file.FileName}' is selected.");
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

        /// <summary>
        /// Opens the selection test asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task OpenSelectionTestAsync() =>
           CommonServices.NavigationService.NavigateAsync<SelectionTestViewModel>();

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
        protected override Task CreateFeedbackAsync() => ThrowFeatureNotEnabledWarning();

        /// <summary>
        /// Opens the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected override Task OpenSettingsAsync() => ThrowFeatureNotEnabledWarning();

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        public override Task InitializeAsync(object parameter)
        {
            CommonServices.NavigationService.Frame = parameter;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Processes the authentication request asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public override Task ProcessAuthenticationRequestAsync()
        {
            return Task.CompletedTask;
        }
    }
}
