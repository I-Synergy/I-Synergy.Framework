using ISynergy.Framework.KeyVault.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ISynergy.Framework.KeyVault.Services;

public sealed class OpenBaoVaultTokenProvider(IConfiguration configuration) : IVaultTokenProvider
{
    public string GetToken()
    {
        // 1. Explicit token in config
        var token = configuration["KeyVaultOptions:Token"];

        if (!string.IsNullOrWhiteSpace(token))
            return token;

        // 2. Resolve state directory: env var > config > fallback
        var stateDirectory = Environment.GetEnvironmentVariable("VaultInitializer__StateDirectory")
            ?? configuration["VaultInitializer:StateDirectory"]
            ?? ".secrets/vault-state";

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
