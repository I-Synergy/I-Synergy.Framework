using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

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
        /// <returns>IView.</returns>
        public static IView NavigateToView(this Frame frame, Type sourcePageType, object parameter)
        {
            IView navigationalView = default;
            navigationalView = Activator.CreateInstance(sourcePageType) as IView;

            if (parameter is IViewModel viewModel)
                navigationalView.DataContext = viewModel;

            frame.Navigate(navigationalView, parameter);
            return navigationalView;
        }
    }
}
