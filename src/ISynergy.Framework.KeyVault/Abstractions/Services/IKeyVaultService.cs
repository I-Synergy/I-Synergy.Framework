namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Defines the contract for interacting with a secret store (e.g. HashiCorp Vault).
/// </summary>
public interface IKeyVaultService
{
    /// <summary>
    /// Retrieves a secret from the specified path and mount point.
    /// </summary>
    /// <param name="path">The path within the mount point where the secret is stored.</param>
    /// <param name="mountPoint">The Vault secrets engine mount point (e.g. <c>secret</c>).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A dictionary mapping secret field names to their values, or <see langword="null"/> values
    /// for fields that are present but have no value.
    /// </returns>
    Task<IDictionary<string, string?>> GetSecretAsync(string path, string mountPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes or updates a secret at the specified path and mount point.
    /// </summary>
    /// <param name="path">The path within the mount point where the secret should be written.</param>
    /// <param name="data">A dictionary of key/value pairs representing the secret data to store.</param>
    /// <param name="mountPoint">The Vault secrets engine mount point (e.g. <c>secret</c>).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
    Task SetSecretAsync(string path, IDictionary<string, object> data, string mountPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all secret paths available under the specified path and mount point.
    /// </summary>
    /// <param name="path">The path prefix within the mount point to list.</param>
    /// <param name="mountPoint">The Vault secrets engine mount point (e.g. <c>secret</c>).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A read-only list of path strings found at the given location.</returns>
    Task<IReadOnlyList<string>> ListPathsAsync(string path, string mountPoint, CancellationToken cancellationToken = default);
}
