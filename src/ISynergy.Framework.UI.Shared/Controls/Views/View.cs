using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;

#if WINDOWS_UWP || HAS_UNO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#elif WINDOWS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
#elif WINDOWS_WPF
using System.Windows;
using System.Windows.Controls;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class View.
    /// Implements the <see cref="IView" />
    /// </summary>
    /// <seealso cref="IView" />
    [Bindable(true)]
    public abstract partial class View : Page, IView
    {
        private IViewModel _viewModel;

        /// <summary>
        /// Gets or sets the viewmodel and data context for a view.
        /// </summary>
        /// <value>The data context.</value>
        public IViewModel ViewModel { 
            get => _viewModel; 
            set {
                _viewModel = value;
                DataContext = _viewModel;
            } 
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

#if !WINDOWS_WPF
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
#endif

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
    }
}
