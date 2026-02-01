using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Synchronization.Abstractions.Settings;

namespace Sample.Models;

public class SynchronizationSettings : ObservableValidatedClass, ISynchronizationSettings
{
    /// <summary>
    /// Gets or sets the SynchronizationInterval property value in seconds.
    /// </summary>
    public int SynchronizationInterval
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the BatchSize property value.
    /// </summary>
    public int BatchSize
    {
        get => GetValue<int>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the SynchronizationFolder property value.
    /// </summary>
    public string SynchronizationFolder
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the SnapshotFolder property value.
    /// </summary>
    public string SnapshotFolder
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the BatchesFolder property value.
    /// </summary>
    public string BatchesFolder
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the CleanSynchronizationFolder property value.
    /// </summary>
    public bool CleanSynchronizationFolder
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the CleanSynchronizationMetadatas property value.
    /// </summary>
    public bool CleanSynchronizationMetadatas
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the synchronization endpoint as anonymous.
    /// Defalut is false.
    /// </summary>
    public bool IsAnonymous
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the default synchronization endpoint version.
    /// </summary>
    public string Version
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizationSettings"/> class.
    /// </summary>
    public SynchronizationSettings()
    {
        BatchSize = 1000;
        CleanSynchronizationFolder = false;
        CleanSynchronizationMetadatas = false;
        IsAnonymous = false;
        SynchronizationInterval = 60;
        Version = "2";
    }
}
