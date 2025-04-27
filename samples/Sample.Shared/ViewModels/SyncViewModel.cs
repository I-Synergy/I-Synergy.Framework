using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.Synchronization.Abstractions.Services;
using ISynergy.Framework.Synchronization.Messages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Sample.ViewModels;

public class SyncViewModel : ViewModelNavigation<object>
{
    public override string Title { get { return LanguageService.Default.GetString("Sync"); } }

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

    public AsyncRelayCommand? SyncCommand { get; set; }
    public AsyncRelayCommand? SyncReinitializeCommand { get; set; }
    public AsyncRelayCommand? CustomActionCommand { get; set; }
    public AsyncRelayCommand? DeprovisionClientCommand { get; private set; }
    public AsyncRelayCommand? ProvisionClientCommand { get; private set; }


    public SyncViewModel(
        ICommonServices commonServices,
        ILogger<SyncViewModel> logger)
        : base(commonServices, logger)
    {
        SyncProgressionText = "Ready...";
        SyncCommandButtonEnabled = true;

        SyncCommand = new AsyncRelayCommand(async () => await ExecuteSyncCommand(SyncType.Normal), () => _commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive);
        SyncReinitializeCommand = new AsyncRelayCommand(async () => await ExecuteSyncCommand(SyncType.Reinitialize), () => _commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive);
        CustomActionCommand = new AsyncRelayCommand(CustomActionCommandExecuteAsync, () => _commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive);
        DeprovisionClientCommand = new AsyncRelayCommand(DeprovisionClientAsync, () => _commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive);
        ProvisionClientCommand = new AsyncRelayCommand(ProvisionClientAsync, () => _commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive);

        MessageService.Default.Register<SyncMessage>(this, m => SyncProgressionText = m.Content);
        MessageService.Default.Register<SyncProgressMessage>(this, m => SyncProgress = m.Content);
        MessageService.Default.Register<SyncSessionStateChangedMessage>(this, m => SyncCommandButtonEnabled = m.Content.State == SyncSessionState.Ready);
    }

    private async Task CustomActionCommandExecuteAsync()
    {
        _commonServices.BusyService.StartBusy();

        try
        {
            CustomActionInsertProductRow();
            SyncProgressionText = "1000 Categories added";
        }
        catch (Exception ex)
        {
            await _commonServices.DialogService.ShowErrorAsync(ex.Message);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    private async Task ExecuteSyncCommand(SyncType syncType)
    {
        _commonServices.BusyService.StartBusy();

        try
        {
            if (_commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive)
                await _commonServices.ScopedContextService.GetService<ISynchronizationService>().SynchronizeAsync(syncType);
        }
        catch (Exception ex)
        {
            await _commonServices.DialogService.ShowErrorAsync(ex.Message);
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    private void CustomActionInsertProductRow()
    {
        if (_commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive)
        {
            var connectionstringbuilder = new SqliteConnectionStringBuilder();
            connectionstringbuilder.DataSource = _commonServices.ScopedContextService.GetService<ISynchronizationService>().OfflineDatabase;

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
        if (_commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive)
        {
            var agent = _commonServices.ScopedContextService.GetService<ISynchronizationService>().SynchronizationAgent;
            var scopeInfo = await agent.RemoteOrchestrator.GetScopeInfoAsync();
            await agent.LocalOrchestrator.ProvisionAsync(scopeInfo);
        }
    }

    private async Task DeprovisionClientAsync()
    {
        if (_commonServices.ScopedContextService.GetService<ISynchronizationService>().IsActive)
        {
            var agent = _commonServices.ScopedContextService.GetService<ISynchronizationService>().SynchronizationAgent;
            await agent.LocalOrchestrator.DeprovisionAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SyncCommand?.Dispose();
            SyncCommand = null;
            SyncReinitializeCommand?.Dispose();
            SyncReinitializeCommand = null;
            CustomActionCommand?.Dispose();
            CustomActionCommand = null;
            DeprovisionClientCommand?.Dispose();
            DeprovisionClientCommand = null;
            ProvisionClientCommand?.Dispose();
            ProvisionClientCommand = null;

            MessageService.Default.Unregister<SyncMessage>(this);
            MessageService.Default.Unregister<SyncProgressMessage>(this);
            MessageService.Default.Unregister<SyncSessionStateChangedMessage>(this);
        }

        base.Dispose(disposing);
    }
}
