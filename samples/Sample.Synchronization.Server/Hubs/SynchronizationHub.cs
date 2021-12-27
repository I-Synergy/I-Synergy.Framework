using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Monitoring.Messages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Sample.Synchronization.Common.Abstractions;
using Sample.Synchronization.Common.Options;

namespace Sample.Synchronization.Server.Hubs
{
    public class SynchronizationHub : Hub, ISynchronizationHub
    {
        private readonly ServerSynchronizationOptions _options;
        private readonly ILogger _logger;

        /// <summary>
        /// The connected event.
        /// </summary>
        public const string Connected = nameof(Connected);

        /// <summary>
        /// The disconnected event.
        /// </summary>
        public const string Disconnected = nameof(Disconnected);

        /// <summary>
        /// Constructor of MonitorHub.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SynchronizationHub(IOptions<ServerSynchronizationOptions> options, ILogger<SynchronizationHub> logger)
        {
            Argument.IsNotNull(options.Value);

            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Dummy method for sending messages.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            await Clients
                .OthersInGroup(_options.Channel)
                .SendAsync(nameof(ISynchronizationHub.SendMessageAsync), message);

            _logger.LogInformation(message);
        }

        /// <summary>
        /// For example: in a chat application, record the association between
        /// the current connection ID and user name, and mark the user as online.
        /// After the code in this method completes, the client is informed that
        /// the connection is established; for example, in a JavaScript client,
        /// the start().done callback is executed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task OnConnectedAsync()
        {
            // add user to Test group.
            await Groups.AddToGroupAsync(Context.ConnectionId, _options.Channel);

            // send other users notification that this uses has connected.
            await Clients.OthersInGroup(_options.Channel).SendAsync(
                nameof(Connected),
                new HubMessage<string>(Context.ConnectionId));
        }

        /// <summary>
        /// For example: in a chat application, mark the user as off line,
        /// delete the association between the current connection id and user name.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // remove user from Test group.
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _options.Channel);

            // send other users notification that this uses has left.
            await Clients.OthersInGroup(_options.Channel).SendAsync(
                nameof(Disconnected),
                new HubMessage<string>(Context.ConnectionId));
        }
    }
}
