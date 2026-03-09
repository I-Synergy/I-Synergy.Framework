namespace ISynergy.Framework.KeyVault.Abstractions.Services;

public interface IKeyVaultService
{
    Task<IDictionary<string, string?>> GetSecretAsync(string path, string mountPoint, CancellationToken cancellationToken = default);
    Task SetSecretAsync(string path, IDictionary<string, object> data, string mountPoint, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListPathsAsync(string path, string mountPoint, CancellationToken cancellationToken = default);
}
