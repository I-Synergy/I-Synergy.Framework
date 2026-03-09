using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ISynergy.Framework.KeyVault.Abstractions.Services;
using ISynergy.Framework.KeyVault.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.KeyVault.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureKeyVaultIntegration(this IServiceCollection services, string vaultUri)
    {
        services.TryAddSingleton(_ => new SecretClient(new Uri(vaultUri), new DefaultAzureCredential()));
        services.TryAddSingleton<IKeyVaultService, AzureKeyVaultService>();
        return services;
    }
}
