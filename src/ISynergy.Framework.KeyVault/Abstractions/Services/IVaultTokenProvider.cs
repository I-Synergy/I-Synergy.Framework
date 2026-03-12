namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Provides a Vault authentication token for use when connecting to the Vault server.
/// </summary>
public interface IVaultTokenProvider
{
    /// <summary>
    /// Returns the current Vault authentication token.
    /// </summary>
    /// <returns>A non-empty string containing the Vault token.</returns>
    string GetToken();
}
