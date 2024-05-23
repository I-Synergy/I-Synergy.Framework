using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Factories;
using ISynergy.Framework.Synchronization.Messages;
using ISynergy.Framework.Synchronization.Options;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace ISynergy.Framework.Synchronization.Services;

internal class SynchronizationService : ISynchronizationService
{
    private readonly IContext _context;
    private readonly IMessageService _messageService;
    private readonly IPreferences _preferences;
    private readonly SynchronizationSettings _synchronizationOptions;

    public SyncAgent SynchronizationAgent { get; }
    public Uri SynchronizationEndpoint { get; }
    public string SynchronizationFolder { get; }
    public string SnapshotsFolder { get; }
    public string BatchesFolder { get; }
    public string OfflineDatabase { get; }
    public SynchronizationSettings SynchronizationOptions => _synchronizationOptions;

    public SynchronizationService(
        IContext context,
        IMessageService messageService,
        IPreferences preferences,
        IOptions<ConfigurationOptions> configurationOptions)
    {
        _context = context;
        _messageService = messageService;
        _preferences = preferences;

        if (!_context.IsAuthenticated)
            throw new InvalidOperationException("User is not authenticated");

        var options = configurationOptions.Value;
        var tenantId = _context.Profile.AccountId.ToString("N");

        _synchronizationOptions = _preferences.GetObject<SynchronizationSettings>(nameof(SynchronizationOptions), default);

        if (_synchronizationOptions is not null && _synchronizationOptions.IsSynchronizationEnabled)
        {
            if (string.IsNullOrEmpty(_synchronizationOptions.SynchronizationFolder))
                _synchronizationOptions.SynchronizationFolder = Path.Combine(FileSystem.AppDataDirectory, "Synchronization");

            SynchronizationFolder = Path.Combine(_synchronizationOptions.SynchronizationFolder, tenantId);

            if (!Directory.Exists(SynchronizationFolder))
                Directory.CreateDirectory(SynchronizationFolder);

            OfflineDatabase = Path.Combine(SynchronizationFolder, "data.db");

            if (string.IsNullOrEmpty(_synchronizationOptions.SnapshotFolder))
                _synchronizationOptions.SnapshotFolder = Path.Combine(FileSystem.AppDataDirectory, "Snapshots");

            SnapshotsFolder = Path.Combine(_synchronizationOptions.SnapshotFolder, tenantId);

            if (!Directory.Exists(SnapshotsFolder))
                Directory.CreateDirectory(SnapshotsFolder);

            if (string.IsNullOrEmpty(_synchronizationOptions.BatchesFolder))
                _synchronizationOptions.BatchesFolder = Path.Combine(FileSystem.AppDataDirectory, "Snapshots");

            BatchesFolder = Path.Combine(_synchronizationOptions.BatchesFolder, tenantId);

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

            if (!string.IsNullOrEmpty(_synchronizationOptions.Version))
                httpClient.DefaultRequestHeaders.Add(GenericConstants.ApiVersion, _synchronizationOptions.Version);

            if (!_synchronizationOptions.IsAnonymous)
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
                BatchSize = _synchronizationOptions.BatchSize,
                BatchDirectory = BatchesFolder,
                SnapshotsDirectory = SnapshotsFolder,
                CleanFolder = _synchronizationOptions.CleanSynchronizationFolder,
                CleanMetadatas = _synchronizationOptions.CleanSynchronizationMetadatas,
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

        if (_synchronizationOptions is not null && _synchronizationOptions.IsSynchronizationEnabled)
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