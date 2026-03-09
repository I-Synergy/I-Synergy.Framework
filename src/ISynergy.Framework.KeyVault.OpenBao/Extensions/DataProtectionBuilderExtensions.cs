using ISynergy.Framework.KeyVault.DataProtection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace ISynergy.Framework.KeyVault.Extensions;

public static class DataProtectionBuilderExtensions
{
    /// <summary>
    /// Configures Data Protection to persist keys in OpenBao KV v2.
    /// </summary>
    /// <param name="builder">The data protection builder.</param>
    /// <param name="vaultUri">The OpenBao URI, e.g. http://localhost:8200</param>
    /// <param name="vaultToken">The OpenBao token.</param>
    /// <param name="keyPath">The KV path under which keys are stored. Default: isynergy/dataprotection</param>
    /// <param name="mountPoint">The KV v2 mount point. Default: secret</param>
    public static IDataProtectionBuilder PersistKeysToOpenBao(
        this IDataProtectionBuilder builder,
        string vaultUri,
        string vaultToken,
        string keyPath = "isynergy/dataprotection",
        string mountPoint = "secret")
    {
        builder.Services.AddOptions<KeyManagementOptions>()
            .Configure<ILoggerFactory>((options, loggerFactory) =>
            {
                var authMethod = new TokenAuthMethodInfo(vaultToken);
                var clientSettings = new VaultClientSettings(vaultUri, authMethod);
                var client = new VaultClient(clientSettings);

                options.XmlRepository = new OpenBaoDataProtectionRepository(
                    client,
                    keyPath,
                    mountPoint,
                    loggerFactory.CreateLogger<OpenBaoDataProtectionRepository>());
            });

        return builder;
    }
}
