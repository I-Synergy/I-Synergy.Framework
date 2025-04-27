using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.UI.Extensions;
public static class ServiceProviderExtensions
{
    public static IHost SetLocatorProvider(this IHost host)
    {
        ServiceLocator.SetLocatorProvider(host.Services);
        return host;
    }
}
