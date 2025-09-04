using ISynergy.Framework.Monitoring.Messages;
using Microsoft.AspNetCore.SignalR.Client;

namespace ISynergy.Framework.Monitoring.Client.Abstractions.Services;

/// <summary>
/// Interface IClientMonitorService
/// </summary>
public interface IClientMonitorService
{
    /// <summary>
    /// Connects the asynchronous.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="connectionAction"></param>
    /// <returns>Task.</returns>
    Task ConnectAsync(string token, Action<HubConnection> connectionAction);
    /// <summary>
    /// Disconnects the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task DisconnectAsync();

    void OnConnected(HubMessage message);
    void OnDisconnected(HubMessage message);
}
