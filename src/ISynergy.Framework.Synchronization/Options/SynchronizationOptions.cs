namespace ISynergy.Framework.Synchronization.Options;
public class SynchronizationOptions
{
    public string SynchronizationEndpoint { get; set; }
    public int BatchSize { get; set; }
    public int SynchronizationInterval { get; set; }
    public bool CleanFolder { get; set; }
    public bool CleanMetadatas { get; set; }
}
