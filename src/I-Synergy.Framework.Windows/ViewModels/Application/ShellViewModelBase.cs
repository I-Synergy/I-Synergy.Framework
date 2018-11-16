using ISynergy.Services;
using ISynergy.ViewModels.Base;
using ISynergy.Views.Library;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using ISynergy.Models;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.ViewModels.Library;
using ISynergy.ViewModels.Authentication;
using ISynergy.Views.Authentication;
using ISynergy.Events;
using ISynergy.Enumerations;
using GalaSoft.MvvmLight.Ioc;

namespace ISynergy.ViewModels
{
    public abstract class ShellViewModelBase : ViewModel
    {
        // public IBusyService Busy => Container.Resolve<IBusyService>();

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

        public RelayCommand RestartUpdate_Command { get; set; }
        public RelayCommand Login_Command { get; set; }
        public RelayCommand Language_Command { get; set; }
        public RelayCommand Color_Command { get; set; }
        public RelayCommand Background_Command { get; set; }
        public RelayCommand Feedback_Command { get; set; }
        public RelayCommand<VisualStateChangedEventArgs> StateChanged_Command { get; set; }

        public ShellViewModelBase(
            IContext context,
            IBaseService synergyService)
            : base(context, synergyService)
        {
            PrimaryItems = new ObservableCollection<NavigationItem>();
            SecondaryItems = new ObservableCollection<NavigationItem>();

            RestartUpdate_Command = new RelayCommand(async () => await ShowDialogRestartAfterUpdate());
            StateChanged_Command = new RelayCommand<VisualStateChangedEventArgs>(args => GoToState(args.NewState.Name));
            Feedback_Command = new RelayCommand(async () => await CreateFeedbackAsync());

            Messenger.Default.Register<AuthenticateUserMessageRequest>(this, async (request) => await ValidateTaskWithUserAsync(request));
            Messenger.Default.Register<AuthenticateUserMessageResult>(this, async (result) => await OnAuthenticateUserMessageResult(result));
            Messenger.Default.Register<OnSubmittanceMessage>(this, async (e) => await OnSubmittanceAsync(e));
        }

        protected abstract Task OnSubmittanceAsync(OnSubmittanceMessage e);

        protected abstract Task CreateFeedbackAsync();

        public abstract Task InitializeAsync(object parameter);

        private async Task OnAuthenticateUserMessageResult(AuthenticateUserMessageResult result)
        {
            try
            {
                await BaseService.BusyService.StartBusyAsync();

                if (result != null && !result.Handled)
                {
                    if (result.Property.ToString() == nameof(Login_Command) && result.IsAuthenticated)
                    {
                        await BaseService.DialogService.ShowGreetingAsync(Context.CurrentProfile.Username);
                        result.Handled = true;
                    }
                }
            }
            finally
            {
                await BaseService.BusyService.EndBusyAsync();
            }
        }

        private async Task ValidateTaskWithUserAsync(AuthenticateUserMessageRequest request)
        {
            try
            {
                await BaseService.BusyService.StartBusyAsync();

                if(request.ShowLogin)
                {
                    await ProcessAuthenticationRequestAsync();
                }
                else if (Context.Profiles?.Count > 0)
                {
                    TagViewModel tagVM = new TagViewModel(
                        Context, 
                        BaseService, 
                        request.EnableLogin);

                    var tagResult = await BaseService.UIVisualizerService.ShowDialogAsync(typeof(ITagWindow), tagVM);

                    if (tagResult.HasValue && tagResult.Value && tagVM.IsValid)
                    {
                        int tag = Convert.ToInt32(tagVM.RfidTag, 16);

                        if (tag != 0 &&
                            Context.CurrentProfile?.UserInfo?.RfidUid != 0 &&
                            Context.Profiles.Any(q => q.UserInfo.RfidUid == tag) &&
                            Context.Profiles.Single(q => q.UserInfo.RfidUid == tag).TokenExpiration.ToLocalTime() > DateTime.Now)
                        {
                            Context.CurrentProfile = null;
                            Context.CurrentProfile = Context.Profiles.Single(q => q.UserInfo.RfidUid == tag);

                            Messenger.Default.Send(new AuthenticateUserMessageResult(this, request.Property, true));
                        }
                        else
                        {
                            await ProcessAuthenticationRequestAsync();
                        }
                    }
                    else if(!request.EnableLogin)
                    {
                        PincodeViewModel pinVM = new PincodeViewModel(
                            Context,
                            BaseService);

                        var pinResult = await BaseService.UIVisualizerService.ShowDialogAsync(typeof(IPincodeWindow), pinVM);

                        if (pinResult == true && pinVM.Result)
                        {
                            Messenger.Default.Send(new AuthenticateUserMessageResult(this, request.Property, true));
                        }
                        else if(pinResult == true)
                        {
                            await BaseService.DialogService.ShowErrorAsync(BaseService.LanguageService.GetString("Warning_Pincode_Invalid"));
                        }
                    }
                }
            }
            finally
            {
                await BaseService.BusyService.EndBusyAsync();
            }
        }

        public async Task ProcessAuthenticationRequestAsync()
        {
            BaseService.BaseSettingsService.User_AutoLogin = false;

            await BaseService.LoginService.ProcessLoginRequestAsync();

            PopulateNavItems();

            BaseService.NavigationService.Navigate(
                SimpleIoc.Default.GetInstance<ILoginViewModel>().GetType().FullName);
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
                default:
                    break;
            }
        }

        protected Task ShowDialogRestartAfterUpdate()
        {
            return BaseService.DialogService.ShowInformationAsync(BaseService.LanguageService.GetString("Generic_UpdateRestart"));
        }

        /// <summary>
        /// Gets or sets the PrimaryItems property value.
        /// </summary>
        public ObservableCollection<NavigationItem> PrimaryItems
        {
            get { return GetValue<ObservableCollection<NavigationItem>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SecondaryItems property value.
        /// </summary>
        public ObservableCollection<NavigationItem> SecondaryItems
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

        protected void SaveLastUsername(string username)
        {
            BaseService.BaseSettingsService.Application_User = username;
        }

        private void CurrentWallpaperChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is byte[])
            {
                BaseService.BaseSettingsService.Application_Wallpaper = sender as byte[];
            }
        }

        protected async Task OpenLanguageAsync()
        {
            LanguageViewModel langVm = new LanguageViewModel(
                Context,
                BaseService);

            var result = await BaseService.UIVisualizerService.ShowDialogAsync(typeof(LanguageWindow), langVm);

            if (result.HasValue && result.Value && !langVm.IsCancelled)
            {
                if (await BaseService.DialogService.ShowAsync(BaseService.LanguageService.GetString("Warning_Restart"), "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await RestartApplication();
                }
            }
        }

        protected async Task OpenColorsAsync()
        {
            ThemeViewModel langVm = new ThemeViewModel(
                Context,
                BaseService);

            var result = await BaseService.UIVisualizerService.ShowDialogAsync(typeof(ThemeWindow), langVm);

            if (result.HasValue && result.Value && !langVm.IsCancelled)
            {
                if (await BaseService.DialogService.ShowAsync(
                BaseService.LanguageService.GetString("Warning_Color_Change") +
                Environment.NewLine +
                BaseService.LanguageService.GetString("Generic_Do_you_want_to_do_it_now"),
                BaseService.LanguageService.GetString("TitleQuestion"),
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await RestartApplication();
                }
            }
        }

        private Task OpenFeedbackAsync()
        {
            //Windows.System.Launcher.LaunchUriAsync(new Uri("feedback-hub:"));
            return Task.CompletedTask;
        }

        public async Task RestartApplication()
        {
            await BaseService.DialogService.ShowInformationAsync("Please restart the application.");

            Application.Current.Exit();
        }
    }
}