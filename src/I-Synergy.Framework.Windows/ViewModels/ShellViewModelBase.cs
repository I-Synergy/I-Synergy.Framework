using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using ISynergy.Framework.Mvvm.Commands;
using Windows.UI.Xaml.Controls;
using ISynergy.Framework.Windows.Navigation;
using Windows.System;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Windows.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using Microsoft.Extensions.Logging;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Windows.Abstractions.Windows;
using ISynergy.Framework.Windows.Functions;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Windows.ViewModels
{
    /// <summary>
    /// Class ShellViewModelBase.
    /// Implements the <see cref="ViewModel" />
    /// Implements the <see cref="IShellViewModel" />
    /// </summary>
    /// <seealso cref="ViewModel" />
    /// <seealso cref="IShellViewModel" />
    public abstract class ShellViewModelBase : ViewModel, IShellViewModel
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
        /// Gets or sets the DisplayMode property value.
        /// </summary>
        /// <value>The display mode.</value>
        public SplitViewDisplayMode DisplayMode
        {
            get { return GetValue<SplitViewDisplayMode>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedForeground property value.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public SolidColorBrush ForegroundColor
        {
            get { return GetValue<SolidColorBrush>() ?? GetStandardTextColorBrush(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DisplayName property value.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the restart update command.
        /// </summary>
        /// <value>The restart update command.</value>
        public RelayCommand RestartUpdate_Command { get; set; }
        /// <summary>
        /// Gets or sets the login command.
        /// </summary>
        /// <value>The login command.</value>
        public RelayCommand Login_Command { get; set; }
        /// <summary>
        /// Gets or sets the language command.
        /// </summary>
        /// <value>The language command.</value>
        public RelayCommand Language_Command { get; set; }
        /// <summary>
        /// Gets or sets the color command.
        /// </summary>
        /// <value>The color command.</value>
        public RelayCommand Color_Command { get; set; }
        /// <summary>
        /// Gets or sets the help command.
        /// </summary>
        /// <value>The help command.</value>
        public RelayCommand Help_Command { get; set; }
        /// <summary>
        /// Gets or sets the settings command.
        /// </summary>
        /// <value>The settings command.</value>
        public RelayCommand Settings_Command { get; set; }
        /// <summary>
        /// Gets or sets the background command.
        /// </summary>
        /// <value>The background command.</value>
        public RelayCommand Background_Command { get; set; }
        /// <summary>
        /// Gets or sets the feedback command.
        /// </summary>
        /// <value>The feedback command.</value>
        public RelayCommand Feedback_Command { get; set; }
        /// <summary>
        /// Gets or sets the state changed command.
        /// </summary>
        /// <value>The state changed command.</value>
        public RelayCommand<VisualStateChangedEventArgs> StateChanged_Command { get; set; }

        /// <summary>
        /// The theme selector
        /// </summary>
        private readonly IThemeSelectorService _themeSelector;
        /// <summary>
        /// The localization functions
        /// </summary>
        private readonly LocalizationFunctions _localizationFunctions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModelBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="themeSelectorService">The theme selector service.</param>
        /// <param name="localizationFunctions">The localization functions.</param>
        /// <param name="validation">The validation.</param>
        protected ShellViewModelBase(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory,
            IThemeSelectorService themeSelectorService,
            LocalizationFunctions localizationFunctions,
            ValidationTriggers validation = ValidationTriggers.Manual)
            : base(context, commonServices, loggerFactory)
        {
            _themeSelector = themeSelectorService;
            _themeSelector.OnThemeChanged += ThemeSelector_OnThemeChanged;
            _localizationFunctions = localizationFunctions;

            PrimaryItems = new ObservableCollection<NavigationItem>();

            RestartUpdate_Command = new RelayCommand(async () => await ShowDialogRestartAfterUpdateAsync());
            StateChanged_Command = new RelayCommand<VisualStateChangedEventArgs>(args => GoToState(args.NewState.Name));

            Language_Command = new RelayCommand(async () => await OpenLanguageAsync());
            Color_Command = new RelayCommand(async () => await OpenColorsAsync());
            Help_Command = new RelayCommand(async () => await OpenHelpAsync());
            Feedback_Command = new RelayCommand(async () => await CreateFeedbackAsync());
            Settings_Command = new RelayCommand(async () => await OpenSettingsAsync());
        }

        /// <summary>
        /// Themes the selector on theme changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ThemeSelector_OnThemeChanged(object sender, object e)
        {
            ForegroundColor = GetStandardTextColorBrush();
        }

        /// <summary>
        /// Gets the standard text color brush.
        /// </summary>
        /// <returns>SolidColorBrush.</returns>
        private SolidColorBrush GetStandardTextColorBrush()
        {
            var brush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;

            if (!_themeSelector.IsLightThemeEnabled)
            {
                brush = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as SolidColorBrush;
            }

            return brush;
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

                if (PrimaryItems != null && PrimaryItems.Count > 0)
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
        /// Goes to state.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        protected void GoToState(string stateName)
        {
            switch (stateName)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    break;
            }
        }

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
            get { return GetValue<ObservableCollection<NavigationItem>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LastSelectedItem property value.
        /// </summary>
        /// <value>The last selected item.</value>
        public NavigationItem LastSelectedItem
        {
            get { return GetValue<NavigationItem>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
        /// <value>The selected item.</value>
        public NavigationItem SelectedItem
        {
            get { return GetValue<NavigationItem>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the Query property value.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Caption property value.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsUpdateAvailable property value.
        /// </summary>
        /// <value><c>true</c> if this instance is update available; otherwise, <c>false</c>.</value>
        public bool IsUpdateAvailable
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Wallpaper property value.
        /// </summary>
        /// <value>The wallpaper.</value>
        public ImageSource Wallpaper
        {
            get { return GetValue<ImageSource>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the Width property value.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Currents the wallpaper changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentWallpaperChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is byte[])
            {
                BaseCommonServices.ApplicationSettingsService.Wallpaper = sender as byte[];
            }
        }

        /// <summary>
        /// Opens the language asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected Task OpenLanguageAsync()
        {
            var languageVM = new LanguageViewModel(Context, BaseCommonServices, _localizationFunctions, _loggerFactory);
            languageVM.Submitted += LanguageVM_Submitted;
            return BaseCommonServices.UIVisualizerService.ShowDialogAsync<ILanguageWindow, LanguageViewModel, string>(languageVM);
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

            if (await BaseCommonServices.DialogService.ShowAsync(
                            BaseCommonServices.LanguageService.GetString("Warning_Language_Change") +
                            Environment.NewLine +
                            BaseCommonServices.LanguageService.GetString("Do_you_want_to_do_it_now"),
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
            var themeVM = new ThemeViewModel(Context, BaseCommonServices, _loggerFactory);
            themeVM.Submitted += ThemeVM_Submitted;
            return BaseCommonServices.UIVisualizerService.ShowDialogAsync<IThemeWindow, ThemeViewModel, bool>(themeVM);
        }

        /// <summary>
        /// Themes the vm submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ThemeVM_Submitted(object sender, SubmitEventArgs<bool> e)
        {
            if (sender is ThemeViewModel vm)
                vm.Submitted -= ThemeVM_Submitted;

            if (e.Result && await BaseCommonServices.DialogService.ShowAsync(
                            BaseCommonServices.LanguageService.GetString("Warning_Color_Change") +
                            Environment.NewLine +
                            BaseCommonServices.LanguageService.GetString("Do_you_want_to_do_it_now"),
                            BaseCommonServices.LanguageService.GetString("TitleQuestion"),
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await RestartApplicationAsync();
            }
        }

        /// <summary>
        /// Opens the help asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected Task OpenHelpAsync() =>
            BaseCommonServices.DialogService.ShowInformationAsync(BaseCommonServices.LanguageService.GetString("EX_FUTURE_MODULE"));

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
        public async Task RestartApplicationAsync()
        {
            await BaseCommonServices.DialogService.ShowInformationAsync("Please restart the application.");
            Application.Current.Exit();
        }
    }
}
