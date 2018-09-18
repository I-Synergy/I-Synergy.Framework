using ISynergy.Library;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using ISynergy.Views.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using ISynergy.Models;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.ViewModels.Library;
using ISynergy.ViewModels.Authentication;
using ISynergy.Views.Authentication;
using ISynergy.Events;

namespace ISynergy.ViewModels
{
    public abstract class ShellViewModelBase : ViewModel
    {
        private const string SetApplicationColor = "Set Application Color";
        private const string SetApplicationWallpaper = "Set Application Wallpaper";

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

        public ShellViewModelBase(IContext context, IBusyService busy)
            : base(context, busy)
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

        private async Task OnAuthenticateUserMessageResult(AuthenticateUserMessageResult result)
        {
            try
            {
                await Busy.StartBusyAsync();

                if (result != null && !result.IsHandled)
                {
                    if (result.Property.ToString() == nameof(Login_Command) && result.IsAuthenticated)
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowGreetingAsync(Context.CurrentProfile.Username);
                        result.IsHandled = true;
                    }
                }
            }
            finally
            {
                await Busy.EndBusyAsync();
            }
        }

        private async Task ValidateTaskWithUserAsync(AuthenticateUserMessageRequest request)
        {
            try
            {
                await Busy.StartBusyAsync();

                if (Context.Profiles?.Count > 0)
                {
                    TagViewModel tagVM = new TagViewModel(Context, Busy, request.EnableLogin);
                    var tagResult = await ServiceLocator.Current.GetInstance<IUIVisualizerService>().ShowDialogAsync(typeof(ITagWindow), tagVM);

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

                            Messenger.Default.Send(new AuthenticateUserMessageResult
                            {
                                Property = request.Property,
                                IsAuthenticated = true
                            });
                        }
                        else
                        {
                            await ProcessLoginRequestAsync();
                        }
                    }
                    else if(!request.EnableLogin)
                    {
                        PincodeViewModel pinVM = new PincodeViewModel(Context, Busy);
                        var pinResult = await ServiceLocator.Current.GetInstance<IUIVisualizerService>().ShowDialogAsync(typeof(IPincodeWindow), pinVM);

                        if (pinResult == true && pinVM.Result)
                        {
                            Messenger.Default.Send(new AuthenticateUserMessageResult
                            {
                                Property = request.Property,
                                IsAuthenticated = true
                            });
                        }
                        else if(pinResult == true)
                        {
                            await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Pincode_Invalid"));
                        }
                    }
                }
            }
            finally
            {
                await Busy.EndBusyAsync();
            }
        }

        public async Task ProcessLoginRequestAsync()
        {
            ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin = false;

            await ServiceLocator.Current.GetInstance<IAuthenticationService>().ProcessLoginRequestAsync();

            PopulateNavItems();

            ServiceLocator.Current.GetInstance<INavigationService>().Navigate(typeof(LoginViewModel).FullName);
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
            return ServiceLocator.Current.GetInstance<IDialogService>().ShowInformationAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_UpdateRestart"));
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
                Messenger.Default.Send(new ChangeWallpaperMessage { Value = value });
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
            ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_User = username;
        }

        private void CurrentWallpaperChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is byte[])
            {
                ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Wallpaper = sender as byte[];
            }
        }

        protected async Task OpenLanguageAsync()
        {
            LanguageViewModel langVm = new LanguageViewModel(Context, Busy);
            var result = await ServiceLocator.Current.GetInstance<IUIVisualizerService>().ShowDialogAsync(typeof(LanguageWindow), langVm);

            if (result.HasValue && result.Value)
            {
                if (await ServiceLocator.Current.GetInstance<IDialogService>().ShowAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Restart"), "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await RestartApplication();
                }
            }
        }

        protected async Task OpenColorsAsync()
        {
            ThemeViewModel langVm = new ThemeViewModel(Context, Busy);
            var result = await ServiceLocator.Current.GetInstance<IUIVisualizerService>().ShowDialogAsync(typeof(ThemeWindow), langVm);

            if (result.HasValue && result.Value)
            {
                if (await ServiceLocator.Current.GetInstance<IDialogService>().ShowAsync(
                ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Color_Change") +
                Environment.NewLine +
                ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Do_you_want_to_do_it_now"),
                ServiceLocator.Current.GetInstance<ILanguageService>().GetString("TitleQuestion"),
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

        protected async Task SetBackgroundAsync()
        {
            ServiceLocator.Current.GetInstance<ITelemetryService>().TrackEvent(SetApplicationWallpaper);

            if (ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Wallpaper is null || ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Wallpaper.Length == 0)
            {
                var result = await ServiceLocator.Current.GetInstance<IFileService>().BrowseFileAsync(
                    ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Filetypes_Images"),
                    2 * 1024 * 1024,
                    "2MB");

                if (result != null)
                {
                    ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Wallpaper = result.File;
                    //Wallpaper = Images.ConvertByteArray2ImageSource(result.File);
                }
            }
            else
            {
                if (await ServiceLocator.Current.GetInstance<IDialogService>()
                    .ShowAsync(
                        ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Wallpaper_Reset"), "Reset", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Wallpaper = Array.Empty<byte>();
                    Wallpaper = null;
                }
            }
        }

        public async Task RestartApplication()
        {
            await ServiceLocator.Current.GetInstance<IDialogService>().ShowInformationAsync("Please restart the application.");

            ServiceLocator.Current.GetInstance<IWindowService>().ExitApplication();
        }
    }
}