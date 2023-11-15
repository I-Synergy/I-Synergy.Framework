using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.UI.ViewModels;
using Sample.ViewModels;
using System.Runtime.ExceptionServices;

namespace Sample
{
    public partial class App : BaseApplication
    {
        public App()
            : base()
        {
            InitializeComponent();
        }

        public override async Task InitializeApplicationAsync()
        {
            await base.InitializeApplicationAsync();
            await Task.Delay(1000);
            await ServiceLocator.Default.GetInstance<INavigationService>().NavigateModalAsync<AppShellViewModel>();
        }

        public override IList<ResourceDictionary> GetAdditionalResourceDictionaries() => new List<ResourceDictionary>()
        {
            new Styles.Colors(),
            new Styles.Style()
        };

        public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
        {
            if (ServiceLocator.Default.GetInstance<INavigationService>() is NavigationService navigationService)
            {
                if (e.Value)
                {
                    await navigationService.NavigateModalAsync<AppShellViewModel>();
                }
                else
                {
                    await navigationService.NavigateModalAsync<AuthenticationViewModel>();
                }
            }
        }

        protected override void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            base.CurrentDomain_FirstChanceException(sender, e);
        }
    }
}