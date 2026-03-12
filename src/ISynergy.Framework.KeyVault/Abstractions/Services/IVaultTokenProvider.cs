namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Provides a Vault authentication token for use when connecting to the Vault server.
/// </summary>
public interface IVaultTokenProvider
{
    /// <summary>
    /// Returns the authentication token for the vault service.
    /// </summary>
    /// <returns>A non-empty string containing the Vault token.</returns>
    string GetToken();
}
