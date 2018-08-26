using System;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Adapters
{
    public interface IFrameAdapter
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        object Content { get; }
        bool IsNavigating { get; }

        event NavigatedEventHandler Navigated;
        event NavigatingCancelEventHandler Navigating;
        event NavigationFailedEventHandler NavigationFailed;
        event NavigationStoppedEventHandler NavigationStopped;

        string GetNavigationState();
        void GoBack();
        void GoForward();
        bool Navigate(Type sourcePageType, object parameter);
        void SetNavigationState(string navigationState);
    }
}