namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Defines a contract for retrieving the authentication token used to communicate with a vault service.
/// </summary>
public interface IVaultTokenProvider
{
    /// <summary>
    /// Retrieves the authentication token for the vault service.
    /// </summary>
    /// <returns>A non-null, non-empty token string.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a token cannot be resolved from any available source.
    /// </exception>
    string GetToken();
}
