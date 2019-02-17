using ISynergy.Helpers;
using ISynergy.Mvvm;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Controls.Views
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

        public View()
        {
            WeakViewLoadedEvent = new WeakEventListener<IView, object, RoutedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.View_Loaded(source, eventargs),
                OnDetachAction = (listener) => this.Loaded -= listener.OnEvent
            };

            this.Loaded += WeakViewLoadedEvent.OnEvent;

            WeakViewUnloadedEvent = new WeakEventListener<IView, object, RoutedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.View_Unloaded(source, eventargs),
                OnDetachAction = (listener) => this.Unloaded -= listener.OnEvent
            };
            
            this.Unloaded += WeakViewUnloadedEvent.OnEvent;
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
            await DataContext?.OnDeactivateAsync();
        }
    }
}
