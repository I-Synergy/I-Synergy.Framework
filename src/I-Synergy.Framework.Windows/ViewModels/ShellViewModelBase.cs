using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using ISynergy.Framework.Mvvm.Commands;
using Windows.UI.Xaml.Controls;
using ISynergy.Framework.Mvvm.Messaging;
using ISynergy.Framework.Windows.Navigation;
using Windows.System;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Messages;
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
    public abstract class ShellViewModelBase : ViewModel, IShellViewModel
    {
        protected const string SetApplicationColor = "Set Application Color";
        protected const string SetApplicationWallpaper = "Set Application Wallpaper";

        protected const string PanoramicStateName = "PanoramicState";
        protected const string WideStateName = "WideState";
        protected const string NarrowStateName = "NarrowState";
        protected const double WideStateMinWindowWidth = 640;
        protected const double PanoramicStateMinWindowWidth = 1024;

        /// <summary>
        /// Gets or sets the DisplayMode property value.
        /// </summary>
        public SplitViewDisplayMode DisplayMode
        {
            get { return GetValue<SplitViewDisplayMode>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedForeground property value.
        /// </summary>
        public SolidColorBrush ForegroundColor
        {
            get { return GetValue<SolidColorBrush>() ?? GetStandardTextColorBrush(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the DisplayName property value.
        /// </summary>
        public string DisplayName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        
        public RelayCommand RestartUpdate_Command { get; set; }
        public RelayCommand Login_Command { get; set; }
        public RelayCommand Language_Command { get; set; }
        public RelayCommand Color_Command { get; set; }
        public RelayCommand Help_Command { get; set; }
        public RelayCommand Settings_Command { get; set; }
        public RelayCommand Background_Command { get; set; }
        public RelayCommand Feedback_Command { get; set; }
        public RelayCommand<VisualStateChangedEventArgs> StateChanged_Command { get; set; }

        private readonly IThemeSelectorService _themeSelector;
        private readonly LocalizationFunctions _localizationFunctions;

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

            Messenger.Default.Register<AuthenticateUserMessageRequest>(this, async (request) => await ValidateTaskWithUserAsync(request));
            Messenger.Default.Register<AuthenticateUserMessageResult>(this, async (result) => await OnAuthenticateUserMessageResult(result));
        }

        private void ThemeSelector_OnThemeChanged(object sender, object e)
        {
            ForegroundColor = GetStandardTextColorBrush();
        }

        private SolidColorBrush GetStandardTextColorBrush()
        {
            var brush = Application.Current.Resources["SystemControlForegroundBaseHighBrush"] as SolidColorBrush;

            if (!_themeSelector.IsLightThemeEnabled)
            {
                brush = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as SolidColorBrush;
            }

            return brush;
        }

        protected abstract Task CreateFeedbackAsync();
        protected abstract Task OpenSettingsAsync();

        public abstract Task InitializeAsync(object parameter);

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

        private async Task OnAuthenticateUserMessageResult(AuthenticateUserMessageResult result)
        {
            try
            {
                await BaseCommonServices.BusyService.StartBusyAsync();

                if (result != null && !result.Handled)
                {
                    if (result.Property.ToString() == nameof(Login_Command) && result.IsAuthenticated)
                    {
                        await BaseCommonServices.DialogService.ShowGreetingAsync(Context.CurrentProfile.Username);
                        result.Handled = true;
                    }
                }
            }
            finally
            {
                await BaseCommonServices.BusyService.EndBusyAsync();
            }
        }

        private async Task ValidateTaskWithUserAsync(AuthenticateUserMessageRequest request)
        {
            try
            {
                await BaseCommonServices.BusyService.StartBusyAsync();

                if (request.ShowLogin)
                {
                    await ProcessAuthenticationRequestAsync();
                }
                else if (Context.Profiles?.Count > 0)
                {
                    var tagVM = new TagViewModel(Context, BaseCommonServices, _loggerFactory, request);
                    //tagVM.Submitted += TagVM_Submitted;
                    await BaseCommonServices.UIVisualizerService.ShowDialogAsync(typeof(ITagWindow), tagVM);
                }
            }
            finally
            {
                await BaseCommonServices.BusyService.EndBusyAsync();
            }
        }

        //private async void TagVM_Submitted(object sender, SubmitEventArgs<bool> e)
        //{
        //    if (e.Result)
        //    {
        //        var tag = Convert.ToInt32(tagVM.RfidTag, 16);

        //        if (tag != 0
        //            && Context.CurrentProfile?.RfidUid != 0
        //            && Context.Profiles.Any(q => q.RfidUid == tag)
        //            && Context.Profiles.Single(q => q.RfidUid == tag).Expiration.ToLocalTime() > DateTime.Now)
        //        {
        //            Context.CurrentProfile = null;
        //            Context.CurrentProfile = Context.Profiles.Single(q => q.RfidUid == tag);

        //            Messenger.Default.Send(new AuthenticateUserMessageResult(this, tagVM.Property, true));
        //        }
        //        else
        //        {
        //            await ProcessAuthenticationRequestAsync();
        //        }
        //    }
        //    else if (!tagVM.IsLoginVisible)
        //    {
        //        var pinVM = new PincodeViewModel(Context, BaseCommonServices, _loggerFactory, tagVM.Property);
        //        pinVM.Submitted += PinVM_Submitted;
        //        await BaseCommonServices.UIVisualizerService.ShowDialogAsync(typeof(IPincodeWindow), pinVM);
        //    }
        //}

        //private async void PinVM_Submitted(object sender, SubmitEventArgs<bool> e)
        //{
        //    if (e.Result)
        //    {
        //        Messenger.Default.Send(new AuthenticateUserMessageResult(this, pinVM.Property, true));
        //    }
        //    else
        //    {
        //        await BaseCommonServices.DialogService.ShowErrorAsync(BaseCommonServices.LanguageService.GetString("Warning_Pincode_Invalid"));
        //    }
        //}

        public async Task ProcessAuthenticationRequestAsync()
        {
            BaseCommonServices.ApplicationSettingsService.IsAutoLogin = false;

            await BaseCommonServices.LoginService.ProcessLoginRequestAsync();

            PopulateNavItems();

            DisplayName = string.Empty;

            await BaseCommonServices.NavigationService.NavigateAsync<ILoginViewModel>();
        }

        protected abstract void PopulateNavItems();

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

        protected Task ShowDialogRestartAfterUpdateAsync() =>
            BaseCommonServices.DialogService.ShowInformationAsync(BaseCommonServices.LanguageService.GetString("UpdateRestart"));

        /// <summary>
        /// Gets or sets the PrimaryItems property value.
        /// </summary>
        public ObservableCollection<NavigationItem> PrimaryItems
        {
            get { return GetValue<ObservableCollection<NavigationItem>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the LastSelectedItem property value.
        /// </summary>
        public NavigationItem LastSelectedItem
        {
            get { return GetValue<NavigationItem>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedItem property value.
        /// </summary>
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
        public string Query
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Caption property value.
        /// </summary>
        public string Caption
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the IsUpdateAvailable property value.
        /// </summary>
        public bool IsUpdateAvailable
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Wallpaper property value.
        /// </summary>
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
        public double Width
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        private void CurrentWallpaperChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is byte[])
            {
                BaseCommonServices.ApplicationSettingsService.Wallpaper = sender as byte[];
            }
        }

        protected Task OpenLanguageAsync()
        {
            var languageVM = new LanguageViewModel(Context, BaseCommonServices, _localizationFunctions, _loggerFactory);
            languageVM.Submitted += LanguageVM_Submitted;
            return BaseCommonServices.UIVisualizerService.ShowDialogAsync<ILanguageWindow, LanguageViewModel, string>(languageVM);
        }

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

        protected Task OpenColorsAsync()
        {
            var themeVM = new ThemeViewModel(Context, BaseCommonServices, _loggerFactory);
            themeVM.Submitted += ThemeVM_Submitted;
            return BaseCommonServices.UIVisualizerService.ShowDialogAsync<IThemeWindow, ThemeViewModel, bool>(themeVM);
        }

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
        
        protected Task OpenHelpAsync() =>
            BaseCommonServices.DialogService.ShowInformationAsync(BaseCommonServices.LanguageService.GetString("EX_FUTURE_MODULE"));

        protected static async Task OpenFeedbackAsync()
        {
            await Launcher.LaunchUriAsync(new Uri("feedback-hub:"));
        }

        public async Task RestartApplicationAsync()
        {
            await BaseCommonServices.DialogService.ShowInformationAsync("Please restart the application.");
            Application.Current.Exit();
        }
    }
}
