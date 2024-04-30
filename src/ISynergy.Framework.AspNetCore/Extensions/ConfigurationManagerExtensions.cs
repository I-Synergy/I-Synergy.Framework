using Microsoft.Extensions.Configuration;

namespace ISynergy.Framework.AspNetCore.Extensions;
public static class ConfigurationManagerExtensions
{
    public static IConfigurationSection GetSetting(this ConfigurationManager configuration, string key) =>
        configuration.GetSection($"Settings:{key}");
}
