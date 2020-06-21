using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Windows.Abstractions.Services;
using ISynergy.Framework.Windows.Functions;
using ISynergy.Framework.Windows.Navigation;
using ISynergy.Framework.Windows.Samples.Abstractions.Services;
using ISynergy.Framework.Windows.Samples.Models;
using ISynergy.Framework.Windows.ViewModels;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml;

namespace ISynergy.Framework.Windows.Samples.ViewModels
{
    public class ShellViewModel : ShellViewModelBase, IShellViewModel
    {
        /// <summary>
        /// Gets the common services.
        /// </summary>
        /// <value>The common services.</value>
        public ICommonServices CommonServices { get; }

        public RelayCommand Display_Command { get; set; }

        public ShellViewModel(
            IContext context,
            ICommonServices commonServices,
            ILoggerFactory loggerFactory,
            IThemeSelectorService themeSelectorService,
            LocalizationFunctions localizationFunctions)
            : base(context, commonServices, loggerFactory, themeSelectorService, localizationFunctions)
        {
            CommonServices = commonServices;

            PopulateNavItems();

            Display_Command = new RelayCommand(async () => await OpenDisplayAsync());
        }

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
            return Task.CompletedTask;
        }

        public override Task ProcessAuthenticationRequestAsync()
        {
            return Task.CompletedTask;
        }
    }
}
