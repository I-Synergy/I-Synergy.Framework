using ISynergy.Models;
using ISynergy.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.System;
using GalaSoft.MvvmLight.Ioc;
using ISynergy.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISynergy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : IView
    {
        public IShellViewModel ViewModel => SimpleIoc.Default.GetInstance<IShellViewModel>();

        private readonly WeakEventListener<ShellView, object, RoutedEventArgs> WeakRootNavigationViewLoadedEvent = null;
        private readonly WeakEventListener<ShellView, NavigationView, NavigationViewBackRequestedEventArgs> WeakRootNavigationViewBackRequestedEvent = null;

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

            WeakRootNavigationViewBackRequestedEvent = new WeakEventListener<ShellView, NavigationView, NavigationViewBackRequestedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.RootNavigationView_BackRequested(source, eventargs),
                OnDetachAction = (listener) => RootNavigationView.BackRequested -= listener.OnEvent
            };

            RootNavigationView.BackRequested += WeakRootNavigationViewBackRequestedEvent.OnEvent;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.InitializeAsync(ContentRootFrame);
        }

        private void NavigationItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if(args.InvokedItem != null && args.InvokedItem is NavigationItem)
            {
                NavigationItem item = args.InvokedItem as NavigationItem;
                if (item.Command.CanExecute(item.CommandParameter)) item.Command.Execute(item.CommandParameter);
            }
        }

        private void NavigationViewList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null && e.ClickedItem is NavigationItem)
            {
                NavigationItem item = e.ClickedItem as NavigationItem;
                if (item.Command.CanExecute(item.CommandParameter)) item.Command.Execute(item.CommandParameter);
            }
        }

        private void RootNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // add keyboard accelerators for backwards navigation
            KeyboardAccelerator GoBack = new KeyboardAccelerator
            {
                Key = VirtualKey.GoBack
            };

            GoBack.Invoked += BackInvoked;

            KeyboardAccelerator AltLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left
            };

            AltLeft.Invoked += BackInvoked;

            KeyboardAccelerators.Add(GoBack);
            KeyboardAccelerators.Add(AltLeft);

            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;

        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            OnBackRequested();
            args.Handled = true;
        }

        private void RootNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            OnBackRequested();
        }

        private void ContentRootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            RootNavigationView.IsBackEnabled = ContentRootFrame.CanGoBack;
        }

        private bool OnBackRequested()
        {
            bool navigated = false;

            // don't go back if the nav pane is overlayed
            if (RootNavigationView.IsPaneOpen && (RootNavigationView.DisplayMode == NavigationViewDisplayMode.Compact || RootNavigationView.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }
            else
            {
                if (ContentRootFrame.CanGoBack)
                {
                    ContentRootFrame.GoBack();
                    navigated = true;
                }
            }
            return navigated;

        }
    }
}
