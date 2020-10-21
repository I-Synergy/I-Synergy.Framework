using System;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Navigation;
using ISynergy.Framework.UI.Sample.Abstractions.Services;
using ISynergy.Framework.UI.Sample.Shared.ViewModels;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml;

namespace ISynergy.Framework.UI.Sample.ViewModels
{
    public class ShellViewModel : ShellViewModelBase, IShellViewModel
    {
        /// <summary>
        /// Gets the common services.
        /// </summary>
        /// <value>The common services.</value>
        public ICommonServices CommonServices { get; }

        public RelayCommand Display_Command { get; set; }
        public RelayCommand Info_Command { get; set; }

        public ShellViewModel(
            IContext context,
            ICommonServices commonServices,
            ILoggerFactory loggerFactory,
            IThemeSelectorService themeSelectorService,
            LocalizationFunctions localizationFunctions)
            : base(context, commonServices, loggerFactory, themeSelectorService, localizationFunctions)
        {
            CommonServices = commonServices;

            Display_Command = new RelayCommand(async () => await OpenDisplayAsync());
            Info_Command = new RelayCommand(async () => await OpenInfoAsync());

            PopulateNavItems();
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
        }

        protected override Task CreateFeedbackAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task OpenSettingsAsync()
        {
            throw new NotImplementedException();
        }

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
