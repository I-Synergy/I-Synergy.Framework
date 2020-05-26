using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Framework.Windows.Actions
{
    /// <summary>
    /// Class CloseFlyoutAction.
    /// Implements the <see cref="Windows.UI.Xaml.DependencyObject" />
    /// Implements the <see cref="IAction" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.DependencyObject" />
    /// <seealso cref="IAction" />
    public class CloseFlyoutAction : DependencyObject, IAction
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
