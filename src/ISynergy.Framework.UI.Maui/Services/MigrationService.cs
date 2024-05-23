using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

public class MigrationService : IMigrationService
{
    public Task ApplyMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        var method = typeof(TMigration).GetMethod(nameof(IMigration.Up));
        method.Invoke(null, null);
        return Task.CompletedTask;
    }

    public Task RevertMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        var method = typeof(TMigration).GetMethod(nameof(IMigration.Down));
        method.Invoke(null, null);
        return Task.CompletedTask;
    }
}
