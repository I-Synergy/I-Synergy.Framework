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
    public class ShellViewModel : ShellViewModelBase, IShellViewModel
    {
        /// <summary>
        /// Gets or sets the Version property value.
        /// </summary>
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

        public Command Display_Command { get; set; }
        public Command Info_Command { get; set; }
        public Command Browse_Command { get; set; }
        public Command Converter_Command { get; set; }

        public ShellViewModel(
            IContext context,
            ICommonServices commonServices,
            ILoggerFactory loggerFactory,
            IThemeSelectorService themeSelectorService,
            LocalizationFunctions localizationFunctions)
            : base(context, commonServices, loggerFactory, themeSelectorService, localizationFunctions)
        {
            CommonServices = commonServices;

            Version = commonServices.InfoService.ProductVersion;
            DisplayName = "User";

            Display_Command = new Command(async () => await OpenDisplayAsync());
            Info_Command = new Command(async () => await OpenInfoAsync());
            Browse_Command = new Command(async () => await BrowseFileAsync());
            Converter_Command = new Command(async () => await OpenConvertersAsync());

            PopulateNavItems();
        }

        private Task OpenConvertersAsync() =>
            CommonServices.NavigationService.NavigateAsync<ConvertersViewModel>();

        private async Task BrowseFileAsync()
        {
            var imageFilter = "Images (Jpeg, Gif, Png)|*.jpg; *.jpeg; *.gif; *.png";

            if (await CommonServices.FileService.BrowseFileAsync(imageFilter, 0) is FileResult file)
            {
                await CommonServices.DialogService.ShowInformationAsync($"File '{file.FileName}' is selected.");
            }
        }

        private Task OpenInfoAsync() =>
            CommonServices.NavigationService.NavigateAsync<InfoViewModel>();

        private Task OpenDisplayAsync() =>
            CommonServices.NavigationService.NavigateAsync<SlideShowViewModel>();

        /// <summary>
        /// initialize as an asynchronous operation.
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }

        protected override void PopulateNavItems()
        {
            PrimaryItems.Clear();
            PrimaryItems.Add(new NavigationItem("SlideShow", Application.Current.Resources["icon_kiosk"] as string, ForegroundColor, Display_Command));
            PrimaryItems.Add(new NavigationItem("Info", Application.Current.Resources["tile_info"] as string, ForegroundColor, Info_Command));
            PrimaryItems.Add(new NavigationItem("Browse", Application.Current.Resources["tb_search"] as string, ForegroundColor, Browse_Command));
            PrimaryItems.Add(new NavigationItem("Converters", Application.Current.Resources["tb_products"] as string, ForegroundColor, Converter_Command));

            SecondaryItems.Clear();
            SecondaryItems.Add(new NavigationItem("Help", Application.Current.Resources["tile_help"] as string, ForegroundColor, Help_Command));
            SecondaryItems.Add(new NavigationItem("Language", Application.Current.Resources["icon_flag"] as string, ForegroundColor, Language_Command));
            SecondaryItems.Add(new NavigationItem("Color", Application.Current.Resources["icon_color"] as string, ForegroundColor, Color_Command));
            SecondaryItems.Add(new NavigationItem(Context.IsAuthenticated ? "Logout" : "Login", Application.Current.Resources["icon_user"] as string, ForegroundColor, Login_Command));
        }

        protected override Task CreateFeedbackAsync() => ThrowFeatureNotEnabledWarning();

        protected override Task OpenSettingsAsync() => ThrowFeatureNotEnabledWarning();

        public override Task InitializeAsync(object parameter)
        {
            CommonServices.NavigationService.Frame = parameter;
            return Task.CompletedTask;
        }

        public override Task ProcessAuthenticationRequestAsync()
        {
            return Task.CompletedTask;
        }
    }
}
