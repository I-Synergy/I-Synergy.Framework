using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Windows;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Navigation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Style = ISynergy.Framework.Core.Models.Style;

namespace ISynergy.Framework.UI.ViewModels
{
    /// <summary>
    /// Class ShellViewModelBase.
    /// Implements the <see cref="ViewModel" />
    /// Implements the <see cref="IShellViewModel" />
    /// </summary>
    /// <seealso cref="ViewModel" />
    /// <seealso cref="IShellViewModel" />
    public abstract partial class ShellViewModelBase : ViewModel, IShellViewModel
    {
        /// <summary>
        /// The set application color
        /// </summary>
        protected const string SetApplicationColor = "Set Application Color";

        /// <summary>
        /// The set application wallpaper
        /// </summary>
        protected const string SetApplicationWallpaper = "Set Application Wallpaper";

        /// <summary>
        /// The panoramic state name
        /// </summary>
        protected const string PanoramicStateName = "PanoramicState";

        /// <summary>
        /// The wide state name
        /// </summary>
        protected const string WideStateName = "WideState";
        
        /// <summary>
        /// The narrow state name
        /// </summary>
        protected const string NarrowStateName = "NarrowState";
        
        /// <summary>
        /// The wide state minimum window width
        /// </summary>
        protected const double WideStateMinWindowWidth = 640;
        
        /// <summary>
        /// The panoramic state minimum window width
        /// </summary>
        protected const double PanoramicStateMinWindowWidth = 1024;

        /// <summary>
        /// Gets or sets the DisplayName property value.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the restart update command.
        /// </summary>
        /// <value>The restart update command.</value>
        public Command RestartUpdate_Command { get; set; }

        /// <summary>
        /// Gets or sets the login command.
        /// </summary>
        /// <value>The login command.</value>
        public Command Login_Command { get; set; }

        /// <summary>
        /// Gets or sets the language command.
        /// </summary>
        /// <value>The language command.</value>
        public Command Language_Command { get; set; }

        /// <summary>
        /// Gets or sets the color command.
        /// </summary>
        /// <value>The color command.</value>
        public Command Color_Command { get; set; }

        /// <summary>
        /// Gets or sets the help command.
        /// </summary>
        /// <value>The help command.</value>
        public Command Help_Command { get; set; }

        /// <summary>
        /// Gets or sets the settings command.
        /// </summary>
        /// <value>The settings command.</value>
        public Command Settings_Command { get; set; }

        /// <summary>
        /// Gets or sets the background command.
        /// </summary>
        /// <value>The background command.</value>
        public Command Background_Command { get; set; }

        /// <summary>
        /// Gets or sets the feedback command.
        /// </summary>
        /// <value>The feedback command.</value>
        public Command Feedback_Command { get; set; }

        /// <summary>
        /// The settings service.
        /// </summary>
        private readonly IBaseApplicationSettingsService _appSettingsService;

        /// <summary>
        /// The theme selector
        /// </summary>
        private readonly IThemeService _themeService;

        /// <summary>
        /// The localization functions
        /// </summary>
        private readonly LocalizationFunctions _localizationFunctions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModelBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="appSettingsService">The settings services.</param>
        /// <param name="logger">The logger factory.</param>
        /// <param name="themeService">The theme selector service.</param>
        /// <param name="localizationFunctions">The localization functions.</param>
        protected ShellViewModelBase(
            IContext context,
            IBaseCommonServices commonServices,
            IBaseApplicationSettingsService appSettingsService,
            ILogger logger,
            IThemeService themeService,
            LocalizationFunctions localizationFunctions)
            : base(context, commonServices, logger)
        {
            _appSettingsService = appSettingsService;
            _themeService = themeService;
            _localizationFunctions = localizationFunctions;

            InitializeUI();

            PrimaryItems = new ObservableCollection<NavigationItem>();
            SecondaryItems = new ObservableCollection<NavigationItem>();

            RestartUpdate_Command = new Command(async () => await ShowDialogRestartAfterUpdateAsync());

            Language_Command = new Command(async () => await OpenLanguageAsync());
            Color_Command = new Command(async () => await OpenColorsAsync());
            Help_Command = new Command(async () => await OpenHelpAsync());
            Feedback_Command = new Command(async () => await CreateFeedbackAsync());
            Settings_Command = new Command(async () => await OpenSettingsAsync());
        }



        /// <summary>
        /// Creates the feedback asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected abstract Task CreateFeedbackAsync();

        /// <summary>
        /// Opens the settings asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected abstract Task OpenSettingsAsync();

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        public abstract Task InitializeAsync(object parameter);

        /// <summary>
        /// Processes the authentication changed asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task ProcessAuthenticationChangedAsync()
        {
            PopulateNavItems();

            if (Context.IsAuthenticated)
            {
                DisplayName = Context.CurrentProfile.Username;

                if (PrimaryItems is not null && PrimaryItems.Count > 0)
                {
                    if (PrimaryItems.First().Command.CanExecute(PrimaryItems.First().CommandParameter))
                        PrimaryItems.First().Command.Execute(PrimaryItems.First().CommandParameter);

                    SelectedItem = PrimaryItems.First();
                }
            }
            else
            {
                DisplayName = string.Empty;
                PrimaryItems?.Clear();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// process authentication request as an asynchronous operation.
        /// </summary>
        /// <returns>Task.</returns>
        public abstract Task ProcessAuthenticationRequestAsync();

        /// <summary>
        /// Populates the nav items.
        /// </summary>
        protected abstract void PopulateNavItems();

        /// <summary>
        /// Shows the dialog restart after update asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected Task ShowDialogRestartAfterUpdateAsync() =>
            BaseCommonServices.DialogService.ShowInformationAsync(BaseCommonServices.LanguageService.GetString("UpdateRestart"));

        /// <summary>
        /// Gets or sets the PrimaryItems property value.
        /// </summary>
        /// <value>The primary items.</value>
        public ObservableCollection<NavigationItem> PrimaryItems
        {
            get => GetValue<ObservableCollection<NavigationItem>>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SecondaryItems property value.
        /// </summary>
        /// <value>The primary items.</value>
        public ObservableCollection<NavigationItem> SecondaryItems
        {
            get => GetValue<ObservableCollection<NavigationItem>>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the LastSelectedItem property value.
        /// </summary>
        /// <value>The last selected item.</value>
        public NavigationItem LastSelectedItem
        {
            get => GetValue<NavigationItem>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        /// <value>The selected item.</value>
        public NavigationItem SelectedItem
        {
            get => GetValue<NavigationItem>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Query property value.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Caption property value.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the IsUpdateAvailable property value.
        /// </summary>
        /// <value><c>true</c> if this instance is update available; otherwise, <c>false</c>.</value>
        public bool IsUpdateAvailable
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Width property value.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        /// <summary>
        /// Currents the wallpaper changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentWallpaperChanged(object sender, EventArgs e)
        {
            if (sender is not null && sender is byte[])
            {
                _appSettingsService.Settings.Wallpaper = sender as byte[];
            }
        }

        /// <summary>
        /// Opens the language asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected Task OpenLanguageAsync()
        {
            var languageVM = new LanguageViewModel(Context, BaseCommonServices, _appSettingsService, _localizationFunctions, Logger);
            languageVM.Submitted += LanguageVM_Submitted;
            return BaseCommonServices.DialogService.ShowDialogAsync<ILanguageWindow, LanguageViewModel, string>(languageVM);
        }

        /// <summary>
        /// Languages the vm submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void LanguageVM_Submitted(object sender, SubmitEventArgs<string> e)
        {
            if (sender is LanguageViewModel vm)
                vm.Submitted -= LanguageVM_Submitted;

            if (await BaseCommonServices.DialogService.ShowMessageAsync(
                        BaseCommonServices.LanguageService.GetString("WarningLanguageChange") +
                        Environment.NewLine +
                        BaseCommonServices.LanguageService.GetString("WarningDoYouWantToDoItNow"),
                        BaseCommonServices.LanguageService.GetString("TitleQuestion"),
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await RestartApplicationAsync();
            }
        }

        /// <summary>
        /// Opens the colors asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected Task OpenColorsAsync()
        {
            var themeVM = new ThemeViewModel(Context, BaseCommonServices, _appSettingsService, _themeService, Logger);
            themeVM.Submitted += ThemeVM_Submitted;
            return BaseCommonServices.DialogService.ShowDialogAsync<IThemeWindow, ThemeViewModel, Style>(themeVM);
        }

        /// <summary>
        /// Themes the vm submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ThemeVM_Submitted(object sender, SubmitEventArgs<Style> e)
        {
            if (sender is ThemeViewModel vm)
                vm.Submitted -= ThemeVM_Submitted;
        }

        /// <summary>
        /// Opens the help asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected virtual Task OpenHelpAsync() => ThrowFeatureNotEnabledWarning();

        /// <summary>
        /// open feedback as an asynchronous operation.
        /// </summary>
        protected static async Task OpenFeedbackAsync()
        {
            await Launcher.LaunchUriAsync(new Uri("feedback-hub:"));
        }

        /// <summary>
        /// restart application as an asynchronous operation.
        /// </summary>
        public Task RestartApplicationAsync() =>
            BaseCommonServices.DialogService.ShowInformationAsync("Please restart the application.");
    }
}
