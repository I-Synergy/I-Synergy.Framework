using ISynergy.Framework.Monitoring.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics.CodeAnalysis;

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
    /// <param name="connectionAction">An action to configure additional SignalR connection handlers.</param>
    /// <returns>Task.</returns>
    /// <remarks>
    /// This method uses <see cref="HubConnectionBuilder"/> which may require unreferenced code
    /// for internal SignalR message deserialization. In AOT-published applications, configure
    /// the JSON protocol with a source-generated <c>JsonSerializerContext</c> that includes
    /// <see cref="HubMessage"/> via the <paramref name="connectionAction"/> callback.
    /// </remarks>
    [RequiresUnreferencedCode("SignalR HubConnectionBuilder may use reflection for message deserialization. Configure AddJsonProtocol with a JsonSerializerContext for AOT compatibility.")]
    Task ConnectAsync(string? token, Action<HubConnection> connectionAction);
    /// <summary>
    /// Disconnects the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task DisconnectAsync();

    void OnConnected(HubMessage message);
    void OnDisconnected(HubMessage message);
}
