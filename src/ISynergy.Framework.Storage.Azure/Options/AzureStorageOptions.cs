namespace ISynergy.Framework.Storage.Azure.Options;

/// <summary>
/// Configuration options for Azure Blob Storage.
/// </summary>
public class AzureStorageOptions
{
    /// <summary>
    /// Gets or sets the Azure Storage connection string.
    /// When set, this takes precedence over <see cref="AccountName"/> and <see cref="AccountKey"/>.
    /// </summary>
    /// <remarks>
    /// Example: <c>DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=mykey;EndpointSuffix=core.windows.net</c>
    /// Use <c>UseDevelopmentStorage=true</c> for the local Azurite emulator.
    /// </remarks>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage account name.
    /// Used together with <see cref="AccountKey"/> when <see cref="ConnectionString"/> is not set.
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage account key (base-64 encoded).
    /// Used together with <see cref="AccountName"/> when <see cref="ConnectionString"/> is not set.
    /// </summary>
    public string? AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the optional Azure Storage service endpoint URI.
    /// When set, overrides the default endpoint derived from <see cref="AccountName"/>.
    /// Useful for sovereign clouds or the Azurite emulator.
    /// </summary>
    public string? ServiceEndpoint { get; set; }

    /// <summary>
    /// Resolves the effective connection string.
    /// Returns <see cref="ConnectionString"/> if set; otherwise builds one from
    /// <see cref="AccountName"/> and <see cref="AccountKey"/>.
    /// </summary>
    /// <returns>The Azure Storage connection string to use when creating <c>BlobContainerClient</c> instances.</returns>
    internal string GetEffectiveConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
            return ConnectionString;

        if (!string.IsNullOrWhiteSpace(AccountName) && !string.IsNullOrWhiteSpace(AccountKey))
        {
            var endpoint = string.IsNullOrWhiteSpace(ServiceEndpoint)
                ? $"https://{AccountName}.blob.core.windows.net"
                : ServiceEndpoint.TrimEnd('/');

            return $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};BlobEndpoint={endpoint}";
        }

        return string.Empty;
    }
}
