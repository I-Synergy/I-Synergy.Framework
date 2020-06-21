using ISynergy.Framework.Storage.Azure.Abstractions;

namespace ISynergy.Framework.Storage.Azure.Sample.Options
{
    public class AzureBlobOptions : IAzureStorageBlobOptions
    {
        public string ConnectionString { get; set; }
    }
}
