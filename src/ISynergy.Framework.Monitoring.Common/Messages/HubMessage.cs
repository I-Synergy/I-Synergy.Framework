namespace ISynergy.Framework.Messages
{
    /// <summary>
    /// Class HubMessage.
    /// </summary>
    public class HubMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HubMessage" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="data">The data.</param>
        public HubMessage(string message, object data = null)
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
        public object Data { get; }
    }
}
