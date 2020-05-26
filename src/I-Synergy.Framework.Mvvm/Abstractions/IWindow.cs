using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions
{
    /// <summary>
    /// Interface IWindow
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>The data context.</value>
        IViewModel DataContext { get; set; }
        /// <summary>
        /// Shows the asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ShowAsync<TEntity>();
        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();
    }
}
