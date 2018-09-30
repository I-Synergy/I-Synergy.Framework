using ISynergy.Models;
using ISynergy.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISynergy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : IView
    {
        public IShellViewModel ViewModel => DataContext as IShellViewModel;

        public ShellView()
        {
            InitializeComponent();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            RootNavigationView.Loaded += RootNavigationView_Loaded;
            RootNavigationView.BackRequested += RootNavigationView_BackRequested;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
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

            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);

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
