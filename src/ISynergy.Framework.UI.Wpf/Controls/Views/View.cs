using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Mvvm.Abstractions;
using System.Windows;
using System.Windows.Controls;

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
    }
}
