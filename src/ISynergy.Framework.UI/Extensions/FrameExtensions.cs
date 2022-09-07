using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class FrameExtensions.
    /// </summary>
    public static class FrameExtensions
    {
        /// <summary>
        /// Navigates to a page and returns the instance of the page if it succeeded, otherwise returns null.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="sourcePageType">Type of the source page.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="transitionInfo">The navigation transition.
        /// Example: <see cref="DrillInNavigationTransitionInfo" /> or <see cref="SlideNavigationTransitionInfo" /></param>
        /// <returns>IView.</returns>
        public static IView NavigateToView(this Frame frame, Type sourcePageType, object parameter, NavigationTransitionInfo transitionInfo = null)
        {
            IView navigationalView = default;

            void OnNavigated(object s, NavigationEventArgs e)
            {
                frame.Navigated -= OnNavigated;
                navigationalView = e.Content as IView;

                if (e.Parameter is IViewModel viewModel)
                    navigationalView.ViewModel = viewModel;
            }

            frame.Navigated += OnNavigated;
            frame.Navigate(sourcePageType, parameter, transitionInfo);
            return navigationalView;
        }
    }
}
