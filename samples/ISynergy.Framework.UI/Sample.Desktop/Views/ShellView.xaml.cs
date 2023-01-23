using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
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
        public ShellView()
        {
            InitializeComponent();
            InitializeView();
        }

        /// <summary>
        /// Handles the Loaded event of the RootNavigationView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootNavigationViewLoaded(object sender, RoutedEventArgs e)
        {
            // By default does nothing.
        }

        /// <summary>
        /// Backs the invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="KeyboardAcceleratorInvokedEventArgs"/> instance containing the event data.</param>
        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            OnBackRequested();
            args.Handled = true;
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ((IShellViewModel)ViewModel).InitializeAsync();
        }

        /// <summary>
        /// Handles the Navigated event of the ContentRootFrame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        private void ContentRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            RootNavigationView.IsBackEnabled = ViewModel.BaseCommonServices.NavigationService.CanGoBack;
        }

        /// <summary>
        /// Handles the Loaded event of the RootNavigationView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RootNavigationViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
            OnBackRequested();

        /// <summary>
        /// Navigations the item invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NavigationViewItemInvokedEventArgs" /> instance containing the event data.</param>
        private async void NavigationItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
                await ExecuteCommandAsync();
        }

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
