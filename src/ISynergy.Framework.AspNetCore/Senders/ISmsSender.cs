namespace ISynergy.Framework.AspNetCore.Senders
{
    /// <summary>
    /// Interface ISmsSender
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// Sends the SMS asynchronous.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        Task SendSmsAsync(string number, string message);
    }
}
