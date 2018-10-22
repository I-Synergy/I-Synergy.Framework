using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ISynergy.ViewModels.Base;
using Windows.UI.Core;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using ISynergy.Controls.Views;

namespace ISynergy.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        private Frame _frame;

        public object Frame
        {
            get
            {
                if (_frame is null)
                {
                    _frame = Window.Current.Content as Frame;
                }

                return _frame;
            }
            set
            {
                //if (_frame != null)
                //{
                //    _frame.Navigated -= OnFrameNavigated;
                //}
                //if (value != null && value is Frame)
                //{
                //    ((Frame)value).Navigated += OnFrameNavigated;
                //}
                _frame = (Frame)value;
            }
        }

        public bool CanGoBack => ((Frame)Frame).CanGoBack;
        public bool CanGoForward => ((Frame)Frame).CanGoForward;

        public void GoBack() => ((Frame)Frame).GoBack();
        public void GoForward() => ((Frame)Frame).GoForward();

        public bool Navigate(string pageKey, object parameter = null, object infoOverride = null)
        {
            lock (_pages)
            {
                if (!_pages.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"Page not found: {pageKey}. Did you forget to call NavigationService.Configure?", "pageKey");
                }
                
                var navigationResult = ((Frame)Frame).Navigate(_pages[pageKey], parameter, (NavigationTransitionInfo)infoOverride);
                return navigationResult;
            }
        }

        public IView GetNavigationBlade(string pageKey, object parameter = null)
        {
            lock (_pages)
            {
                if (!_pages.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"Page not found: {pageKey}. Did you forget to call NavigationService.Configure?", "pageKey");
                }

                IView result = (IView)Activator.CreateInstance(_pages[pageKey]);

                if(parameter != null && parameter is IViewModel)
                {
                    Binding datacontextBinding = new Binding
                    {
                        Source = parameter as IViewModel,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    BindingOperations.SetBinding(result as View, FrameworkElement.DataContextProperty, datacontextBinding);
                }

                return result;
            }
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in NavigationService");
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
                }

                _pages.Add(key, pageType);
            }
        }

        public string GetNameOfRegisteredPage(Type page)
        {
            if (_pages.ContainsValue(page))
            {
                return _pages.FirstOrDefault(p => p.Value == page).Key;
            }
            else
            {
                throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
            }
        }

        public async Task CleanBackStackAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        ((Frame)Frame).BackStack.Clear();
                    });
        }

        //private void OnFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        //{
        //    SetBackButtonVisibility();
        //}

        //private void SetBackButtonVisibility()
        //{
        //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = CanGoBack ?
        //                    AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        //}
    }
}
