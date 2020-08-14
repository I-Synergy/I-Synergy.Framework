using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.Windows.Controls
{
    /// <summary>
    /// Class View.
    /// Implements the <see cref="Windows.UI.Xaml.Controls.Page" />
    /// Implements the <see cref="IView" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    /// <seealso cref="IView" />
    public abstract class View : Page, IView
    {
        /// <summary>
        /// Gets or sets the data context for a FrameworkElement. A common use of a data context is when a **FrameworkElement** uses the {Binding} markup extension and participates in data binding.
        /// </summary>
        /// <value>The data context.</value>
        public new IViewModel DataContext
        {
            get { return base.DataContext as IViewModel; }
            set { base.DataContext = value; }
        }

        /// <summary>
        /// The weak view loaded event
        /// </summary>
        private readonly WeakEventListener<IView, object, RoutedEventArgs> WeakViewLoadedEvent = null;
        /// <summary>
        /// The weak view unloaded event
        /// </summary>
        private readonly WeakEventListener<IView, object, RoutedEventArgs> WeakViewUnloadedEvent = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
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

        /// <summary>
        /// Views the unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public virtual void View_Unloaded(object sender, object e)
        {
            //if(e is RoutedEventArgs args)
            //{
            //}
        }

        /// <summary>
        /// Views the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public virtual void View_Loaded(object sender, object e)
        {
            //if (e is RoutedEventArgs args)
            //{
            //}
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page. Usually the most relevant property to examine is Parameter.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await DataContext?.OnActivateAsync(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatedFrom" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
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
