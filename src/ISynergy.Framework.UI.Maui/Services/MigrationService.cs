using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

public class MigrationService : IMigrationService
{
    private readonly IApplicationSettingsService _applicationSettingsService;

    public MigrationService(IApplicationSettingsService applicationSettingsService)
    {
        _applicationSettingsService = applicationSettingsService;
    }

    public async Task ApplyMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        var migration = Activator.CreateInstance(typeof(TMigration)) as IMigration;

        if (_applicationSettingsService.Settings.MigrationVersion < migration.MigrationVersion)
        {
            await migration.UpAsync();

            _applicationSettingsService.Settings.MigrationVersion = migration.MigrationVersion;
            _applicationSettingsService.SaveSettings();
        }
    }

    public async Task RevertMigrationAsync<TMigration>()
        where TMigration : class, IMigration
    {
        var migration = Activator.CreateInstance(typeof(TMigration)) as IMigration;

        if (_applicationSettingsService.Settings.MigrationVersion > migration.MigrationVersion)
        {
            await migration.DownAsync();

            _applicationSettingsService.Settings.MigrationVersion = migration.MigrationVersion;
            _applicationSettingsService.SaveSettings();
        }
    }
}
