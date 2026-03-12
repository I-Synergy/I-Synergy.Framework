namespace ISynergy.Framework.KeyVault.Options;

/// <summary>
/// Options for configuring the local vault state persistence.
/// </summary>
public class VaultStateOptions
{
    /// <summary>
    /// Gets or sets the directory path used to persist vault state on disk.
    /// </summary>
    /// <value>Defaults to <c>.secrets/vault-state</c>.</value>
    public string StateDirectory { get; set; } = ".secrets/vault-state";
}
