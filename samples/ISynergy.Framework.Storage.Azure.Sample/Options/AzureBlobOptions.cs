using ISynergy.Framework.Storage.Abstractions.Options;

namespace ISynergy.Framework.Storage.Azure.Sample.Options
{
    public class AzureBlobOptions : IStorageOptions
    {
        public string ConnectionString { get; set; }
    }
}
