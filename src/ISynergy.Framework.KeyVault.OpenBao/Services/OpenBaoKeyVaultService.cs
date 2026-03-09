using ISynergy.Framework.KeyVault.Abstractions.Services;
using VaultSharp;

namespace ISynergy.Framework.KeyVault.Services;

public sealed class OpenBaoKeyVaultService(IVaultClient vaultClient) : IKeyVaultService
{
    public async Task<IDictionary<string, string?>> GetSecretAsync(string path, string mountPoint, CancellationToken cancellationToken = default)
    {
        var secret = await vaultClient.V1.Secrets.KeyValue.V2
            .ReadSecretAsync<Dictionary<string, string>>(path, mountPoint: mountPoint)
            .WaitAsync(cancellationToken);
        return secret.Data.Data.ToDictionary(k => k.Key, v => (string?)v.Value);
    }

    public async Task SetSecretAsync(string path, IDictionary<string, object> data, string mountPoint, CancellationToken cancellationToken = default)
    {
        await vaultClient.V1.Secrets.KeyValue.V2
            .WriteSecretAsync(path, data, mountPoint: mountPoint)
            .WaitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ListPathsAsync(string path, string mountPoint, CancellationToken cancellationToken = default)
    {
        var result = await vaultClient.V1.Secrets.KeyValue.V2
            .ReadSecretPathsAsync(path, mountPoint: mountPoint)
            .WaitAsync(cancellationToken);
        return result.Data.Keys.ToList();
    }

    public async Task WaitUntilUnsealedAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var status = await vaultClient.V1.System.GetSealStatusAsync();
            if (!status.Sealed) return;
            await Task.Delay(2000, cancellationToken);
        }
    }
}
