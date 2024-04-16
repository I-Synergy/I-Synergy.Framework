using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Messages;
using ISynergy.Framework.Synchronization.Options;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace ISynergy.Framework.Synchronization.Services;

public class SynchronizationService : ISynchronizationService
{
    private readonly IDialogService _dialogService;
    private readonly IMessageService _messageService;

    private readonly SqliteSyncProvider _sqliteSyncProvider;
    private readonly WebRemoteOrchestrator _webRemoteOrchestrator;
    private readonly SyncAgent _syncAgent;
    private readonly SynchronizationOptions _synchronizationOptions;
    private readonly HttpClient _httpClient;

    private bool _stopping;
    private int _synchronizationInterval = 30000;

    public string SynchronizationFolder => Path.Combine(FileSystem.AppDataDirectory, "Synchronization");
    public string SnapshotFolder => Path.Combine(FileSystem.AppDataDirectory, "Snapshots");
    public string BatchFolder => Path.Combine(FileSystem.AppDataDirectory, "Batches");
    public string SynchronizationDatabase => Path.Combine(SynchronizationFolder, "sync.db");

    public SynchronizationService(
        IContext context,
        IDialogService dialogService,
        IMessageService messageService,
        IOptions<SynchronizationOptions> synchronizationOptions)
    {
        _dialogService = dialogService;
        _messageService = messageService;
        _synchronizationOptions = synchronizationOptions.Value;
        _synchronizationInterval  = _synchronizationOptions.SynchronizationInterval;

        if (!Directory.Exists(SynchronizationFolder))
            Directory.CreateDirectory(SynchronizationFolder);

        if (!Directory.Exists(SnapshotFolder))
            Directory.CreateDirectory(SnapshotFolder);

        if (!Directory.Exists(BatchFolder))
            Directory.CreateDirectory(BatchFolder);

        SQLitePCL.Batteries_V2.Init();

        var synchronizationUri = new Uri(_synchronizationOptions.SynchronizationEndpoint);

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

        _httpClient = new HttpClient(handler);

        // Check if we are trying to reach a IIS Express.
        // IIS Express does not allow any request other than localhost
        // So far,hacking the Host-Content header to mimic localhost call
        if (DeviceInfo.Platform == DevicePlatform.Android && synchronizationUri.Host == "10.0.2.2")
            _httpClient.DefaultRequestHeaders.Host = $"localhost:{synchronizationUri.Port}";

        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        _webRemoteOrchestrator = new WebRemoteOrchestrator(synchronizationUri.AbsoluteUri, client: _httpClient, maxDownladingDegreeOfParallelism: 1);
        
        var connectionstringbuilder = new SqliteConnectionStringBuilder();
        connectionstringbuilder.DataSource = SynchronizationDatabase;

        _sqliteSyncProvider = new SqliteSyncProvider(connectionstringbuilder.ConnectionString);

        var clientOptions = new SyncOptions 
        { 
            BatchSize = _synchronizationOptions.BatchSize, 
            BatchDirectory = BatchFolder,
            SnapshotsDirectory = SnapshotFolder,
            CleanFolder = _synchronizationOptions.CleanFolder,
            CleanMetadatas = _synchronizationOptions.CleanMetadatas,
            DisableConstraintsOnApplyChanges = true
        };

        _syncAgent = new SyncAgent(_sqliteSyncProvider, _webRemoteOrchestrator, clientOptions);
        
        _syncAgent.SessionStateChanged += (s, state) =>
        {
            _messageService.Send(new SyncSessionStateChangedMessage(state));
        };
    }

    public async Task StartServiceAsync(SyncParameters parameters, CancellationToken cancellationToken = default)
    {
        _stopping = false;

        await Task.Run(async () =>
        {
            while (!_stopping)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await SynchronizeAsync(SyncType.Normal, parameters, cancellationToken);
                    await Task.Delay(_synchronizationInterval);
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync(ex.Message);
                    _stopping = true;
                }
            }
            return;
        }, cancellationToken);
    }
    
    public void StopService() => _stopping = true;


    public async Task SynchronizeAsync(SyncType syncType, SyncParameters parameters, CancellationToken cancellationToken = default)
    {
        var result = string.Empty;

        cancellationToken.ThrowIfCancellationRequested();

        var progress = new Progress<ProgressArgs>(args =>
        {
             _messageService.Send(new SyncProgressMessage(args.ProgressPercentage));

            if (result == string.Empty)
                _messageService.Send(new SyncMessage( args.Message));
        });

        _syncAgent.LocalOrchestrator.OnConflictingSetup(async args =>
        {
            await _syncAgent.LocalOrchestrator.DeprovisionAsync(connection: args.Connection, transaction: args.Transaction);
            await _syncAgent.LocalOrchestrator.ProvisionAsync(args.ServerScopeInfo, connection: args.Connection, transaction: args.Transaction);
            args.Action = ConflictingSetupAction.Continue;
        });

        if (await _syncAgent.SynchronizeAsync(syncType, parameters, progress) is SyncResult syncResult)
            result = syncResult.ToString();

        _messageService.Send(new SyncMessage(result));
    }
}
