using ISynergy.Framework.KeyVault.Abstractions.Services;
using ISynergy.Framework.KeyVault.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace ISynergy.Framework.KeyVault.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenBaoKeyVault(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<IVaultTokenProvider, OpenBaoVaultTokenProvider>();
        services.TryAddSingleton<IKeyVaultService>(sp =>
        {
            var tokenProvider = sp.GetRequiredService<IVaultTokenProvider>();
            var vaultUri = configuration["KeyVaultOptions:Uri"]
                ?? throw new InvalidOperationException("KeyVaultOptions:Uri not configured.");
            var token = tokenProvider.GetTokenAsync().GetAwaiter().GetResult();
            var client = new VaultClient(new VaultClientSettings(vaultUri, new TokenAuthMethodInfo(token)));
            return new OpenBaoKeyVaultService(client);
        });
        return services;
    }
}
