using System;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.UI.Adapters
{
    /// <summary>
    /// Interface IFrameAdapter
    /// </summary>
    public interface IFrameAdapter
    {
        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        bool CanGoBack { get; }
        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        bool CanGoForward { get; }
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        object Content { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is navigating.
        /// </summary>
        /// <value><c>true</c> if this instance is navigating; otherwise, <c>false</c>.</value>
        bool IsNavigating { get; }

        /// <summary>
        /// Occurs when [navigated].
        /// </summary>
        event NavigatedEventHandler Navigated;
        /// <summary>
        /// Occurs when [navigating].
        /// </summary>
        event NavigatingCancelEventHandler Navigating;
        /// <summary>
        /// Occurs when [navigation failed].
        /// </summary>
        event NavigationFailedEventHandler NavigationFailed;
        /// <summary>
        /// Occurs when [navigation stopped].
        /// </summary>
        event NavigationStoppedEventHandler NavigationStopped;

        /// <summary>
        /// Gets the state of the navigation.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetNavigationState();
        /// <summary>
        /// Goes the back.
        /// </summary>
        void GoBack();
        /// <summary>
        /// Goes the forward.
        /// </summary>
        void GoForward();
        /// <summary>
        /// Navigates the specified source page type.
        /// </summary>
        /// <param name="sourcePageType">Type of the source page.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Navigate(Type sourcePageType, object parameter);
        /// <summary>
        /// Sets the state of the navigation.
        /// </summary>
        /// <param name="navigationState">State of the navigation.</param>
        void SetNavigationState(string navigationState);
    }
}
