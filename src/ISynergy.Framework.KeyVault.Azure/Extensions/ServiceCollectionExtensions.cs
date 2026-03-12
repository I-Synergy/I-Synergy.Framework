using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ISynergy.Framework.KeyVault.Abstractions.Services;
using ISynergy.Framework.KeyVault.Azure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.KeyVault.Azure.Extensions;

/// <summary>
/// Service collection extensions for Azure Key Vault integration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure Key Vault integration using <see cref="DefaultAzureCredential"/> for authentication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="vaultUri">The URI of the Azure Key Vault instance.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// <para>
    /// <strong>AOT/Trimming notice:</strong> <see cref="DefaultAzureCredential"/> probes multiple credential
    /// providers at runtime using reflection and carries <c>[RequiresUnreferencedCode]</c>. Applications
    /// targeting <c>&lt;PublishAot&gt;true&lt;/PublishAot&gt;</c> should use the
    /// <see cref="AddAzureKeyVaultIntegration(IServiceCollection, string, TokenCredential)"/> overload with an
    /// explicit credential type such as <see cref="ManagedIdentityCredential"/> or
    /// <see cref="ClientSecretCredential"/> instead.
    /// </para>
    /// </remarks>
    [RequiresUnreferencedCode("DefaultAzureCredential uses reflection to probe credential providers. For AOT publishing, use a specific credential type such as ClientSecretCredential or ManagedIdentityCredential.")]
    public static IServiceCollection AddAzureKeyVaultIntegration(this IServiceCollection services, string vaultUri)
    {
        services.TryAddSingleton(_ => new SecretClient(new Uri(vaultUri), new DefaultAzureCredential()));
        services.TryAddSingleton<IKeyVaultService, AzureKeyVaultService>();
        return services;
    }

    /// <summary>
    /// Adds Azure Key Vault integration using an explicit <see cref="TokenCredential"/> for authentication.
    /// This overload is fully AOT-safe.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="vaultUri">The URI of the Azure Key Vault instance.</param>
    /// <param name="credential">
    /// The credential used to authenticate with Azure Key Vault. Use a specific credential type such as
    /// <see cref="ManagedIdentityCredential"/>, <see cref="ClientSecretCredential"/>, or
    /// <see cref="WorkloadIdentityCredential"/> for AOT-compatible publishing.
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAzureKeyVaultIntegration(
        this IServiceCollection services,
        string vaultUri,
        TokenCredential credential)
    {
        services.TryAddSingleton(_ => new SecretClient(new Uri(vaultUri), credential));
        services.TryAddSingleton<IKeyVaultService, AzureKeyVaultService>();
        return services;
    }
}
