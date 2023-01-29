using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Models;
using ISynergy.Framework.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;

namespace Sample.ViewModels
{
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
        public IBaseApplicationSettingsService SettingsService { get; }

        /// <summary>
        /// Gets or sets the information command.
        /// </summary>
        /// <value>The information command.</value>
        public RelayCommand Info_Command { get; set; }

        /// <summary>
        /// Gets or sets the browse command.
        /// </summary>
        /// <value>The browse command.</value>
        public RelayCommand Browse_Command { get; set; }

        public RelayCommand EditableCombo_Command { get; set; }

        /// <summary>
        /// gets or sets the Unit Conversion command.
        /// </summary>
        public RelayCommand UnitConversion_Command { get; set; }

        /// <summary>
        /// Gets or sets the Validation test command.
        /// </summary>
        public RelayCommand ValidationTest_Command { get; set; }

        public RelayCommand StitchImage_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="settingsService">The settings services.</param>
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
            DisplayName = "User";

            Info_Command = new RelayCommand(async () => await OpenInfoAsync());
            Browse_Command = new RelayCommand(async () => await BrowseFileAsync());
            EditableCombo_Command = new RelayCommand(async () => await OpenEditableComboAsync());
            ValidationTest_Command = new RelayCommand(async () => await OpenValidationTestAsync());
            UnitConversion_Command = new RelayCommand(async () => await OpenUnitConversionAsync());
            StitchImage_Command = new RelayCommand(async () => await OpenImageStitchingAsync());

            PopulateNavItems();
        }

        private Task OpenImageStitchingAsync() =>
            CommonServices.NavigationService.NavigateAsync<StitchingViewModel>();

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
            var imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

            if (await CommonServices.FileService.BrowseFileAsync(imageFilter) is List<FileResult> files && files.Count > 0)
                await CommonServices.DialogService.ShowInformationAsync($"File '{files.First().FileName}' is selected.");
        }

        /// <summary>
        /// Populates the nav items.
        /// </summary>
        protected override void PopulateNavItems()
        {
            PrimaryItems.Clear();
            PrimaryItems.Add(new NavigationItem("Info", (Application.Current.Resources["info"] as string).ToPath(), _themeService.Style.Color, Info_Command));
            PrimaryItems.Add(new NavigationItem("Browse", (Application.Current.Resources["search"] as string).ToPath(), _themeService.Style.Color, Browse_Command));
            PrimaryItems.Add(new NavigationItem("Editable Combobox", (Application.Current.Resources["combobox"] as string).ToPath(), _themeService.Style.Color, EditableCombo_Command));
            PrimaryItems.Add(new NavigationItem("Validation", (Application.Current.Resources["validation"] as string).ToPath(), _themeService.Style.Color, ValidationTest_Command));
            PrimaryItems.Add(new NavigationItem("Unit Conversion", (Application.Current.Resources["weight"] as string).ToPath(), _themeService.Style.Color, UnitConversion_Command));
            PrimaryItems.Add(new NavigationItem("Image Stitching", (Application.Current.Resources["camera"] as string).ToPath(), _themeService.Style.Color, StitchImage_Command));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", (Application.Current.Resources["help"] as string).ToPath(), _themeService.Style.Color, Help_Command));
            SecondaryItems.Add(new NavigationItem("Language", (Application.Current.Resources["flag"] as string).ToPath(), _themeService.Style.Color, Language_Command));
            SecondaryItems.Add(new NavigationItem("Theme", (Application.Current.Resources["color"] as string).ToPath(), _themeService.Style.Color, Color_Command));
            SecondaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", (Application.Current.Resources["user2"] as string).ToPath(), _themeService.Style.Color, Login_Command));
        }

        /// <summary>
        /// Opens the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected override Task OpenSettingsAsync() => 
            ThrowFeatureNotEnabledWarning();
    }
}
