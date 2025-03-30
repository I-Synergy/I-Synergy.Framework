namespace ISynergy.Framework.AspNetCore.Options;

/// <summary>
/// Class KeyVaultOptions.
/// </summary>
public class KeyVaultOptions
{
    /// <summary>
    /// Gets or sets the key vault URI.
    /// </summary>
    /// <value>The key vault URI.</value>
    public required Uri KeyVaultUri { get; set; }
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>The client identifier.</value>
    public required string ClientId { get; set; }
    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>The client secret.</value>
    public required string ClientSecret { get; set; }
}