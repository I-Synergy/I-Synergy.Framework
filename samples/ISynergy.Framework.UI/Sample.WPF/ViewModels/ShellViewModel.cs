using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.Navigation;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Sample.ViewModels
{
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

        /// <summary>
        /// Gets or sets the information command.
        /// </summary>
        /// <value>The information command.</value>
        public Command Info_Command { get; set; }

        public Command EditableCombo_Command { get; set; }

        /// <summary>
        /// gets or sets the Unit Conversion command.
        /// </summary>
        public Command UnitConversion_Command { get; set; }

        /// <summary>
        /// Gets or sets the Validation test command.
        /// </summary>
        public Command ValidationTest_Command { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="applicationSettingsService"></param>
        /// <param name="logger">The logger factory.</param>
        public ShellViewModel(
            IContext context,
            ICommonServices commonServices,
            IBaseApplicationSettingsService applicationSettingsService,
            ILogger<ShellViewModel> logger)
            : base(context, commonServices, applicationSettingsService, logger, null, null)
        {
            CommonServices = commonServices;

            Title = "Sample WPF Title";
            Version = commonServices.InfoService.ProductVersion;
            DisplayName = "User";

            Info_Command = new Command(async () => await OpenInfoAsync());
            EditableCombo_Command = new Command(async () => await OpenEditableComboAsync());
            ValidationTest_Command = new Command(async () => await OpenValidationTestAsync());
            UnitConversion_Command = new Command(async () => await OpenUnitConversionAsync());

            PopulateNavItems();
        }

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
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["info"] as string, ForegroundColor, Info_Command));
            PrimaryItems.Add(new NavigationItem("Editable Combobox", Application.Current.Resources["combobox"] as string, ForegroundColor, EditableCombo_Command));
            PrimaryItems.Add(new NavigationItem("Validation", Application.Current.Resources["validation"] as string, ForegroundColor, ValidationTest_Command));
            PrimaryItems.Add(new NavigationItem("Unit Conversion", Application.Current.Resources["weight"] as string, ForegroundColor, UnitConversion_Command));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"] as string, ForegroundColor, Help_Command));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["flag"] as string, ForegroundColor, Language_Command));
            SecondaryItems.Add(new NavigationItem("Color", Application.Current.Resources["color"] as string, ForegroundColor, Color_Command));
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
