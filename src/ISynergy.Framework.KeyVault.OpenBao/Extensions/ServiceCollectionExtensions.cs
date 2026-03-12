using ISynergy.Framework.KeyVault.Abstractions.Services;
using ISynergy.Framework.KeyVault.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace ISynergy.Framework.KeyVault.Extensions;

/// <summary>
/// Service collection extensions for OpenBao Key Vault integration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenBao Key Vault integration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> VaultSharp uses reflection-based JSON serialization internally
    /// and does not currently carry full Native AOT annotations. Applications targeting
    /// <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should suppress <c>IL2026</c> and <c>IL3050</c>
    /// warnings at the call site or consider using <c>ISynergy.Framework.KeyVault.Azure</c> with an explicit
    /// credential type, which is fully AOT-safe.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("VaultSharp uses reflection-based JSON serialization internally. Verify AOT compatibility with the VaultSharp version in use.")]
    [RequiresDynamicCode("VaultSharp may require dynamic code generation for JSON deserialization.")]
    public static IServiceCollection AddKeyVaultIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<IVaultTokenProvider, OpenBaoVaultTokenProvider>();
        services.TryAddSingleton<IKeyVaultService>(sp =>
        {
            var tokenProvider = sp.GetRequiredService<IVaultTokenProvider>();

            var vaultUri = configuration["KeyVaultOptions:Uri"]
                ?? throw new InvalidOperationException("KeyVaultOptions:Uri not configured.");

            var token = tokenProvider.GetToken();
            var client = new VaultClient(new VaultClientSettings(vaultUri, new TokenAuthMethodInfo(token)));

            return new OpenBaoKeyVaultService(client);
        });
        return services;
    }
}
