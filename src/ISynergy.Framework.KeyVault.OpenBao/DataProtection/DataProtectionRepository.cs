using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using VaultSharp;

namespace ISynergy.Framework.KeyVault.OpenBao.DataProtection;

/// <summary>
/// Stores ASP.NET Core Data Protection keys in OpenBao KV v2.
/// Keys are stored at {mountPoint}/{keyPath}/{friendlyName} as JSON: { "xml": "&lt;key /&gt;" }
/// </summary>
public class DataProtectionRepository : IXmlRepository
{
    private readonly IVaultClient _vaultClient;
    private readonly string _keyPath;
    private readonly string _mountPoint;
    private readonly ILogger<DataProtectionRepository> _logger;

    public DataProtectionRepository(
        IVaultClient vaultClient,
        string keyPath,
        string mountPoint,
        ILogger<DataProtectionRepository> logger)
    {
        _vaultClient = vaultClient;
        _keyPath = keyPath;
        _mountPoint = mountPoint;
        _logger = logger;
    }

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var elements = new List<XElement>();

        try
        {
            var paths = _vaultClient.V1.Secrets.KeyValue.V2
                .ReadSecretPathsAsync(_keyPath, mountPoint: _mountPoint)
                .GetAwaiter().GetResult();

            foreach (var keyName in paths.Data.Keys)
            {
                try
                {
                    var secret = _vaultClient.V1.Secrets.KeyValue.V2
                        .ReadSecretAsync<Dictionary<string, string>>($"{_keyPath}/{keyName}", mountPoint: _mountPoint)
                        .GetAwaiter().GetResult();

                    if (secret.Data.Data.TryGetValue("xml", out var xml) && !string.IsNullOrEmpty(xml))
                        elements.Add(XElement.Parse(xml));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read data protection key {KeyName} from OpenBao", keyName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to list data protection keys from OpenBao at path {KeyPath}", _keyPath);
        }

        return elements.AsReadOnly();
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        var data = new Dictionary<string, object>
        {
            ["xml"] = element.ToString(SaveOptions.DisableFormatting)
        };

        _vaultClient.V1.Secrets.KeyValue.V2
            .WriteSecretAsync($"{_keyPath}/{friendlyName}", data, mountPoint: _mountPoint)
            .GetAwaiter().GetResult();

        _logger.LogDebug("Stored data protection key {FriendlyName} to OpenBao", friendlyName);
    }
}
