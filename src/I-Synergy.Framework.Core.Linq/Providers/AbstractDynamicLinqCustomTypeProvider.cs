using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Attributes;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Providers
{
    /// <summary>
    /// The abstract DynamicLinqCustomTypeProvider which is used by the DefaultDynamicLinqCustomTypeProvider and can be used by a custom TypeProvider like in .NET Core.
    /// </summary>
    public abstract class AbstractDynamicLinqCustomTypeProvider
    {
        /// <summary>
        /// Finds the unique types marked with DynamicLinqTypeAttribute.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> FindTypesMarkedWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
        {
            Argument.IsNotNull(nameof(assemblies), assemblies);
            assemblies = assemblies.Where(a => !a.IsDynamic);
            return GetAssemblyTypesWithDynamicLinqTypeAttribute(assemblies).Distinct().ToArray();
        }

        /// <summary>
        /// Resolve any type which is registered in the current application domain.
        /// </summary>
        /// <param name="assemblies">The assemblies to inspect.</param>
        /// <param name="typeName">The type name to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        protected Type ResolveType(IEnumerable<Assembly> assemblies, string typeName)
        {
            Argument.IsNotNull(nameof(assemblies), assemblies);
            Argument.IsNotNullOrEmpty(nameof(typeName), typeName);

            foreach (var assembly in assemblies)
            {
                var resolvedType = assembly.GetType(typeName, false, true);
                if (resolvedType != null)
                {
                    return resolvedType;
                }
            }

            return null;
        }

        /// <summary>
        /// Resolve a type by the simple name which is registered in the current application domain.
        /// </summary>
        /// <param name="assemblies">The assemblies to inspect.</param>
        /// <param name="simpleTypeName">The simple type name to resolve.</param>
        /// <returns>A resolved <see cref="Type"/> or null when not found.</returns>
        protected Type ResolveTypeBySimpleName(IEnumerable<Assembly> assemblies, string simpleTypeName)
        {
            Argument.IsNotNull(nameof(assemblies), assemblies);
            Argument.IsNotNullOrEmpty(nameof(simpleTypeName), simpleTypeName);

            foreach (var assembly in assemblies)
            {
                var fullnames = assembly.GetTypes().Select(t => t.FullName).Distinct();
                var firstMatchingFullname = fullnames.FirstOrDefault(fn => fn.EndsWith($".{simpleTypeName}"));

                if (firstMatchingFullname != null)
                {
                    var resolvedType = assembly.GetType(firstMatchingFullname, false, true);
                    if (resolvedType != null)
                    {
                        return resolvedType;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the assembly types annotated with <see cref="DynamicLinqTypeAttribute"/> in an Exception friendly way.
        /// </summary>
        /// <param name="assemblies">The assemblies to process.</param>
        /// <returns><see cref="IEnumerable{Type}" /></returns>
        protected IEnumerable<Type> GetAssemblyTypesWithDynamicLinqTypeAttribute(IEnumerable<Assembly> assemblies)
        {
            Argument.IsNotNull(nameof(assemblies), assemblies);

            foreach (var assembly in assemblies)
            {
                Type[] definedTypes = null;

                try
                {
                    definedTypes = assembly.ExportedTypes.Where(t => t.GetTypeInfo().IsDefined(typeof(DynamicLinqTypeAttribute), false)).ToArray();
                }
                catch (ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    definedTypes = reflectionTypeLoadException.Types;
                }
                catch
                {
                    // Ignore all other exceptions
                }

                if (definedTypes != null && definedTypes.Length > 0)
                {
                    foreach (var definedType in definedTypes)
                    {
                        yield return definedType;
                    }
                }
            }
        }
    }
}
