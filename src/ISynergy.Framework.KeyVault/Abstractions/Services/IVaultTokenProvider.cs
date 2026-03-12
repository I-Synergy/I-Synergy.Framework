namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Provides a Vault authentication token for use with the HashiCorp Vault API.
/// </summary>
public interface IVaultTokenProvider
{
    /// <summary>
    /// Returns the current Vault authentication token.
    /// </summary>
    /// <returns>The Vault token string.</returns>
    string GetToken();
}
