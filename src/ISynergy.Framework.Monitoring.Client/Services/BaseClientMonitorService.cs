using ISynergy.Framework.Monitoring.Client.Abstractions.Services;
using ISynergy.Framework.Monitoring.Enumerations;
using ISynergy.Framework.Monitoring.Messages;
using ISynergy.Framework.Monitoring.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    /// <param name="connectionAction"></param>
    /// <returns>Task.</returns>
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
