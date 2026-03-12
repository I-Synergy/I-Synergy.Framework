namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Provides an abstraction for obtaining an authentication token used to access a vault service.
/// </summary>
public interface IVaultTokenProvider
{
    /// <summary>
    /// Returns the authentication token for the vault service.
    /// </summary>
    /// <returns>A string containing the vault authentication token.</returns>
    string GetToken();
}
