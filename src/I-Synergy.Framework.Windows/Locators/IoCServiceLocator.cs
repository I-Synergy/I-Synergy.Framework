using CommonServiceLocator;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Services
{
    /// <summary>
    /// Implementation of ServiceLocator for IoC container,
    /// see rationale at https://commonservicelocator.codeplex.com/
    /// </summary>
    public class IoCServiceLocator : ServiceLocatorImplBase
    {
        /// <summary>Exposes underlying Container for direct operation.</summary>
        public readonly IContainer Container;

        /// <summary>Creates new locator as adapter for provided container.</summary>
        /// <param name="container">Container to use/adapt.</param>
        public IoCServiceLocator(IContainer container)
        {
            Container = container ?? throw new ArgumentNullException("container");
        }

        /// <summary>Resolves service from container. Throws if unable to resolve.</summary>
        /// <param name="serviceType">Service type to resolve.</param>
        /// <param name="key">(optional) Service key to resolve.</param>
        /// <returns>Resolved service object.</returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            return Container.Resolve(serviceType, key);
        }

        /// <summary>Returns enumerable which when enumerated! resolves all default and named
        /// implementations/registrations of requested service type.
        /// If no services resolved when enumerable accessed, no exception is thrown - enumerable is empty.</summary>
        /// <param name="serviceType">Service type to resolve.</param>
        /// <returns>Returns enumerable which will return resolved service objects.</returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            return Container.ResolveMany<object>(serviceType);
        }
    }
}
