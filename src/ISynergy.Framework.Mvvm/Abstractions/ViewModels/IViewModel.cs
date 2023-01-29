using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IViewModel
    /// Implements the <see cref="IObservableClass" />
    /// Implements the <see cref="ICleanup" />
    /// </summary>
    /// <seealso cref="IObservableClass" />
    /// <seealso cref="ICleanup" />
    public interface IViewModel : IObservableClass, ICleanup
    {
        /// <summary>
        /// Occurs when [cancelled].
        /// </summary>
        event EventHandler Cancelled;
        /// <summary>
        /// Occurs when [closed].
        /// </summary>
        event EventHandler Closed;
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        IContext Context { get; }
        /// <summary>
        /// Gets the base common services.
        /// </summary>
        /// <value>The base common services.</value>
        IBaseCommonServices BaseCommonServices { get; }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        ILogger Logger { get; }
        /// <summary>
        /// Gets the close _command.
        /// </summary>
        /// <value>The close _command.</value>
        RelayCommand Close_Command { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance can close.
        /// </summary>
        /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
        bool CanClose { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is cancelled.
        /// </summary>
        /// <value><c>true</c> if this instance is cancelled; otherwise, <c>false</c>.</value>
        bool IsCancelled { get; }
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }
        /// <summary>
        /// Called when [deactivate asynchronous].
        /// </summary>
        /// <returns>Task.</returns>
        Task OnDeactivateAsync();
        /// <summary>
        /// Called when [activate asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="isBack">if set to <c>true</c> [is back].</param>
        /// <returns>Task.</returns>
        Task OnActivateAsync(object parameter, bool isBack);
        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task InitializeAsync();
        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        bool IsInitialized { get; set; }
        /// <summary>
        /// Gets or sets a value indicating that viewmodel is refreshing.
        /// </summary>
        bool IsRefreshing { get; set; }
        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}
