using ISynergy.Framework.KeyVault.Abstractions.Services;
using ISynergy.Framework.KeyVault.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ISynergy.Framework.KeyVault.OpenBao.Services;

/// <summary>
/// Provides vault authentication tokens for OpenBao by resolving them from configuration
/// or a persisted vault-state file written by the VaultInitializer.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Security notice:</strong> This provider supports reading the root token either from
/// <see cref="KeyVaultOptions.Token"/> (for backward compatibility only) or from a local
/// <c>vault-state.json</c> file. Both mechanisms carry inherent risks in production environments:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <strong>Preferred:</strong> Set the <c>VAULT_TOKEN</c> environment variable and supply it via
///     <see cref="KeyVaultOptions.Token"/> (sourced from the environment, not from <c>appsettings.json</c>).
///   </description></item>
///   <item><description>
///     <strong>vault-state.json:</strong> This file is written in plaintext by the VaultInitializer.
///     It must be stored in a directory that is readable only by the service account running the
///     application (mode 600 on Linux). Ensure the path is excluded from source control and from
///     any file-system backups that are not encrypted at rest.
///   </description></item>
///   <item><description>
///     <strong>Production recommendation:</strong> Replace token auth with AppRole auth or Kubernetes
///     auth to eliminate long-lived root tokens entirely.
///   </description></item>
/// </list>
/// </remarks>
public sealed class OpenBaoVaultTokenProvider(
    IOptions<KeyVaultOptions> keyVaultOptions,
    IOptions<VaultStateOptions> vaultStateOptions) : IVaultTokenProvider
{
    /// <summary>
    /// Retrieves the root token used to authenticate with the OpenBao vault.
    /// </summary>
    /// <remarks>
    /// Token resolution order:
    /// <list type="number">
    ///   <item><description><see cref="KeyVaultOptions.Token"/>, if present and non-empty.</description></item>
    ///   <item><description>The <c>RootToken</c> field read from the JSON vault-state file whose path is
    ///       resolved via <see cref="VaultStateOptions.StateDirectory"/>, defaulting to <c>.secrets/vault-state</c>.
    ///       The environment variable <c>VaultStateOptions__StateDirectory</c> is supported via the standard
    ///       configuration double-underscore convention.</description></item>
    /// </list>
    /// A leading <c>~/</c> in the state-directory path is expanded to the current user's home directory.
    /// </remarks>
    /// <returns>The root token string required to authenticate vault API calls.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no token is configured and the vault-state file does not exist at the resolved path,
    /// or when the state file exists but does not contain a <c>RootToken</c> property.
    /// </exception>
    public string GetToken()
    {
        // 1. Explicit token in options (Obsolete but supported for backward compatibility).
        //    Suppress the obsolete warning here because this class is explicitly the consumer of that property.
#pragma warning disable CS0618
        var token = keyVaultOptions.Value.Token;
#pragma warning restore CS0618

        if (!string.IsNullOrWhiteSpace(token))
            return token;

        // 2. Resolve state directory from VaultStateOptions
        var stateDirectory = vaultStateOptions.Value.StateDirectory;

        if (stateDirectory.StartsWith("~/"))
            stateDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                stateDirectory[2..]);

        var stateFilePath = Path.Combine(stateDirectory, "vault-state.json");

        if (!File.Exists(stateFilePath))
            throw new InvalidOperationException(
                $"KeyVaultOptions:Token is not configured and vault state file was not found at '{stateFilePath}'.");

        using var doc = JsonDocument.Parse(File.ReadAllText(stateFilePath));
        var rootToken = doc.RootElement.GetProperty("RootToken").GetString()
            ?? throw new InvalidOperationException("RootToken is missing from vault state file.");

        return rootToken;
    }
}
