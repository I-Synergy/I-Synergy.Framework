using ISynergy.Framework.Core.Abstractions;

namespace Sample.Migrations;

public class _001 : IMigration
{
    public int MigrationVersion => 1;

    public Task UpAsync() =>
        Task.CompletedTask;

    public Task DownAsync() =>
        Task.CompletedTask;
}
