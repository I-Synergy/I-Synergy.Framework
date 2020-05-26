﻿using ISynergy.Framework.Windows.Helpers;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.Windows.Adapters
{
    public class FrameAdapter : IFrameAdapter
    {
        private readonly Frame _internalFrame;

        public event NavigatedEventHandler Navigated { add => _internalFrame.Navigated += value; remove => _internalFrame.Navigated -= value; }

        public event NavigatingCancelEventHandler Navigating { add => _internalFrame.Navigating += value; remove => _internalFrame.Navigating -= value; }

        public event NavigationFailedEventHandler NavigationFailed { add => _internalFrame.NavigationFailed += value; remove => _internalFrame.NavigationFailed -= value; }

        public event NavigationStoppedEventHandler NavigationStopped { add => _internalFrame.NavigationStopped += value; remove => _internalFrame.NavigationStopped -= value; }

        public FrameAdapter(Frame internalFrame)
        {
            _internalFrame = internalFrame;
        }

        public bool IsNavigating { get; }

        public bool CanGoBack => _internalFrame.CanGoBack;

        public bool CanGoForward => _internalFrame.CanGoForward;

        public object Content => _internalFrame.Content;

        public void GoForward()
        {
            _internalFrame.GoForward();
        }

        public void GoBack() => _internalFrame.GoBack();

        public string GetNavigationState() => _internalFrame.GetNavigationState();

        public void SetNavigationState(string navigationState) => _internalFrame.SetNavigationState(navigationState);

        public bool Navigate(Type sourcePageType, object parameter)
        {
            return _internalFrame.NavigateWithFadeOutgoing(parameter, sourcePageType);
        }
    }
}
