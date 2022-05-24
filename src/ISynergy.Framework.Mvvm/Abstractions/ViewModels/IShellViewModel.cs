using ISynergy.Framework.Mvvm.Commands;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IShellViewModel
    /// Implements the <see cref="IViewModel" />
    /// </summary>
    /// <seealso cref="IViewModel" />
    public interface IShellViewModel : IViewModel
    {
        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        Task InitializeAsync(object parameter);
        /// <summary>
        /// Processes the authentication request asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task ProcessAuthenticationRequestAsync();
        /// <summary>
        /// Gets or sets the settings command.
        /// </summary>
        /// <value>The settings command.</value>
        Command Settings_Command { get; set; }
    }
}
