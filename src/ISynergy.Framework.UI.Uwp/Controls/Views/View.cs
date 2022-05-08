using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class View.
    /// Implements the <see cref="Page" />
    /// Implements the <see cref="IView" />
    /// </summary>
    /// <seealso cref="Page" />
    /// <seealso cref="IView" />
    public abstract partial class View : Page
    {
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
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page. Usually the most relevant property to examine is Parameter.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Content is IView view && e.Parameter is IViewModel viewModel)
                view.ViewModel = viewModel;

            if (ViewModel is not null)
                await ViewModel.OnActivateAsync(e.Parameter, e.NavigationMode == NavigationMode.Back);
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatedFrom" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (ViewModel is not null)
                await ViewModel.OnDeactivateAsync();
        }
    }
}
