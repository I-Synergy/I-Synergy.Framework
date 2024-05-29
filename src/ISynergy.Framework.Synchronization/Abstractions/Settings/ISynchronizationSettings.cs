namespace ISynergy.Framework.Synchronization.Abstractions.Settings;

public interface ISynchronizationSettings
{
    string BatchesFolder { get; set; }
    int BatchSize { get; set; }
    bool CleanSynchronizationFolder { get; set; }
    bool CleanSynchronizationMetadatas { get; set; }
    bool IsAnonymous { get; set; }
    string SnapshotFolder { get; set; }
    string SynchronizationFolder { get; set; }
    int SynchronizationInterval { get; set; }
    string Version { get; set; }
}