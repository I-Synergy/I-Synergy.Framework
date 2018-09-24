using ISynergy.Services;
using ISynergy.ViewModels.Authentication;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Views.Authentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : ILoginView
    {
        private ILoginViewModel ViewModel => DataContext as ILoginViewModel;

        public LoginView()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.SynergyService.Navigation.CleanBackStackAsync();
            await ViewModel.CheckAutoLogin();
        }
    }
}
