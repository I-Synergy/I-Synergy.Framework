using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IBusyService
    /// </summary>
    public interface IBusyService
    {
        /// <summary>
        /// Starts the busy asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task StartBusyAsync();
        /// <summary>
        /// Starts the busy asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        Task StartBusyAsync(string message);
        /// <summary>
        /// Ends the busy asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task EndBusyAsync();
        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        /// <value>The busy message.</value>
        string BusyMessage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        bool IsBusy { get; set; }
    }
}
