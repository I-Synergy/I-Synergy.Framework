using ISynergy.Framework.Monitoring.Client.Abstractions.Services;
using ISynergy.Framework.Monitoring.Enumerations;
using ISynergy.Framework.Monitoring.Messages;
using ISynergy.Framework.Monitoring.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Monitoring.Client.Services;

/// <summary>
/// Class ClientMonitorService.
/// </summary>
public abstract class BaseClientMonitorService : IClientMonitorService
{
    protected readonly ILogger _logger;
    protected readonly ClientMonitorOptions _clientMonitorOptions;
    protected HubConnection? _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseClientMonitorService"/> class.
    /// </summary>
    /// <param name="clientMonitorOptions">The configuration options.</param>
    /// <param name="logger">The logger factory.</param>
    protected BaseClientMonitorService(
        IOptions<ClientMonitorOptions> clientMonitorOptions,
        ILogger<BaseClientMonitorService> logger)
    {
        _clientMonitorOptions = clientMonitorOptions.Value;
        _logger = logger;
    }

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
    public virtual Task ConnectAsync(string? token, Action<HubConnection> connectionAction)
    {
        _logger.LogInformation($"Connecting to {_clientMonitorOptions.EndpointUrl}");

        _connection = new HubConnectionBuilder()
            .WithUrl(_clientMonitorOptions.EndpointUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .Build();

        _connection.On<HubMessage>(nameof(MonitorEvents.Connected), OnConnected);

        _connection.On<HubMessage>(nameof(MonitorEvents.Disconnected), OnDisconnected);

        // Set up additional handlers
        connectionAction.Invoke(_connection);

        return _connection.StartAsync();
    }

    public abstract void OnDisconnected(HubMessage message);

    public abstract void OnConnected(HubMessage message);

    /// <summary>
    /// disconnect as an asynchronous operation.
    /// </summary>
    public virtual async Task DisconnectAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
