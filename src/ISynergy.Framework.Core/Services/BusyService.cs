using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// Class BusyService.
/// Implements the <see cref="IBusyService" />
/// </summary>
/// <seealso cref="IBusyService" />
public sealed class BusyService : ObservableClass, IBusyService
{
    private readonly ILanguageService _languageService;
    private static readonly object _creationLock = new object();
    private static IBusyService? _defaultInstance;

    /// <summary>
    /// Gets the LanguageService's default instance.
    /// </summary>
    public static IBusyService Default
    {
        get
        {
            if (_defaultInstance is null)
            {
                lock (_creationLock)
                {
                    if (_defaultInstance is null)
                    {
                        _defaultInstance = new BusyService(new LanguageService());
                    }
                }
            }

            return _defaultInstance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusyService"/> class.
    /// </summary>
    /// <param name="languageService">The language service.</param>
    public BusyService(ILanguageService languageService)
    {
        _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
    }

    /// <summary>
    /// Gets or sets the IsBusy property value.
    /// </summary>
    public bool IsBusy
    {
        get { return GetValue<bool>(); }
        private set
        {
            SetValue(value);
            IsEnabled = !value;
        }
    }

    /// <summary>
    /// Gets or sets the IsEnabled property value.
    /// </summary>
    /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
    public bool IsEnabled
    {
        get { return GetValue<bool>(); }
        private set { SetValue(value); }
    }

    /// <summary>
    /// Gets or sets the BusyMessage property value.
    /// </summary>
    /// <value>The busy message.</value>
    public string BusyMessage
    {
        get { return GetValue<string>(); }
        private set { SetValue(value); }
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
            BusyMessage = _languageService.GetString("PleaseWait");

        IsBusy = true;
    }

    /// <summary>
    /// Updates the busy message.
    /// </summary>
    /// <param name="message"></param>
    public void UpdateMessage(string message) => BusyMessage = message;

    /// <summary>
    /// Ends the busy asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public void StopBusy() => IsBusy = false;
}
