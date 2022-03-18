using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class Window.
    /// Implements the <see cref="Mvvm.Abstractions.IWindow" />
    /// </summary>
    /// <seealso cref="Mvvm.Abstractions.IWindow" />
    [Bindable(true)]
    public partial class Window : IWindow
    {
        /// <summary>
        /// Gets or sets the data context for a FrameworkElement. A common use of a data context is when a **FrameworkElement** uses the {Binding} markup extension and participates in data binding.
        /// </summary>
        /// <value>The data context.</value>
        public new IViewModel DataContext
        {
            get => base.DataContext as IViewModel;
            set => base.DataContext = value;
        }
    }
}
