#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Behaviors.Base
{
    /// <summary>
    /// Base class for behaviors that solves 2 problems:
    /// 1. Prevent duplicate initialization that can happen (prevent multiple OnAttached calls);
    /// 2. Whenever <see cref="Initialize" /> initially fails, this method will subscribe to <see cref="FrameworkElement.SizeChanged" /> to allow lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the associated object.</typeparam>
    /// <seealso cref="Behavior{T}" />
    /// <remarks>For more info, see https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/1008.</remarks>
    public abstract class BehaviorBase<T> : Behavior<T>
        where T : UIElement
    {
        /// <summary>
        /// The is attaching
        /// </summary>
        private bool _isAttaching;
        /// <summary>
        /// The is attached
        /// </summary>
        private bool _isAttached;

        /// <summary>
        /// Gets a value indicating whether this behavior is attached.
        /// </summary>
        /// <value><c>true</c> if this behavior is attached; otherwise, <c>false</c>.</value>
        protected bool IsAttached
        {
            get { return _isAttached; }
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="P:Behavior.AssociatedObject" />.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the <see cref="P:Behavior.AssociatedObject" /></remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            HandleAttach();

            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded += OnAssociatedObjectLoaded;
                frameworkElement.Unloaded += OnAssociatedObjectUnloaded;
                frameworkElement.SizeChanged += OnAssociatedObjectSizeChanged;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="P:Behavior.AssociatedObject" />.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the <see cref="P:Behavior.AssociatedObject" /></remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded -= OnAssociatedObjectLoaded;
                frameworkElement.Unloaded -= OnAssociatedObjectUnloaded;
                frameworkElement.SizeChanged -= OnAssociatedObjectSizeChanged;
            }

            HandleDetach();
        }

        /// <summary>
        /// Called when the associated object has been loaded.
        /// </summary>
        protected virtual void OnAssociatedObjectLoaded()
        {
        }

        /// <summary>
        /// Called when the associated object has been unloaded.
        /// </summary>
        protected virtual void OnAssociatedObjectUnloaded()
        {
        }

        /// <summary>
        /// Initializes the behavior to the associated object.
        /// </summary>
        /// <returns><c>true</c> if the initialization succeeded; otherwise <c>false</c>.</returns>
        protected virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Uninitializes the behavior from the associated object.
        /// </summary>
        /// <returns><c>true</c> if uninitialization succeeded; otherwise <c>false</c>.</returns>
        protected virtual bool Uninitialize()
        {
            return true;
        }

        /// <summary>
        /// Handles the <see cref="E:AssociatedObjectLoaded" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isAttached)
            {
                HandleAttach();
            }

            OnAssociatedObjectLoaded();
        }

        /// <summary>
        /// Handles the <see cref="E:AssociatedObjectUnloaded" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            OnAssociatedObjectUnloaded();

            // Note: don't detach here, we'll let the behavior implementation take care of that
        }

        /// <summary>
        /// Handles the <see cref="E:AssociatedObjectSizeChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isAttached)
            {
                HandleAttach();
            }
        }

        /// <summary>
        /// Handles the attach.
        /// </summary>
        private void HandleAttach()
        {
            if (_isAttaching || _isAttached)
            {
                return;
            }

            _isAttaching = true;

            var attached = Initialize();
            if (attached)
            {
                _isAttached = true;
            }

            _isAttaching = false;
        }

        /// <summary>
        /// Handles the detach.
        /// </summary>
        private void HandleDetach()
        {
            if (!_isAttached)
            {
                return;
            }

            var detached = Uninitialize();
            if (detached)
            {
                _isAttached = false;
            }
        }
    }
}
