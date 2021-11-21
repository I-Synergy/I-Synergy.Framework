using Microsoft.Extensions.DependencyInjection;
using System;

namespace ISynergy.Framework.Core.Locators
{
    /// <summary>
    /// This class provides the ambient container for this application. If your
    /// framework defines such an ambient container, use ServiceLocator.Current
    /// to get it.
    /// </summary>
    public class ServiceLocator
    {
        /// <summary>
        /// The current service provider
        /// </summary>
        private IServiceProvider _currentServiceProvider;
        /// <summary>
        /// The service provider
        /// </summary>
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        /// <param name="currentServiceProvider">The current service provider.</param>
        public ServiceLocator(IServiceProvider currentServiceProvider)
        {
            _currentServiceProvider = currentServiceProvider;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static ServiceLocator Default
        {
            get
            {
                return new ServiceLocator(_serviceProvider);
            }
        }

        /// <summary>
        /// Sets the locator provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>System.Object.</returns>
        public object GetInstance(Type serviceType) =>
            _currentServiceProvider.GetService(serviceType);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="TService">The type of the t service.</typeparam>
        /// <returns>TService.</returns>
        public TService GetInstance<TService>() =>
            _currentServiceProvider.GetService<TService>();
    }
}
