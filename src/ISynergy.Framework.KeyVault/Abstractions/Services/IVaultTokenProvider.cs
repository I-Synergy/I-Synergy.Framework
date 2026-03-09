namespace ISynergy.Framework.KeyVault.Abstractions.Services;

public interface IVaultTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}
