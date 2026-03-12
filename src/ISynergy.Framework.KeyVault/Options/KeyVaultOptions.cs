namespace ISynergy.Framework.KeyVault.Options;

/// <summary>
/// Configuration options for connecting to a HashiCorp Vault instance.
/// </summary>
public class KeyVaultOptions
{
    /// <summary>
    /// Gets or sets the URI of the Vault server.
    /// </summary>
    /// <value>The fully-qualified URI of the Vault server (e.g. <c>https://vault.example.com:8200</c>).</value>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authentication token used to access the Vault server.
    /// </summary>
    /// <value>A valid Vault token string.</value>
    public string Token { get; set; } = string.Empty;
}
