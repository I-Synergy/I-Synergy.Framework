using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Synchronization.Abstractions;
using ISynergy.Framework.Synchronization.Messages;
using ISynergy.Framework.Synchronization.Options;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Abstractions;
using System.Data;

namespace Sample.ViewModels;

[Scoped(true)]
public class SyncViewModel : ViewModelNavigation<object>
{
    public override string Title { get { return BaseCommonServices.LanguageService.GetString("Sync"); } }

    private readonly ISynchronizationService _synchronizationService;
    private readonly SynchronizationOptions _synchronizationOptions;
    private readonly SyncParameters _syncParameters;

    /// <summary>
    /// Gets or sets the SyncProgressionText property value.
    /// </summary>
    public string SyncProgressionText
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the SyncCommandButtonEnabled property value.
    /// </summary>
    public bool SyncCommandButtonEnabled
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the SyncProgress property value.
    /// </summary>
    public double SyncProgress
    {
        get => GetValue<double>();
        set => SetValue(value);
    }

    public AsyncRelayCommand SyncCommand { get; set; }
    public AsyncRelayCommand StartSyncCommand { get; set; }
    public RelayCommand StopSyncCommand { get; set; }
    public AsyncRelayCommand SyncReinitializeCommand { get; set; }
    public AsyncRelayCommand CustomActionCommand { get; set; }

    public SyncViewModel(
        IContext context, 
        ICommonServices commonServices,
        ISynchronizationService synchronizationService,
        IOptions<SynchronizationOptions> options,
        ILogger logger, 
        bool automaticValidation = false) 
        : base(context, commonServices, logger, automaticValidation)
    {
        _synchronizationService = synchronizationService;
        _synchronizationOptions = options.Value;

        _syncParameters = new SyncParameters();
        //_syncParameters.Add(new SyncParameter(nameof(IBaseTenantEntity.TenantId), Context.Profile.AccountId));

        SyncProgressionText = "Ready...";
        SyncCommandButtonEnabled = true;

        SyncCommand = new AsyncRelayCommand(async () => await ExecuteSyncCommand(SyncType.Normal));
        SyncReinitializeCommand = new AsyncRelayCommand(async () => await ExecuteSyncCommand(SyncType.Reinitialize));
        CustomActionCommand = new AsyncRelayCommand(CustomActionCommandExecuteAsync);

        StartSyncCommand = new AsyncRelayCommand(() => _synchronizationService.StartServiceAsync(_syncParameters));
        StopSyncCommand = new RelayCommand(_synchronizationService.StopService);

        MessageService.Default.Register<SyncMessage>(this, m => SyncProgressionText = m.Content);
        MessageService.Default.Register<SyncProgressMessage>(this, m => SyncProgress = m.Content);
        MessageService.Default.Register<SyncSessionStateChangedMessage>(this, m => SyncCommandButtonEnabled = m.Content == SyncSessionState.Ready ? true : false);
    }

    private async Task CustomActionCommandExecuteAsync()
    {
        BaseCommonServices.BusyService.StartBusy();
        
        try
        {
            CustomActionInsertProductRow();
            SyncProgressionText = "1000 Categories added";
        }
        catch (Exception ex)
        {
            await BaseCommonServices.DialogService.ShowErrorAsync(ex.Message);
        }
        finally
        {
            BaseCommonServices.BusyService.EndBusy();
        }
    }

    private async Task ExecuteSyncCommand(SyncType syncType)
    {
        BaseCommonServices.BusyService.StartBusy();

        try
        {
            await _synchronizationService.SynchronizeAsync(SyncType.Normal, _syncParameters);
        }
        catch (Exception ex)
        {
            await BaseCommonServices.DialogService.ShowErrorAsync(ex.Message);
        }
        finally
        {
            BaseCommonServices.BusyService.EndBusy();
        }
    }

    private void CustomActionInsertProductRow()
    {
        var connectionstringbuilder = new SqliteConnectionStringBuilder();
        connectionstringbuilder.DataSource = _synchronizationService.SynchronizationDatabase;

        using var sqliteConnection = new SqliteConnection(connectionstringbuilder.ConnectionString);
        var c = new SqliteCommand($"Insert into ProductCategory(ProductcategoryId, Name, rowguid, ModifiedDate, IsActive) Values (@productcategoryId, @name, @rowguid, @modifiedDate, @isactive)")
        {
            Connection = sqliteConnection
        };

        var p = new SqliteParameter
        {
            DbType = DbType.Guid,
            ParameterName = "@productcategoryId"
        };
        c.Parameters.Add(p);

        p = new SqliteParameter
        {
            DbType = DbType.String,
            Size = 50,
            ParameterName = "@name"
        };
        c.Parameters.Add(p);

        p = new SqliteParameter
        {
            DbType = DbType.String,
            Size = 36,
            ParameterName = "@rowguid"
        };
        c.Parameters.Add(p);

        p = new SqliteParameter
        {
            DbType = DbType.DateTime,
            ParameterName = "@modifiedDate"
        };
        c.Parameters.Add(p);

        p = new SqliteParameter
        {
            DbType = DbType.Int32,
            ParameterName = "@isactive"
        };
        c.Parameters.Add(p);

        sqliteConnection.Open();

        c.Prepare();

        using (var t = sqliteConnection.BeginTransaction())
        {

            for (var i = 0; i < 1000; i++)
            {
                c.Transaction = t;
                c.Parameters[0].Value = Guid.NewGuid();
                c.Parameters[1].Value = Guid.NewGuid().ToString();
                c.Parameters[2].Value = Guid.NewGuid().ToString();
                c.Parameters[3].Value = DateTime.Now;
                c.Parameters[4].Value = 1;

                c.ExecuteNonQuery();
            }

            t.Commit();
        }

        sqliteConnection.Close();

    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MessageService.Default.Unregister<SyncMessage>(this);
            MessageService.Default.Unregister<SyncProgressMessage>(this);
            MessageService.Default.Unregister<SyncSessionStateChangedMessage>(this);
        }
    }
}
