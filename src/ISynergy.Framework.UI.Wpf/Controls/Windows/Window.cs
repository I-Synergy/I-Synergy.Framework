using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class Window.
    /// Implements the <see cref="Mvvm.Abstractions.IWindow" />
    /// </summary>
    /// <seealso cref="Mvvm.Abstractions.IWindow" />
    public partial class Window : System.Windows.Window, IWindow
    {
        /// <summary>
        /// Gets the name of the descendant from.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns>FrameworkElement.</returns>
        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (var i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name)
                    {
                        return frameworkElement;
                    }

                    frameworkElement = GetDescendantFromName(frameworkElement, name);

                    if (frameworkElement is not null)
                    {
                        return frameworkElement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// show as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public Task<bool> ShowAsync<TEntity>()
        {
            var result = base.ShowDialog();

            if ((result.HasValue && result.Value) || (ViewModel is IViewModelDialog<TEntity> dataContext && !dataContext.IsCancelled))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
