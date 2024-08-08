using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Synchronization.Abstractions.Services;
using ISynergy.Framework.Synchronization.Messages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using System.Data;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Scoped)]
public class SyncViewModel : ViewModelNavigation<object>
{
    public override string Title { get { return BaseCommonServices.LanguageService.GetString("Sync"); } }

    private readonly ISynchronizationService _synchronizationService;

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
    public AsyncRelayCommand SyncReinitializeCommand { get; set; }
    public AsyncRelayCommand CustomActionCommand { get; set; }

    public AsyncRelayCommand DeprovisionClientCommand { get; private set; }
    public AsyncRelayCommand ProvisionClientCommand { get; private set; }


    public SyncViewModel(
        IContext context,
        ICommonServices commonServices,
        ISynchronizationService synchronizationService,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        _synchronizationService = synchronizationService;

        SyncProgressionText = "Ready...";
        SyncCommandButtonEnabled = true;

        SyncCommand = new AsyncRelayCommand(async () => await ExecuteSyncCommand(SyncType.Normal), () => _synchronizationService.IsActive);
        SyncReinitializeCommand = new AsyncRelayCommand(async () => await ExecuteSyncCommand(SyncType.Reinitialize), () => _synchronizationService.IsActive);
        CustomActionCommand = new AsyncRelayCommand(CustomActionCommandExecuteAsync, () => _synchronizationService.IsActive);
        DeprovisionClientCommand = new AsyncRelayCommand(DeprovisionClientAsync, () => _synchronizationService.IsActive);
        ProvisionClientCommand = new AsyncRelayCommand(ProvisionClientAsync, () => _synchronizationService.IsActive);

        MessageService.Default.Register<SyncMessage>(this, m => SyncProgressionText = m.Content);
        MessageService.Default.Register<SyncProgressMessage>(this, m => SyncProgress = m.Content);
        MessageService.Default.Register<SyncSessionStateChangedMessage>(this, m => SyncCommandButtonEnabled = m.Content == SyncSessionState.Ready);
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
            if (_synchronizationService.IsActive)
                await _synchronizationService.SynchronizeAsync(syncType);
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
        if (_synchronizationService.IsActive)
        {
            var connectionstringbuilder = new SqliteConnectionStringBuilder();
            connectionstringbuilder.DataSource = _synchronizationService.OfflineDatabase;

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
    }

    private async Task ProvisionClientAsync()
    {
        if (_synchronizationService.IsActive)
        {
            var agent = _synchronizationService.SynchronizationAgent;
            var scopeInfo = await agent.RemoteOrchestrator.GetScopeInfoAsync();
            await agent.LocalOrchestrator.ProvisionAsync(scopeInfo);
        }
    }

    private async Task DeprovisionClientAsync()
    {
        if (_synchronizationService.IsActive)
        {
            var agent = _synchronizationService.SynchronizationAgent;
            await agent.LocalOrchestrator.DeprovisionAsync();
        }
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
