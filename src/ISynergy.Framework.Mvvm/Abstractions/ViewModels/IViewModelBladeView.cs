namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IViewModelBladeView
    /// Implements the <see cref="IViewModel" />
    /// </summary>
    /// <seealso cref="IViewModel" />
    public interface IViewModelBladeView : IViewModel
    {
        /// <summary>
        /// Gets or sets the blades.
        /// </summary>
        /// <value>The blades.</value>
        ObservableCollection<IView> Blades { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is pane enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is pane enabled; otherwise, <c>false</c>.</value>
        bool IsPaneVisible { get; set; }
    }
}
