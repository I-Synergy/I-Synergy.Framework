using ISynergy.Framework.Storage.Abstractions.Options;

namespace Sample.Storage.Azure.Options;

public class AzureBlobOptions : IStorageOptions
{
    public string ConnectionString { get; set; }
}
