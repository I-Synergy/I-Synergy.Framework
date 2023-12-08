using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Models;
using ISynergy.Framework.Monitoring.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Monitoring.Hubs;

/// <summary>
/// Monitor hub class.
/// </summary>
[Authorize(AuthenticationSchemes = "OpenIddict.Validation.AspNetCore")]
internal class MonitorHub : Hub
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor of MonitorHub.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public MonitorHub(ILogger<MonitorHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// For example: in a chat application, record the association between
    /// the current connection ID and user name, and mark the user as online.
    /// After the code in this method completes, the client is informed that
    /// the connection is established; for example, in a JavaScript client,
    /// the start().done callback is executed.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.</returns>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");

        var accountId = Context.User.GetAccountId();
        var userId = Context.User.GetAccountId();
        var userName = Context.User.GetUserName();

        // add user to own Group
        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
        // add user to Account Group
        await Groups.AddToGroupAsync(Context.ConnectionId, accountId.ToString());

        await Clients.OthersInGroup(accountId.ToString()).SendAsync(nameof(MonitorEvents.Connected),
            new HubMessage<string>(accountId.ToString(), MonitorEvents.Connected.ToString(), userName));
    }

    /// <summary>
    /// For example: in a chat application, mark the user as off line,
    /// delete the association between the current connection id and user name.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");

        var accountId = Context.User.GetAccountId();
        var userId = Context.User.GetAccountId();
        var userName = Context.User.GetUserName();

        // remove user own Group
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
        // remove user from Account Group
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, accountId.ToString());

        await Clients.OthersInGroup(accountId.ToString()).SendAsync(nameof(MonitorEvents.Disconnected),
            new HubMessage<string>(accountId.ToString(), MonitorEvents.Disconnected.ToString(), userName));
    }
}
