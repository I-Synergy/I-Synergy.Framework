using ISynergy.Framework.Storage.Abstractions.Options;

namespace ISynergy.Framework.Storage.Options;
/// <summary>
/// Azure Storage options.
/// </summary>
public class AzureStorageOptions : IStorageOptions
{
    /// <summary>
    /// Connectionstring
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
