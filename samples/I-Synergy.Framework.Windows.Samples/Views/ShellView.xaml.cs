using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Windows.Abstractions.Views;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.Windows.Samples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : IShellView
    {
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public IShellViewModel ViewModel => ServiceLocator.Default.GetInstance<IShellViewModel>();

        /// <summary>
        /// The weak root navigation view loaded event
        /// </summary>
        private readonly WeakEventListener<ShellView, object, RoutedEventArgs> WeakRootNavigationViewLoadedEvent = null;
        /// <summary>
        /// The weak root navigation view back requested event
        /// </summary>
        private readonly WeakEventListener<ShellView, Microsoft.UI.Xaml.Controls.NavigationView, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs> WeakRootNavigationViewBackRequestedEvent = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView"/> class.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();

            DataContext = ViewModel;

            WeakRootNavigationViewLoadedEvent = new WeakEventListener<ShellView, object, RoutedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.RootNavigationView_Loaded(source, eventargs),
                OnDetachAction = (listener) => RootNavigationView.Loaded -= listener.OnEvent
            };

            RootNavigationView.Loaded += WeakRootNavigationViewLoadedEvent.OnEvent;

            WeakRootNavigationViewBackRequestedEvent = new WeakEventListener<ShellView, Microsoft.UI.Xaml.Controls.NavigationView, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.RootNavigationView_BackRequested(source, eventargs),
                OnDetachAction = (listener) => RootNavigationView.BackRequested -= listener.OnEvent
            };

            RootNavigationView.BackRequested += WeakRootNavigationViewBackRequestedEvent.OnEvent;
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.InitializeAsync(ContentRootFrame);
        }

        /// <summary>
        /// Navigations the item invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs"/> instance containing the event data.</param>
        private async void NavigationItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked && ViewModel != null)
            {
                if (ViewModel.Context.IsAuthenticated)
                {
                    if (ViewModel.Settings_Command.CanExecute(null)) ViewModel.Settings_Command.Execute(null);
                }
                else
                {
                    await ViewModel.ProcessAuthenticationRequestAsync();
                }
            }
        }

        /// <summary>
        /// Handles the Loaded event of the RootNavigationView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RootNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // add keyboard accelerators for backwards navigation
            var GoBack = new KeyboardAccelerator
            {
                Key = VirtualKey.GoBack
            };

            GoBack.Invoked += BackInvoked;

            var AltLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left
            };

            AltLeft.Invoked += BackInvoked;

            KeyboardAccelerators.Add(GoBack);
            KeyboardAccelerators.Add(AltLeft);

            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;
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
        /// Roots the navigation view back requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs"/> instance containing the event data.</param>
        private void RootNavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            OnBackRequested();
        }

        /// <summary>
        /// Handles the Navigated event of the ContentRootFrame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        private void ContentRootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            RootNavigationView.IsBackEnabled = ViewModel.BaseCommonServices.NavigationService.CanGoBack;
        }

        /// <summary>
        /// Called when [back requested].
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool OnBackRequested()
        {
            var navigated = false;

            // don't go back if the navigation pane is overlaid
            if (RootNavigationView.IsPaneOpen && (RootNavigationView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Compact || RootNavigationView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal))
            {
                return false;
            }
            else
            {
                if (ViewModel.BaseCommonServices.NavigationService.CanGoBack)
                {
                    ViewModel.BaseCommonServices.NavigationService.GoBack();
                    navigated = true;
                }
            }
            return navigated;
        }
    }
}
