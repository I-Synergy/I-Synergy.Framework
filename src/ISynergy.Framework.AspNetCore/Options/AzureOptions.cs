namespace ISynergy.Framework.AspNetCore.Options;
public class AzureOptions
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    /// <value>The tenant identifier.</value>
    public required string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the keyvault.
    /// </summary>
    public required KeyVaultOptions KeyVault { get; set; }
}