﻿using System;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Framework.Windows.Extensions
{
    public static class FrameExtensions
    {
        /// <summary>
        /// Navigates to a page and returns the instance of the page if it succeeded, otherwise returns null.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="transitionInfo">The navigation transition.
        /// Example: <see cref="DrillInNavigationTransitionInfo"/> or <see cref="SlideNavigationTransitionInfo"/></param>
        /// <returns></returns>
        public static IView NavigateToView(this Frame frame, Type sourcePageType, object parameter, NavigationTransitionInfo transitionInfo = null)
        {
            IView navigationalView = default;

            void OnNavigated(object s, NavigationEventArgs e)
            {
                frame.Navigated -= OnNavigated;
                navigationalView = e.Content as IView;

                if(e.Parameter is IViewModel viewModel)
                    navigationalView.DataContext = viewModel;
            }

            frame.Navigated += OnNavigated;
            frame.Navigate(sourcePageType, parameter, transitionInfo);
            return navigationalView;
        }
    }
}
