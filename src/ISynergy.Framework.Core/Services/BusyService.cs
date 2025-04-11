using ISynergy.Framework.Core.Abstractions.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Class BusyService.
/// Implements the <see cref="IBusyService" />
/// </summary>
/// <seealso cref="IBusyService" />
public class BusyService : IBusyService
{
    private bool _isBusy = false;
    private bool _isEnabled;
    private string _busyMessage = string.Empty;

    /// <summary>
    /// Gets or sets the IsBusy property value.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                IsEnabled = !value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the IsEnabled property value.
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    /// <summary>
    /// Gets or sets the BusyMessage property value.
    /// </summary>
    /// <value>The busy message.</value>
    public string BusyMessage
    {
        get => _busyMessage;
        set
        {
            if (_busyMessage != value)
            {
                _busyMessage = value;
                OnPropertyChanged(nameof(BusyMessage));
            }
        }
    }

    /// <summary>
    /// Starts the busy asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>Task.</returns>
    public void StartBusy(string? message = null)
    {
        if (!string.IsNullOrEmpty(message))
            BusyMessage = message;
        else
            BusyMessage = LanguageService.Default.GetString("PleaseWait");

        IsBusy = true;
    }

    /// <summary>
    /// Ends the busy asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public void StopBusy() => IsBusy = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
