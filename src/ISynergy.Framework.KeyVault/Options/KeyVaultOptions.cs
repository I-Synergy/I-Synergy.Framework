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
    /// <remarks>
    /// <strong>Security notice:</strong> This property is provided for backward compatibility and local
    /// development scenarios only. In production, prefer sourcing the token exclusively from the
    /// <c>VAULT_TOKEN</c> environment variable or from an <c>IVaultTokenProvider</c> implementation
    /// (e.g. AppRole auth). Never place a production Vault token in <c>appsettings.json</c> or
    /// any plain-text configuration file that may be committed to source control.
    /// </remarks>
    [Obsolete("Source the Vault token from the VAULT_TOKEN environment variable or an IVaultTokenProvider. Storing tokens in configuration is a security risk.")] // NOSONAR
    public string Token { get; set; } = string.Empty;
}
