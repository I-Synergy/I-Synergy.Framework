using ISynergy.Framework.Mvvm.Commands;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IViewModelSelectedItem
    /// Implements the <see cref="IViewModel" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="IViewModel" />
    public interface IViewModelSelectedItem<TEntity> : IViewModel 
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        bool IsNew { get; set; }
        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void SetSelectedItem(TEntity entity);
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        TEntity SelectedItem { get; }
        /// <summary>
        /// Gets the submit command.
        /// </summary>
        /// <value>The submit command.</value>
        Command<TEntity> Submit_Command { get; }
    }
}
