namespace ISynergy.Framework.KeyVault.Options;

/// <summary>
/// Configuration options for the local Vault state storage used to persist token and lease information.
/// </summary>
public class VaultStateOptions
{
    /// <summary>
    /// Gets or sets the directory path where Vault state files are stored.
    /// </summary>
    /// <value>A relative or absolute path to the state directory. Defaults to <c>.secrets/vault-state</c>.</value>
    public string StateDirectory { get; set; } = ".secrets/vault-state";
}
