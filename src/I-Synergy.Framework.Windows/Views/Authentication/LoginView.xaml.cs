// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using CommonServiceLocator;
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
        private LoginViewModel ViewModel => DataContext as LoginViewModel;

        public LoginView()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ServiceLocator.Current.GetInstance<INavigationService>().CleanBackStackAsync();

            await ViewModel.CheckAutoLogin();
        }
    }
}
