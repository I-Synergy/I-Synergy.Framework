namespace ISynergy.Framework.KeyVault.Abstractions.Services;

/// <summary>
/// Provides an abstraction for interacting with a key vault (e.g. HashiCorp Vault),
/// allowing secrets to be retrieved, stored, and enumerated by path and mount point.
/// </summary>
public interface IKeyVaultService
{
    /// <summary>
    /// Retrieves the secret values stored at the specified path within a given mount point.
    /// </summary>
    /// <param name="path">The path within the mount point where the secret is stored.</param>
    /// <param name="mountPoint">The mount point (engine path) under which the secret resides.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A dictionary of key/value pairs representing the secret data at the given path,
    /// where values may be <see langword="null"/>.
    /// </returns>
    Task<IDictionary<string, string?>> GetSecretAsync(string path, string mountPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes or updates the secret values at the specified path within a given mount point.
    /// </summary>
    /// <param name="path">The path within the mount point where the secret should be stored.</param>
    /// <param name="data">A dictionary of key/value pairs representing the secret data to store.</param>
    /// <param name="mountPoint">The mount point (engine path) under which the secret will reside.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetSecretAsync(string path, IDictionary<string, object> data, string mountPoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all child paths (keys or sub-paths) available under the specified path within a given mount point.
    /// </summary>
    /// <param name="path">The path within the mount point to list children of.</param>
    /// <param name="mountPoint">The mount point (engine path) under which the path resides.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list of child path names relative to the specified path.</returns>
    Task<IReadOnlyList<string>> ListPathsAsync(string path, string mountPoint, CancellationToken cancellationToken = default);
}
