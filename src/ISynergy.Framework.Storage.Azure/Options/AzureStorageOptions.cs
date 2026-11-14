using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Storage.Azure.Options;

/// <summary>
/// Configuration options for Azure Blob Storage.
/// </summary>
/// <remarks>
/// <para>
/// Authentication is resolved in this order:
/// </para>
/// <list type="number">
/// <item><description>
/// An injected <c>BlobServiceClient</c> (for example the one registered by Aspire's
/// <c>AddAzureBlobServiceClient</c>) takes precedence.
/// </description></item>
/// <item><description>
/// Identity-based authentication when a service URI can be resolved from
/// <see cref="ServiceEndpoint"/> or from <see cref="AccountName"/> without <see cref="AccountKey"/>.
/// </description></item>
/// <item><description>
/// The connection string (<see cref="ConnectionString"/>, or <see cref="AccountName"/> plus
/// <see cref="AccountKey"/>) as fallback.
/// </description></item>
/// </list>
/// </remarks>
public class AzureStorageOptions
{
    /// <summary>
    /// Gets or sets the Azure Storage connection string.
    /// Used only when identity-based authentication cannot be configured.
    /// </summary>
    /// <remarks>
    /// Example: <c>DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=mykey;EndpointSuffix=core.windows.net</c>
    /// Use <c>UseDevelopmentStorage=true</c> for the local Azurite emulator.
    /// </remarks>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage account name.
    /// When <see cref="AccountKey"/> is also set, the account key connection string is used as a
    /// fallback. When <see cref="AccountKey"/> is not set, the account name is used to derive the
    /// service URI and identity-based authentication is used.
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Gets or sets the Azure Storage account key (base-64 encoded).
    /// Used together with <see cref="AccountName"/> when no identity-based endpoint is configured.
    /// </summary>
    public string? AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the optional Azure Storage service endpoint URI.
    /// When set, identity-based authentication is used against this endpoint.
    /// Useful for sovereign clouds, Azure Aspire service discovery, and the Azurite emulator.
    /// </summary>
    public string? ServiceEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the client identifier of a user-assigned managed identity.
    /// When empty, the system-assigned identity (or another
    /// <c>DefaultAzureCredential</c> source) is used.
    /// </summary>
    public string? ManagedIdentityClientId { get; set; }

    /// <summary>
    /// Attempts to resolve the Blob Storage service endpoint used for identity-based authentication.
    /// </summary>
    /// <param name="serviceUri">The resolved service URI when the method returns <c>true</c>.</param>
    /// <returns>
    /// <c>true</c> when an absolute service URI can be resolved from <see cref="ServiceEndpoint"/>
    /// or from <see cref="AccountName"/> (without an account key); otherwise <c>false</c>.
    /// </returns>
    internal bool TryGetServiceUri([NotNullWhen(true)] out Uri? serviceUri)
    {
        var endpoint = ServiceEndpoint;

        if (string.IsNullOrWhiteSpace(endpoint)
            && !string.IsNullOrWhiteSpace(AccountName)
            && string.IsNullOrWhiteSpace(AccountKey))
        {
            endpoint = $"https://{AccountName}.blob.core.windows.net";
        }

        if (!string.IsNullOrWhiteSpace(endpoint)
            && Uri.TryCreate(endpoint.TrimEnd('/'), UriKind.Absolute, out var uri))
        {
            serviceUri = uri;
            return true;
        }

        serviceUri = null;
        return false;
    }

    /// <summary>
    /// Resolves the effective connection string used as the fallback authentication mechanism.
    /// Returns <see cref="ConnectionString"/> if set; otherwise builds one from
    /// <see cref="AccountName"/> and <see cref="AccountKey"/>.
    /// </summary>
    /// <returns>The Azure Storage connection string, or an empty string when none is configured.</returns>
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
