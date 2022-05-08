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
        private IViewModel _viewModel;

        /// <summary>
        /// Gets or sets the viewmodel and data context for a window.
        /// </summary>
        /// <value>The data context.</value>
        public IViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }
    }
}
