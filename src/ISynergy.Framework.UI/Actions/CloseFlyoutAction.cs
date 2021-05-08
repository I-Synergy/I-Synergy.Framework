using Microsoft.Xaml.Interactivity;

#if (__UWP__ || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif (__WINUI__)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endif

namespace ISynergy.Framework.UI.Actions
{
    /// <summary>
    /// Class CloseFlyoutAction.
    /// Implements the <see cref="UIElement" />
    /// Implements the <see cref="IAction" />
    /// </summary>
    /// <seealso cref="UIElement" />
    /// <seealso cref="IAction" />
    public partial class CloseFlyoutAction : DependencyObject, IAction
    {
        /// <summary>
        /// Executes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>System.Object.</returns>
        public object Execute(object sender, object parameter)
        {
            var flyout = sender as Flyout;
            flyout?.Hide();

            return null;
        }
    }
}
