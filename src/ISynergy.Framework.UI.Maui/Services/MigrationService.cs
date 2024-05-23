using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

public class MigrationService : IMigrationService
{
    private readonly IPreferences _preferences;

    public MigrationService(IPreferences preferences)
    {
        _preferences = preferences;
    }

    public Task ApplyMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        var property = typeof(TMigration).GetProperty(nameof(IMigration.MigrationVersion));
        var version = (int)property.GetValue(null);

        if (_preferences.ContainsKey(nameof(IMigration.MigrationVersion)) &&
            _preferences.Get(nameof(IMigration.MigrationVersion), 0) < version)
        {
            var method = typeof(TMigration).GetMethod(nameof(IMigration.Up));
            method.Invoke(null, null);

            _preferences.Set(nameof(IMigration.MigrationVersion), version);
        }

        return Task.CompletedTask;
    }

    public Task RevertMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        var property = typeof(TMigration).GetProperty(nameof(IMigration.MigrationVersion));
        var version = (int)property.GetValue(null);

        if (_preferences.ContainsKey(nameof(IMigration.MigrationVersion)) &&
            _preferences.Get(nameof(IMigration.MigrationVersion), 0) > version)
        {
            var method = typeof(TMigration).GetMethod(nameof(IMigration.Down));
            method.Invoke(null, null);

            _preferences.Set(nameof(IMigration.MigrationVersion), version);
        }
        
        return Task.CompletedTask;
    }
}
