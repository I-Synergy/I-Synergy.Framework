using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using System.Threading.Tasks;

namespace Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : View, IShellView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView" /> class.
        /// </summary>
        public void InitializeView()
        {
            var navigationService = ServiceLocator.Default.GetInstance<INavigationService>();
            navigationService.Frame = ContentRootFrame;

            ViewModel = ServiceLocator.Default.GetInstance<IShellViewModel>();

            RootNavigationView.Loaded += RootNavigationViewLoaded;
            RootNavigationView.BackRequested += RootNavigationViewBackRequested;
            RootNavigationView.ItemInvoked += NavigationItemInvoked;
            ContentRootFrame.Navigated += ContentRootFrameNavigated;
        }

        /// <summary>
        /// Executes settings command if authenticated.
        /// Otherwise authenticate first.
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteCommandAsync()
        {
            Argument.IsNotNull(ViewModel);

            if (ViewModel.Context.IsAuthenticated)
            {
                if (((IShellViewModel)ViewModel).Settings_Command.CanExecute(null))
                    ((IShellViewModel)ViewModel).Settings_Command.Execute(null);
            }
            else
            {
                //await ((IShellViewModel)ViewModel).ProcessAuthenticationRequestAsync();
            }
        }

        /// <summary>
        /// Called when [back requested].
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool OnBackRequested()
        {
            if (ViewModel.BaseCommonServices.NavigationService.CanGoBack)
            {
                ViewModel.BaseCommonServices.NavigationService.GoBack();
                return true;
            }

            return false;
        }
    }
}
