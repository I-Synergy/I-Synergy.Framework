using ISynergy.Framework.Storage.Abstractions;

namespace ISynergy.Framework.Storage.Azure.Sample.Options
{
    public class AzureBlobOptions : IStorageOptions
    {
        public string ConnectionString { get; set; }
    }
}
