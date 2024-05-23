namespace ISynergy.Framework.Synchronization.Options;

public class SynchronizationSettings
{
    /// <summary>
    /// Gets or sets the IsSynchronizationEnabled property value.
    /// </summary>
    public bool IsSynchronizationEnabled { get; set; }

    /// <summary>
    /// Gets or sets the SynchronizationInterval property value in seconds.
    /// </summary>
    public int SynchronizationInterval { get; set; }

    /// <summary>
    /// Gets or sets the BatchSize property value.
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// Gets or sets the SynchronizationFolder property value.
    /// </summary>
    public string SynchronizationFolder { get; set; }

    /// <summary>
    /// Gets or sets the SnapshotFolder property value.
    /// </summary>
    public string SnapshotFolder { get; set; }

    /// <summary>
    /// Gets or sets the BatchesFolder property value.
    /// </summary>
    public string BatchesFolder { get; set; }

    /// <summary>
    /// Gets or sets the CleanSynchronizationFolder property value.
    /// </summary>
    public bool CleanSynchronizationFolder { get; set; }

    /// <summary>
    /// Gets or sets the CleanSynchronizationMetadatas property value.
    /// </summary>
    public bool CleanSynchronizationMetadatas { get; set; }

    /// <summary>
    /// Gets or sets the synchronization endpoint as anonymous.
    /// Defalut is false.
    /// </summary>
    public bool IsAnonymous { get; set; }

    /// <summary>
    /// Gets or sets the default synchronization endpoint version.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizationSettings"/> class.
    /// </summary>
    public SynchronizationSettings()
    {
        BatchSize = 1000;
        CleanSynchronizationFolder = false;
        CleanSynchronizationMetadatas = false;
        IsAnonymous = false;
        IsSynchronizationEnabled = false;
        SynchronizationInterval = 60;
        Version = "2";
    }
}
