using ISynergy.Common.Options;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Services.Enumerations;
using ISynergy.Services.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    /// <summary>
    /// Class ClientMonitorService.
    /// </summary>
    public class ClientMonitorService : IClientMonitorService
    {
        /// <summary>
        /// Event raised when UI is refreshed.
        /// </summary>
        public event EventHandler RefreshUI;

        /// <summary>
        /// Event raised when CallerId phone number is selected.
        /// </summary>
        public event EventHandler<CallerMessage> POSCallerIdPhoneSelected;

        /// <summary>
        /// The dialog service
        /// </summary>
        protected readonly IDialogService _dialogService;
        /// <summary>
        /// The language service
        /// </summary>
        protected readonly ILanguageService _languageService;
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// The configuration options
        /// </summary>
        protected readonly ConfigurationOptions _configurationOptions;

        /// <summary>
        /// The connection
        /// </summary>
        protected HubConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMonitorService"/> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger factory.</param>
        public ClientMonitorService(
            IDialogService dialogService,
            ILanguageService languageService,
            IOptions<ConfigurationOptions> configurationOptions,
            ILogger logger)
        {
            _dialogService = dialogService;
            _languageService = languageService;
            _configurationOptions = configurationOptions.Value;
            _logger = logger;
        }

        public void OnRefreshUI()
        {
            RefreshUI?.Invoke(this, EventArgs.Empty);
        }

        public void OnPOSCallerIdPhoneSelected(CallerMessage message)
        {
            POSCallerIdPhoneSelected?.Invoke(this, message);
        }

        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public virtual Task ConnectAsync(string token)
        {
            _logger.LogInformation($"Connecting to {_configurationOptions.SignalREndpoint}");

            _connection = new HubConnectionBuilder()
                .WithUrl(_configurationOptions.SignalREndpoint, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            // Set up handler
            _connection.On<HubMessage>(nameof(MonitorEvents.RefreshDashboard), (_) =>
            {
                OnRefreshUI();
            });

            _connection.On<HubMessage>(nameof(MonitorEvents.Connected), async (m) =>
            {
                // User has logged in. 
                await _dialogService.ShowInformationAsync(
                    string.Format(_languageService.GetString("Warning_User_Loggedin"), m.Data.ToString()));
            });

            _connection.On<HubMessage>(nameof(MonitorEvents.NotifyCallerId), (m) =>
            {
                var message = JsonConvert.DeserializeObject<CallerMessage>(m.Data.ToString());

                if (message is not null)
                    OnPOSCallerIdPhoneSelected(message);
            });

            _connection.On<HubMessage>(nameof(MonitorEvents.Disconnected), async (m) =>
            {
                // User has logged out. 
                await _dialogService.ShowInformationAsync(
                    string.Format(_languageService.GetString("Warning_User_Loggedout"), m.Data.ToString()));
            });

            return _connection.StartAsync();
        }

        /// <summary>
        /// disconnect as an asynchronous operation.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_connection is not null)
                await _connection.DisposeAsync();
        }
    }
}
