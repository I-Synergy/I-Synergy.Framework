using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class Window.
    /// Implements the <see cref="ContentDialog" />
    /// Implements the <see cref="Mvvm.Abstractions.IWindow" />
    /// </summary>
    /// <seealso cref="ContentDialog" />
    /// <seealso cref="Mvvm.Abstractions.IWindow" />
    [Bindable(true)]
    public partial class Window : ContentDialog, IWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Window()
        {
            CornerRadius = new CornerRadius(8);
        }

        /// <summary>
        /// Gets or sets the data context for a FrameworkElement. A common use of a data context is when a **FrameworkElement** uses the {Binding} markup extension and participates in data binding.
        /// </summary>
        /// <value>The data context.</value>
        public new IViewModel DataContext
        {
            get => base.DataContext as IViewModel;
            set => base.DataContext = value;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            Hide();
        }

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

                    if (frameworkElement != null)
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
        public async Task<bool> ShowAsync<TEntity>()
        {
            switch (await ShowAsync())
            {
                case ContentDialogResult.Primary:
                    return true;
                case ContentDialogResult.Secondary:
                    return false;
                default:
                    if (DataContext is IViewModelDialog<TEntity> dataContext && !dataContext.IsCancelled)
                    {
                        return true;
                    }

                    return false;
            }
        }
    }
}
