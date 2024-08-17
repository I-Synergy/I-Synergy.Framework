using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Synchronization.Abstractions.Services;
using ISynergy.Framework.Synchronization.Abstractions.Settings;
using ISynergy.Framework.Synchronization.Factories;
using ISynergy.Framework.Synchronization.Messages;
using ISynergy.Framework.UI.Options;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace Sample.Services;

internal class SynchronizationService : ISynchronizationService
{
    private readonly IContext _context;
    private readonly IMessageService _messageService;
    private readonly SettingsService _settingsService;
    private readonly ISynchronizationSettings _synchronizationSettings;

    public bool IsActive { get; }
    public SyncAgent SynchronizationAgent { get; }
    public Uri SynchronizationEndpoint { get; }
    public string SynchronizationFolder { get; }
    public string SnapshotsFolder { get; }
    public string BatchesFolder { get; }
    public string OfflineDatabase { get; }
    public ISynchronizationSettings SynchronizationOptions => _synchronizationSettings;

    public SynchronizationService(
        IContext context,
        IMessageService messageService,
        SettingsService settingsService,
        IOptions<ConfigurationOptions> configurationOptions)
    {
        _context = context;
        _messageService = messageService;
        _settingsService = settingsService;

        if (!_context.IsAuthenticated)
            throw new InvalidOperationException("User is not authenticated");

        var options = configurationOptions.Value;
        var tenantId = _context.Profile.AccountId.ToString("N");

        _synchronizationSettings = _settingsService.RoamingSettings.SynchronizationSetting;

        if (_synchronizationSettings is not null && _settingsService.RoamingSettings.IsSynchronizationEnabled)
        {
            IsActive = _settingsService.RoamingSettings.IsSynchronizationEnabled;

            if (string.IsNullOrEmpty(_synchronizationSettings.SynchronizationFolder))
                _synchronizationSettings.SynchronizationFolder = Path.Combine(FileSystem.AppDataDirectory, "Synchronization");

            SynchronizationFolder = Path.Combine(_synchronizationSettings.SynchronizationFolder, tenantId);

            if (!Directory.Exists(SynchronizationFolder))
                Directory.CreateDirectory(SynchronizationFolder);

            OfflineDatabase = Path.Combine(SynchronizationFolder, "data.db");

            if (string.IsNullOrEmpty(_synchronizationSettings.SnapshotFolder))
                _synchronizationSettings.SnapshotFolder = Path.Combine(FileSystem.AppDataDirectory, "Snapshots");

            SnapshotsFolder = Path.Combine(_synchronizationSettings.SnapshotFolder, tenantId);

            if (!Directory.Exists(SnapshotsFolder))
                Directory.CreateDirectory(SnapshotsFolder);

            if (string.IsNullOrEmpty(_synchronizationSettings.BatchesFolder))
                _synchronizationSettings.BatchesFolder = Path.Combine(FileSystem.AppDataDirectory, "Snapshots");

            BatchesFolder = Path.Combine(_synchronizationSettings.BatchesFolder, tenantId);

            if (!Directory.Exists(BatchesFolder))
                Directory.CreateDirectory(BatchesFolder);

            SynchronizationEndpoint = new Uri(Path.Combine(options.ServiceEndpoint, "sync"));

            var handler = new HttpClientHandler();
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Android && SynchronizationEndpoint.Host == "10.0.2.2")
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                };
            }
#endif

            handler.AutomaticDecompression = DecompressionMethods.GZip;

            var httpClient = new HttpClient(handler);

            httpClient.DefaultRequestHeaders.Add(nameof(Grant.client_id), options.ClientId);
            httpClient.DefaultRequestHeaders.Add(nameof(Grant.client_secret), options.ClientSecret);

            if (!string.IsNullOrEmpty(_synchronizationSettings.Version))
                httpClient.DefaultRequestHeaders.Add(GenericConstants.ApiVersion, _synchronizationSettings.Version);

            if (!_synchronizationSettings.IsAnonymous)
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationTypes.Bearer, _context.Profile.Token.AccessToken);

            // Check if we are trying to reach a IIS Express.
            // IIS Express does not allow any request other than localhost
            // So far,hacking the Host-Content header to mimic localhost call
            if (DeviceInfo.Platform == DevicePlatform.Android && SynchronizationEndpoint.Host == "10.0.2.2")
                httpClient.DefaultRequestHeaders.Host = $"localhost:{SynchronizationEndpoint.Port}";

            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var webRemoteOrchestrator = new WebRemoteOrchestrator(SynchronizationEndpoint.AbsoluteUri, client: httpClient, maxDownladingDegreeOfParallelism: 1)
            {
                //Converter = new SqliteConverter()
                SerializerFactory = new MessagePackSerializerFactory()
            };

            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = OfflineDatabase
            };

            var sqliteSyncProvider = new SqliteSyncProvider(connectionStringBuilder.ConnectionString);

            var clientOptions = new SyncOptions
            {
                BatchSize = _synchronizationSettings.BatchSize,
                BatchDirectory = BatchesFolder,
                SnapshotsDirectory = SnapshotsFolder,
                CleanFolder = _synchronizationSettings.CleanSynchronizationFolder,
                CleanMetadatas = _synchronizationSettings.CleanSynchronizationMetadatas,
                DisableConstraintsOnApplyChanges = true
            };

            SynchronizationAgent = new SyncAgent(sqliteSyncProvider, webRemoteOrchestrator, clientOptions);

            SynchronizationAgent.SessionStateChanged += (s, state) =>
            {
                _messageService.Send(new SyncSessionStateChangedMessage(state));
            };
        }
    }

    public async Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default)
    {
        if (!_context.IsAuthenticated)
            throw new InvalidOperationException("User is not authenticated");

        if (_synchronizationSettings is not null && IsActive)
        {
            var result = string.Empty;

            var parameters = new SyncParameters()
            {
                new SyncParameter(GenericConstants.TenantId, _context.Profile.AccountId)
            };

            cancellationToken.ThrowIfCancellationRequested();

            var progress = new Progress<ProgressArgs>(args =>
            {
                _messageService.Send(new SyncProgressMessage(args.ProgressPercentage));

                if (result == string.Empty)
                    _messageService.Send(new SyncMessage(args.Message));
            });

            SynchronizationAgent.LocalOrchestrator.OnConflictingSetup(async args =>
            {
                await SynchronizationAgent.LocalOrchestrator.DeprovisionAsync(connection: args.Connection, transaction: args.Transaction);
                await SynchronizationAgent.LocalOrchestrator.ProvisionAsync(args.ServerScopeInfo, connection: args.Connection, transaction: args.Transaction);
                args.Action = ConflictingSetupAction.Continue;
            });

            if (await SynchronizationAgent.SynchronizeAsync(scopeName, syncType, parameters, progress) is SyncResult syncResult)
                result = syncResult.ToString();

            _messageService.Send(new SyncMessage(result));
        }
    }
}
