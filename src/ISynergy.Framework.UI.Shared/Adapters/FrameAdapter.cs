using ISynergy.Framework.UI.Helpers;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.UI.Adapters
{
    /// <summary>
    /// Class FrameAdapter.
    /// Implements the <see cref="IFrameAdapter" />
    /// </summary>
    /// <seealso cref="IFrameAdapter" />
    public class FrameAdapter : IFrameAdapter
    {
        /// <summary>
        /// The internal frame
        /// </summary>
        private readonly Frame _internalFrame;

        /// <summary>
        /// Occurs when [navigated].
        /// </summary>
        public event NavigatedEventHandler Navigated { add => _internalFrame.Navigated += value; remove => _internalFrame.Navigated -= value; }

        /// <summary>
        /// Occurs when [navigating].
        /// </summary>
        public event NavigatingCancelEventHandler Navigating { add => _internalFrame.Navigating += value; remove => _internalFrame.Navigating -= value; }

        /// <summary>
        /// Occurs when [navigation failed].
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed { add => _internalFrame.NavigationFailed += value; remove => _internalFrame.NavigationFailed -= value; }

        /// <summary>
        /// Occurs when [navigation stopped].
        /// </summary>
        public event NavigationStoppedEventHandler NavigationStopped { add => _internalFrame.NavigationStopped += value; remove => _internalFrame.NavigationStopped -= value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameAdapter"/> class.
        /// </summary>
        /// <param name="internalFrame">The internal frame.</param>
        public FrameAdapter(Frame internalFrame)
        {
            _internalFrame = internalFrame;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is navigating.
        /// </summary>
        /// <value><c>true</c> if this instance is navigating; otherwise, <c>false</c>.</value>
        public bool IsNavigating { get; }

        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack => _internalFrame.CanGoBack;

        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        public bool CanGoForward => _internalFrame.CanGoForward;

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content => _internalFrame.Content;

        /// <summary>
        /// Goes the forward.
        /// </summary>
        public void GoForward()
        {
            _internalFrame.GoForward();
        }

        /// <summary>
        /// Goes the back.
        /// </summary>
        public void GoBack() => _internalFrame.GoBack();

        /// <summary>
        /// Gets the state of the navigation.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetNavigationState() => _internalFrame.GetNavigationState();

        /// <summary>
        /// Sets the state of the navigation.
        /// </summary>
        /// <param name="navigationState">State of the navigation.</param>
        public void SetNavigationState(string navigationState) => _internalFrame.SetNavigationState(navigationState);

        /// <summary>
        /// Navigates the specified source page type.
        /// </summary>
        /// <param name="sourcePageType">Type of the source page.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Navigate(Type sourcePageType, object parameter)
        {
            return _internalFrame.NavigateWithFadeOutgoing(parameter, sourcePageType);
        }
    }
}
