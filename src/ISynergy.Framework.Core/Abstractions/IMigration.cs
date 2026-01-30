namespace ISynergy.Framework.Core.Abstractions;

public interface IMigration
{
    Task UpAsync();
    Task DownAsync();
    int MigrationVersion { get; }
}
