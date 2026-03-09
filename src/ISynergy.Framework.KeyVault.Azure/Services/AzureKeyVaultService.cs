using Azure.Security.KeyVault.Secrets;
using ISynergy.Framework.KeyVault.Abstractions.Services;

namespace ISynergy.Framework.KeyVault.Services;

internal sealed class AzureKeyVaultService(SecretClient secretClient) : IKeyVaultService
{
    // Azure Key Vault uses flat key naming, not hierarchical mount/path like OpenBao.
    // path is mapped as "{mountPoint}--{path-with-dashes}" to comply with Azure naming.
    public async Task<IDictionary<string, string?>> GetSecretAsync(string path, string mountPoint, CancellationToken cancellationToken = default)
    {
        var keyName = BuildSecretName(mountPoint, path);
        var response = await secretClient.GetSecretAsync(keyName, cancellationToken: cancellationToken);
        return new Dictionary<string, string?> { ["value"] = response.Value.Value };
    }

    public async Task SetSecretAsync(string path, IDictionary<string, object> data, string mountPoint, CancellationToken cancellationToken = default)
    {
        foreach (var (key, value) in data)
        {
            var secretName = BuildSecretName(mountPoint, $"{path}/{key}");
            await secretClient.SetSecretAsync(secretName, value?.ToString(), cancellationToken);
        }
    }

    public Task<IReadOnlyList<string>> ListPathsAsync(string path, string mountPoint, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Azure Key Vault does not support path listing in the same way as OpenBao.");

    private static string BuildSecretName(string mountPoint, string path)
        => $"{mountPoint}--{path}".Replace('/', '-');
}
