using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Messages;
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
    private readonly SyncAgent _syncAgent;
    private const string _apiVersion = "2";

    public SynchronizationService(
        IContext context,
        IMessageService messageService,
        ISynchronizationSettingsService localSettingsService,
        IOptions<ConfigurationOptions> configurationOptions)
    {
        _context = context;
        _messageService = messageService;

        if (!_context.IsAuthenticated)
            throw new InvalidOperationException("User is not authenticated");

        var options = configurationOptions.Value;

        if (!Directory.Exists(localSettingsService.Settings.SynchronizationFolder))
            Directory.CreateDirectory(localSettingsService.Settings.SynchronizationFolder);

        if (!Directory.Exists(localSettingsService.Settings.SnapshotFolder))
            Directory.CreateDirectory(localSettingsService.Settings.SnapshotFolder);

        if (!Directory.Exists(localSettingsService.Settings.BatchesFolder))
            Directory.CreateDirectory(localSettingsService.Settings.BatchesFolder);

        var synchronizationUri = new Uri(Path.Combine(options.ServiceEndpoint, "sync"));

        var handler = new HttpClientHandler();
#if DEBUG
        if (DeviceInfo.Platform == DevicePlatform.Android && synchronizationUri.Host == "10.0.2.2")
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
        httpClient.DefaultRequestHeaders.Add(GenericConstants.ApiVersion, _apiVersion);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _context.Profile.Token.AccessToken);


        // Check if we are trying to reach a IIS Express.
        // IIS Express does not allow any request other than localhost
        // So far,hacking the Host-Content header to mimic localhost call
        if (DeviceInfo.Platform == DevicePlatform.Android && synchronizationUri.Host == "10.0.2.2")
            httpClient.DefaultRequestHeaders.Host = $"localhost:{synchronizationUri.Port}";

        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        var webRemoteOrchestrator = new WebRemoteOrchestrator(synchronizationUri.AbsoluteUri, client: httpClient, maxDownladingDegreeOfParallelism: 1)
        {
            //Converter = new SqliteConverter()
        };

        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(localSettingsService.Settings.SynchronizationFolder, $"{_context.Profile.AccountId:N}.db")
        };

        var sqliteSyncProvider = new SqliteSyncProvider(connectionStringBuilder.ConnectionString);

        var clientOptions = new SyncOptions
        {
            BatchSize = localSettingsService.Settings.BatchSize,
            BatchDirectory = localSettingsService.Settings.BatchesFolder,
            SnapshotsDirectory = localSettingsService.Settings.SnapshotFolder,
            CleanFolder = localSettingsService.Settings.CleanSynchronizationFolder,
            CleanMetadatas = localSettingsService.Settings.CleanSynchronizationMetadatas,
            DisableConstraintsOnApplyChanges = true
        };

        _syncAgent = new SyncAgent(sqliteSyncProvider, webRemoteOrchestrator, clientOptions);

        _syncAgent.SessionStateChanged += (s, state) =>
        {
            _messageService.Send(new SyncSessionStateChangedMessage(state));
        };
    }

    public async Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default)
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

        _syncAgent.LocalOrchestrator.OnConflictingSetup(async args =>
        {
            await _syncAgent.LocalOrchestrator.DeprovisionAsync(connection: args.Connection, transaction: args.Transaction);
            await _syncAgent.LocalOrchestrator.ProvisionAsync(args.ServerScopeInfo, connection: args.Connection, transaction: args.Transaction);
            args.Action = ConflictingSetupAction.Continue;
        });

        if (await _syncAgent.SynchronizeAsync(scopeName, syncType, parameters, progress) is SyncResult syncResult)
            result = syncResult.ToString();

        _messageService.Send(new SyncMessage(result));
    }
}