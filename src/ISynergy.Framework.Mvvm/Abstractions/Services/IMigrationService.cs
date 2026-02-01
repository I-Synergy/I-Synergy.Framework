using ISynergy.Framework.Core.Abstractions;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

public interface IMigrationService
{
    Task ApplyMigrationAsync<TMigration>() where TMigration : class, IMigration;
    Task RevertMigrationAsync<TMigration>() where TMigration : class, IMigration;
}
