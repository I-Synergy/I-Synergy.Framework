using ISynergy.Framework.Storage.Abstractions;

namespace ISynergy.Framework.Storage.Azure.Tests
{
    public class AzureBlobOptions : IStorageOptions
    {
        public string ConnectionString { get; set; }
    }
}
