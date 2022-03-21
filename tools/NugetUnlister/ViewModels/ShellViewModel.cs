using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.Navigation;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using NugetUnlister.Common.Abstractions;
using NugetUnlister.Common.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace NugetUnlister.ViewModels
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

        /// <summary>
        /// Gets or sets the display command.
        /// </summary>
        /// <value>The display command.</value>
        public Command Unlist_Command { get; set; }


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
            ILogger<ShellViewModel> logger)
            : base(context, commonServices, settingsService, logger, null, null)
        {
            CommonServices = commonServices;

            Title = commonServices.InfoService.ProductName;
            Version = commonServices.InfoService.ProductVersion;
            DisplayName = "User";

            Unlist_Command = new Command(async () => await OpenUnlistViewAsync());

            PopulateNavItems();
        }

        /// <summary>
        /// Opens the display asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        private Task OpenUnlistViewAsync() =>
            CommonServices.NavigationService.NavigateAsync<NugetViewModel>();

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
            PrimaryItems.Add(new NavigationItem("Unlist Nuget packages", Application.Current.Resources["validation"] as string, ForegroundColor, Unlist_Command));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["help"] as string, ForegroundColor, Help_Command));
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
