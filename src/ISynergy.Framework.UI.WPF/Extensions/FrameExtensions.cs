using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Extensions;

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
    public static IView? NavigateToView(this Frame frame, Type sourcePageType, object parameter)
    {
        var navigationalView = Activator.CreateInstance(sourcePageType) as IView;

        if (navigationalView is null)
            return null;

        if (parameter is IViewModel viewModel)
            navigationalView.ViewModel = viewModel;

        frame.Navigate(navigationalView, parameter);

        return navigationalView;
    }
}
