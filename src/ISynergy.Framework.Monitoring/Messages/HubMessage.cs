namespace ISynergy.Framework.Monitoring.Messages;

/// <summary>
/// Class HubMessage.
/// </summary>
public class HubMessage : HubMessage<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HubMessage" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="data">The data.</param>
    public HubMessage(string message, object data = null)
        : base(message, data) { }
}
