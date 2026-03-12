namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Provides operations for reading and writing secrets in a HashiCorp Vault instance.
/// </summary>
public interface IKeyVaultService
{
    /// <summary>
    /// Retrieves all key-value pairs stored at the specified secret path.
    /// </summary>
    /// <param name="path">The secret path within the mount point.</param>
    /// <param name="mountPoint">The Vault secrets engine mount point.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A dictionary of secret key-value pairs, where values may be <see langword="null"/>.</returns>
    Task<IDictionary<string, string?>> GetSecretAsync(string path, string mountPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the supplied data as a secret at the specified path.
    /// </summary>
    /// <param name="path">The secret path within the mount point.</param>
    /// <param name="data">The key-value data to store as the secret.</param>
    /// <param name="mountPoint">The Vault secrets engine mount point.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    Task SetSecretAsync(string path, IDictionary<string, object> data, string mountPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all child paths beneath the specified path.
    /// </summary>
    /// <param name="path">The path to list within the mount point.</param>
    /// <param name="mountPoint">The Vault secrets engine mount point.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of child path names.</returns>
    Task<IReadOnlyList<string>> ListPathsAsync(string path, string mountPoint, CancellationToken cancellationToken = default);
}
