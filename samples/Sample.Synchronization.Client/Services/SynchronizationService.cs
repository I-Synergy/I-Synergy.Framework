using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.SqlServer.ChangeTracking.Providers;
using Microsoft.AspNetCore.SignalR.Client;
using Sample.Synchronization.Common.Abstractions;
using Sample.Synchronization.Common.Messages;
using Sample.Synchronization.Common.Options;
using System.Net.NetworkInformation;
using Timer = System.Timers.Timer;

namespace Sample.Synchronization.Client.Services
{
    internal class SynchronizationService : ISynchronizationService
    {
        private readonly IContext _context;
        private readonly IMessageService _messageService;
        private readonly IVersionService _versionService;
        private readonly IFileSynchronizationService _fileSynchronizationService;
        private readonly ClientSynchronizationOptions _options;
        private readonly Timer _timer;
        private readonly HubConnection _connection;

        public SynchronizationService(
            IContext context,
            IMessageService messageService,
            IVersionService versionService,
            IFileSynchronizationService fileSynchronizationService,
            ClientSynchronizationOptions options)
        {
            Argument.IsNotNull(options);

            _context = context;
            _messageService = messageService;
            _versionService = versionService;
            _fileSynchronizationService = fileSynchronizationService;
            _options = options;

            _connection = new HubConnectionBuilder()
                .WithUrl($"{_options.Host}/synchronization")
                .WithAutomaticReconnect()
                .Build();

            _timer = new Timer
            {
                Interval = _options.CheckHostInterval.TotalMilliseconds,
                AutoReset = true
            };

            _timer.Elapsed += async (sender, e) =>
            {
                _timer.Stop();

                // Async task that the background worker has to perform.
                // Check if network connection is available or host available.
                // Check if serial number is set correctly.
                if (NetworkInterface.GetIsNetworkAvailable() && NetworkUtility.IsUrlReachable(_options.Host))
                {
                    // create connection to signalR service.
                    // check if signalR service is connected (do nothing, else reconnect)
                    if (!IsConnected)
                        await ConnectAsync();

                    if (IsConnected)
                    {
                        // Tester is online and able to connect.
                        MarkClientOnline();

                        //await SynchronizeAsync();
                        await SynchronizeFilesAsync();
                    }
                    else
                    {
                        MarkClientOffline();
                    }
                }
                else
                {
                    MarkClientOffline();
                }

                _timer.Start();
            };

            _timer.Start();
        }

        private bool IsConnected =>
            _connection.State == HubConnectionState.Connected;

        private Task ConnectAsync()
        {
            Console.WriteLine("Connecting to synchronization host.");
            return _connection.StartAsync();
        }

        public async Task DisconnectAsync()
        {
            Console.WriteLine("Disconnecting from synchronization host.");

            if (_connection is not null)
                await _connection.DisposeAsync();
        }

        public async Task SynchronizeAsync()
        {
            if (!_context.IsOffline)
            {
                try
                {
                    Console.WriteLine($"Synchronization started at {DateTime.Now}");

                    var serverOrchestrator = new WebClientOrchestrator($"{_options.Host}/api/sync", _versionService);
                    var clientProvider = new SqlSyncChangeTrackingProvider(_options.ConnectionString);

                    var options = new SyncOptions
                    {
                        BatchSize = 1000,
                        DisableConstraintsOnApplyChanges = true
                    };

                    // Creating an agent that will handle all the process
                    var agent = new SyncAgent(_versionService, clientProvider, serverOrchestrator, options);

                    // Launch the sync process
                    // This first sync will create all the sync architecture
                    // and will get the server rows
                    await agent.SynchronizeAsync();

                    Console.WriteLine($"Synchronization completed at {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Synchronization failed at {DateTime.Now}.{Environment.NewLine}{ex.Message}");
                }

            }
            else
            {
                Console.WriteLine("Tester is unavailable and disconnected.");
            }
        }

        public async Task SynchronizeFilesAsync()
        {
            if (!_context.IsOffline)
            {
                try
                {
                    Console.WriteLine($"File synchronization started at {DateTime.Now}");

                    await _fileSynchronizationService.SynchronizeAsync();

                    Console.WriteLine($"File synchronization completed at {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File synchronization failed at {DateTime.Now}.{Environment.NewLine}{ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Tester is unavailable and disconnected.");
            }
        }

        /// <summary>
        /// Marks host as available (online)
        /// - which enables:
        ///   1. CRUD methods
        ///   2. CRUD result code groups.
        ///   3. CRUD result codes.
        /// </summary>
        private void MarkClientOnline()
        {
            // Mark client as online.
            _messageService.Send(new IsOnlineMessage(true));
            Console.WriteLine("Synchronization service is online.");
        }

        /// <summary>
        /// Marks host as unavailable (offline)
        /// - which disables:
        ///   1. CRUD methods
        ///   2. CRUD result code groups.
        ///   3. CRUD result codes.
        /// </summary>
        private void MarkClientOffline()
        {
            // Mark client as offline.
            _messageService.Send(new IsOnlineMessage(false));
            Console.WriteLine("Synchronization service is offline.");
        }
    }
}
