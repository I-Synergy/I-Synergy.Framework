using ISynergy.Framework.Core.Listeners;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.Windows.Controls
{
    public abstract class View : Page, IView
    {
        public new IViewModel DataContext
        {
            get { return base.DataContext as IViewModel; }
            set { base.DataContext = value; }
        }

        private readonly WeakEventListener<IView, object, RoutedEventArgs> WeakViewLoadedEvent = null;
        private readonly WeakEventListener<IView, object, RoutedEventArgs> WeakViewUnloadedEvent = null;

        protected View()
        {
            // By default, all pages are cached to increase performance
            NavigationCacheMode = NavigationCacheMode.Required;

            WeakViewLoadedEvent = new WeakEventListener<IView, object, RoutedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.View_Loaded(source, eventargs),
                OnDetachAction = (listener) => Loaded -= listener.OnEvent
            };

            Loaded += WeakViewLoadedEvent.OnEvent;

            WeakViewUnloadedEvent = new WeakEventListener<IView, object, RoutedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.View_Unloaded(source, eventargs),
                OnDetachAction = (listener) => Unloaded -= listener.OnEvent
            };

            Unloaded += WeakViewUnloadedEvent.OnEvent;
        }

        public virtual void View_Unloaded(object sender, object e)
        {
            //if(e is RoutedEventArgs args)
            //{
            //}
        }

        public virtual void View_Loaded(object sender, object e)
        {
            //if (e is RoutedEventArgs args)
            //{
            //}
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await DataContext?.OnActivateAsync(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }

            await DataContext?.OnDeactivateAsync();
        }
    }
}
