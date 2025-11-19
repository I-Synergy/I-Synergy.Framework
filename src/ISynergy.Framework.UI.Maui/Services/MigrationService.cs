using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

public class MigrationService : IMigrationService
{
    private readonly ISettingsService _settingsService;

    public MigrationService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task ApplyMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        if (Activator.CreateInstance(typeof(TMigration)) is IMigration migration &&
            _settingsService.LocalSettings.MigrationVersion < migration.MigrationVersion)
        {
            await migration.UpAsync();

            _settingsService.LocalSettings.MigrationVersion = migration.MigrationVersion;
            _settingsService.SaveLocalSettings();
        }
    }

    public async Task RevertMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        if (Activator.CreateInstance(typeof(TMigration)) is IMigration migration &&
            _settingsService.LocalSettings.MigrationVersion > migration.MigrationVersion)
        {
            await migration.DownAsync();

            _settingsService.LocalSettings.MigrationVersion = migration.MigrationVersion;
            _settingsService.SaveLocalSettings();
        }
    }
}
