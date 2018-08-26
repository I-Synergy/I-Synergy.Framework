using Microsoft.Extensions.Configuration;

namespace ISynergy.Extensions
{
    public static class ConfigurationBinderExtensions
    {
        public static void BindWithReload(this IConfiguration configuration, object instance)
        {
            configuration.Bind(instance);
            configuration.GetReloadToken().RegisterChangeCallback((obj) => configuration.Bind(instance), null);
        }
    }
}