namespace ISynergy.Framework.KeyVault.Options;

/// <summary>
/// Options for configuring the HashiCorp Vault connection.
/// </summary>
public class KeyVaultOptions
{
    /// <summary>
    /// Gets or sets the URI of the Vault server (e.g. <c>https://vault.example.com:8200</c>).
    /// </summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Vault token used to authenticate requests.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
