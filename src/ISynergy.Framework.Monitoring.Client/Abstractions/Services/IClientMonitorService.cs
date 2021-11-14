namespace ISynergy.Framework.Monitoring.Client.Abstractions.Services
{
    /// <summary>
    /// Interface IClientMonitorService
    /// </summary>
    public interface IClientMonitorService
    {
        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        Task ConnectAsync(string token);
        /// <summary>
        /// Disconnects the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Occurs when [refresh UI].
        /// </summary>
        event EventHandler RefreshUI;
        /// <summary>
        /// Occurs when [position caller identifier phone selected].
        /// </summary>
        event EventHandler<CallerMessage> POSCallerIdPhoneSelected;
    }
}
