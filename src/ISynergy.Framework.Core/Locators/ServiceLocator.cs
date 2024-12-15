using ISynergy.Framework.Core.Services;

namespace ISynergy.Framework.Core.Locators;

/// <summary>
/// This class provides the ambient container for this application. If your
/// framework defines such an ambient container, use ServiceLocator.Current
/// to get it.
/// </summary>
public class ServiceLocator : ScopedContextService
{
    private static IServiceProvider _staticServiceProvider;
    private static ServiceLocator _default;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
    /// </summary>
    /// <param name="currentServiceProvider">The current service provider.</param>
    public ServiceLocator(IServiceProvider currentServiceProvider)
        : base(currentServiceProvider)
    {
        _staticServiceProvider = currentServiceProvider;
        _default = this;
    }

    /// <summary>
    /// Gets the current.
    /// </summary>
    /// <value>The current.</value>
    public static ServiceLocator Default => _default ?? new ServiceLocator(_staticServiceProvider);


    /// <summary>
    /// Sets the locator provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _staticServiceProvider = serviceProvider;
        _default = new ServiceLocator(serviceProvider);
    }
}
