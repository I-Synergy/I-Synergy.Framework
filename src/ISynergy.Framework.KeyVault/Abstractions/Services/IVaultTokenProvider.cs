namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Defines a provider that supplies authentication tokens for accessing Azure Key Vault.
/// </summary>
public interface IVaultTokenProvider
{
    /// <summary>
    /// Retrieves the authentication token used to access the vault.
    /// </summary>
    /// <returns>A string containing the authentication token.</returns>
    string GetToken();
}
