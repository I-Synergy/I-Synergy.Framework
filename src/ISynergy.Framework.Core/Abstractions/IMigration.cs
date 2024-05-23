namespace ISynergy.Framework.Core.Abstractions;

public interface IMigration
{
    void Up();
    void Down();
    int MigrationVersion { get; }
}
