namespace ISynergy.Framework.AspNetCore.Options;
public class AzureOptions
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    /// <value>The tenant identifier.</value>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the keyvault.
    /// </summary>
    public KeyVaultOptions? KeyVault { get; set; }
}