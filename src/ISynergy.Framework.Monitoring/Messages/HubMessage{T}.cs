namespace ISynergy.Framework.Monitoring.Messages;

/// <summary>
/// Class HubMessage.
/// </summary>
public class HubMessage<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HubMessage{T}" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="data">The data.</param>
    public HubMessage(string message, T data)
    {
        Message = message;
        Data = data;
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; }
    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <value>The data.</value>
    public T Data { get; }
}
