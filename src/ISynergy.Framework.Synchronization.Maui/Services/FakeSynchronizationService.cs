using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Synchronization.Abstractions;

namespace ISynergy.Framework.Synchronization.Services;

internal class FakeSynchronizationService : ISynchronizationService
{
    private readonly IBaseCommonServices _commonServices;

    public FakeSynchronizationService(IBaseCommonServices commonServices)
    {
        _commonServices = commonServices;
    }

    public Task SynchronizeAsync(SyncType syncType, string scopeName = SyncOptions.DefaultScopeName, CancellationToken cancellationToken = default) =>
        _commonServices.DialogService.ShowWarningAsync(_commonServices.LanguageService.GetString("WarningSynchronizationRestart"));
}
