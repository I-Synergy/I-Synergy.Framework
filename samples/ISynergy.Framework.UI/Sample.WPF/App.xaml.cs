using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Services;
using Sample.ViewModels;
using System.Windows;

namespace Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BaseApplication
    {
        public App()
            : base()
        {
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await ServiceLocator.Default.GetInstance<INavigationService>().NavigateModalAsync<ShellViewModel>();
        }

        public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
        {
            if (ServiceLocator.Default.GetInstance<INavigationService>() is NavigationService navigationService)
            {
                await navigationService.CleanBackStackAsync();
                await navigationService.NavigateModalAsync<ShellViewModel>();
            }
        }
    }
}
