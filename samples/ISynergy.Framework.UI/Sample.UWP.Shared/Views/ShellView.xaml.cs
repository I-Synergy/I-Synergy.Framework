using Microsoft.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Sample.Views
{
    public sealed partial class ShellView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView" /> class.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();

            BackdropMaterial.SetApplyToRootOrPageBackground(this, true);

            InitializeView();
        }

        /// <summary>
        /// Handles the Loaded event of the RootNavigationView control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootNavigationViewLoaded(object sender, RoutedEventArgs e)
        {
#if WINDOWS_UWP
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
#endif
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
            await ViewModel.InitializeAsync(ContentRootFrame);
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
    }
}
