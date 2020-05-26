using Microsoft.Extensions.Configuration;

namespace ISynergy.Framework.Core.Extensions
{
    public static class ConfigurationBinderExtensions
    {
        public static void BindWithReload(this IConfiguration configuration, object instance)
        {
            configuration.Bind(instance);
            configuration.GetReloadToken().RegisterChangeCallback((_) => configuration.Bind(instance), null);
        }
    }
}
