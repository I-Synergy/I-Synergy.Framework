using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Core.Extensions;

public static class ServiceProviderExtensions
{
    // Static dictionary to store service collections
    private static readonly Dictionary<IServiceProvider, IServiceCollection> _serviceCollections = new();

    public static IServiceProvider BuildServiceProviderWithLocator(this IServiceCollection services, bool serviceLocator = false)
    {
        var serviceProvider = services.BuildServiceProvider();

        if (serviceLocator)
            ServiceLocator.SetLocatorProvider(serviceProvider);

        _serviceCollections[serviceProvider] = services;

        return serviceProvider;
    }

    public static IEnumerable<ServiceDescriptor> GetRegisteredServices(this IServiceProvider serviceProvider)
    {
        return _serviceCollections.TryGetValue(serviceProvider, out var services)
            ? services
            : Enumerable.Empty<ServiceDescriptor>();
    }
}