using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.ComponentModel;

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class View.
    /// Implements the <see cref="IView" />
    /// </summary>
    /// <seealso cref="IView" />
    [Bindable(true)]
    public abstract partial class View : IView
    {
        private IViewModel _viewModel;

        /// <summary>
        /// Gets or sets the viewmodel and data context for a view.
        /// </summary>
        /// <value>The data context.</value>
        public IViewModel ViewModel { 
            get => _viewModel; 
            set {
                _viewModel = value;
                DataContext = _viewModel;
            } 
        }

        /// <summary>
        /// Views the unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public virtual void View_Unloaded(object sender, object e)
        {
            //if(e is RoutedEventArgs args)
            //{
            //}
        }

        /// <summary>
        /// Views the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public virtual void View_Loaded(object sender, object e)
        {
            //if (e is RoutedEventArgs args)
            //{
            //}
        }
    }
}
