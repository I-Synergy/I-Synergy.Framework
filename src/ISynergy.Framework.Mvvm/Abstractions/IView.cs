using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions
{
    /// <summary>
    /// Interface IView
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        IViewModel ViewModel { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }
        ///// <summary>
        ///// Views the unloaded.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The e.</param>
        //void View_Unloaded(object sender, object e);
        ///// <summary>
        ///// Views the loaded.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The e.</param>
        //void View_Loaded(object sender, object e);
    }
}
