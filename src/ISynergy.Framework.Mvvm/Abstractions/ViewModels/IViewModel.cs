using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Mvvm.Commands;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModel
/// Implements the <see cref="IObservableValidatedClass" />
/// Implements the <see cref="ICleanup" />
/// </summary>
/// <seealso cref="IObservableValidatedClass" />
/// <seealso cref="ICleanup" />
public interface IViewModel : IObservableValidatedClass, ICleanup
{
    /// <summary>
    /// Occurs when [cancelled].
    /// </summary>
    event EventHandler? Cancelled;
    void OnCancelled(EventArgs e);

    /// <summary>
    /// Occurs when [closed].
    /// </summary>
    event EventHandler? Closed;
    void OnClosed(EventArgs e);

    /// <summary>
    /// Gets the close _command.
    /// </summary>
    /// <value>The close _command.</value>
    AsyncRelayCommand? CloseCommand { get; }

    /// <summary>
    /// /// Gets or sets the cancel command.
    /// </summary>
    AsyncRelayCommand? CancelCommand { get; }

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
    void OnPropertyChanged(object? sender, PropertyChangedEventArgs e);

    /// <summary>
    /// Gets or sets an optional parameter.
    /// </summary>
    object Parameter { get; set; }

    void OnNavigatedFrom();
    void OnNavigatedTo();
}
