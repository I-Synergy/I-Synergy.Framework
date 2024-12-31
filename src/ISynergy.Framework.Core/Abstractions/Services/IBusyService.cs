using System.ComponentModel;

namespace ISynergy.Framework.Core.Abstractions.Services;

/// <summary>
/// Interface IBusyService
/// </summary>
public interface IBusyService : INotifyPropertyChanged
{
    /// <summary>
    /// Starts the busy.
    /// </summary>
    /// <param name="message">The message.</param>
    void StartBusy(string message = null);
    /// <summary>
    /// Ends the busy.
    /// </summary>
    void StopBusy();
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
