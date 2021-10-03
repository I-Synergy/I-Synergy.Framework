using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ISynergy.Framework.Core.Events;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Xaml.Media;

#if NETFX_CORE
using Windows.UI.Xaml.Input;
using Windows.System;
#endif

namespace Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : ISynergy.Framework.UI.Controls.View, IShellView
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
        private readonly WeakEventListener<ShellView, NavigationView, NavigationViewBackRequestedEventArgs> WeakRootNavigationViewBackRequestedEvent = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView" /> class.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;

            DataContext = ViewModel;

            WeakRootNavigationViewLoadedEvent = new WeakEventListener<ShellView, object, RoutedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.RootNavigationViewLoaded(source, eventargs),
                OnDetachAction = (listener) => RootNavigationView.Loaded -= listener.OnEvent
            };

            RootNavigationView.Loaded += WeakRootNavigationViewLoadedEvent.OnEvent;

            WeakRootNavigationViewBackRequestedEvent = new WeakEventListener<ShellView, NavigationView, NavigationViewBackRequestedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.RootNavigationViewBackRequested(source, eventargs),
                OnDetachAction = (listener) => RootNavigationView.BackRequested -= listener.OnEvent
            };

            RootNavigationView.BackRequested += WeakRootNavigationViewBackRequestedEvent.OnEvent;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (RootNavigationView.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
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
        /// Navigations the item invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NavigationViewItemInvokedEventArgs" /> instance containing the event data.</param>
        private async void NavigationItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked && ViewModel != null)
            {
                if (ViewModel.Context.IsAuthenticated)
                {
                    if (ViewModel.Settings_Command.CanExecute(null))
                        ViewModel.Settings_Command.Execute(null);
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
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RootNavigationViewLoaded(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE
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


#if NETFX_CORE
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
#endif

        /// <summary>
        /// Roots the navigation view back requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NavigationViewBackRequestedEventArgs" /> instance containing the event data.</param>
        private void RootNavigationViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            OnBackRequested();
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
