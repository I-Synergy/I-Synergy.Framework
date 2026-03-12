using ISynergy.Framework.KeyVault.Abstractions.Services;
using ISynergy.Framework.KeyVault.OpenBao.Services;
using ISynergy.Framework.KeyVault.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace ISynergy.Framework.KeyVault.OpenBao.Extensions;

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
    /// Registers <see cref="KeyVaultOptions"/> from the <c>KeyVaultOptions</c> configuration section
    /// and <see cref="VaultStateOptions"/> from the <c>VaultStateOptions</c> configuration section.
    /// </para>
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
        services.Configure<KeyVaultOptions>(configuration.GetSection(nameof(KeyVaultOptions)));
        services.Configure<VaultStateOptions>(configuration.GetSection(nameof(VaultStateOptions)));

        services.TryAddSingleton<IVaultTokenProvider, OpenBaoVaultTokenProvider>();
        services.TryAddSingleton<IKeyVaultService>(sp =>
        {
            var tokenProvider = sp.GetRequiredService<IVaultTokenProvider>();
            var keyVaultOptions = sp.GetRequiredService<IOptions<KeyVaultOptions>>().Value;

            if (string.IsNullOrWhiteSpace(keyVaultOptions.Uri))
                throw new InvalidOperationException($"{nameof(KeyVaultOptions)}:{nameof(KeyVaultOptions.Uri)} is not configured.");

            var token = tokenProvider.GetToken();
            var client = new VaultClient(new VaultClientSettings(keyVaultOptions.Uri, new TokenAuthMethodInfo(token)));

            return new OpenBaoKeyVaultService(client);
        });
        return services;
    }
}
