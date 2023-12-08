using Microsoft.Extensions.Configuration;

namespace ISynergy.Framework.Core.Extensions;

/// <summary>
/// Class ConfigurationBinderExtensions.
/// </summary>
public static class ConfigurationBinderExtensions
{
    /// <summary>
    /// Binds the with reload.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="instance">The instance.</param>
    public static void BindWithReload(this IConfiguration configuration, object instance)
    {
        configuration.Bind(instance);
        configuration.GetReloadToken().RegisterChangeCallback((_) => configuration.Bind(instance), null);
    }
}
