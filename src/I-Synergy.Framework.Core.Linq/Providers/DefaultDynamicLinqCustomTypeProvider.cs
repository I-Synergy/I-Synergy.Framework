using System;
using System.Collections.Generic;
using System.Reflection;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Attributes;
using ISynergy.Framework.Core.Linq.Helpers;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Providers
{
    /// <summary>
    /// The default implementation for <see cref="IDynamicLinkCustomTypeProvider" />.
    /// Scans the current AppDomain for all types marked with <see cref="DynamicLinqTypeAttribute" />, and adds them as custom Dynamic Link types.
    /// Also provides functionality to resolve a Type in the current Application Domain.
    /// This class is used as default for full .NET Framework, so not for .NET Core
    /// </summary>
    public class DefaultDynamicLinqCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        /// <summary>
        /// The assembly helper
        /// </summary>
        private readonly IAssemblyHelper _assemblyHelper = new AssemblyHelper();
        /// <summary>
        /// The cache custom types
        /// </summary>
        private readonly bool _cacheCustomTypes;

        /// <summary>
        /// The cached custom types
        /// </summary>
        private HashSet<Type> _cachedCustomTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDynamicLinqCustomTypeProvider" /> class.
        /// </summary>
        /// <param name="cacheCustomTypes">Defines whether to cache the CustomTypes which are found in the Application Domain. Default set to 'true'.</param>
        public DefaultDynamicLinqCustomTypeProvider(bool cacheCustomTypes = true)
        {
            _cacheCustomTypes = cacheCustomTypes;
        }

        /// <summary>
        /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
        /// </summary>
        /// <returns>A <see cref="HashSet{Type}" /> list of custom types.</returns>
        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.GetCustomTypes" />
        public virtual HashSet<Type> GetCustomTypes()
        {
            if (_cacheCustomTypes)
            {
                if (_cachedCustomTypes == null)
                {
                    _cachedCustomTypes = GetCustomTypesInternal();
                }

                return _cachedCustomTypes;
            }

            return GetCustomTypesInternal();
        }

        /// <summary>
        /// Resolve any type by fullname which is registered in the current application domain.
        /// </summary>
        /// <param name="typeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type" /> or null when not found.</returns>
        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.ResolveType" />
        public Type ResolveType(string typeName)
        {
            Argument.IsNotNullOrEmpty(nameof(typeName), typeName);

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return ResolveType(assemblies, typeName);
        }

        /// <summary>
        /// Resolve any type by the simple name which is registered in the current application domain.
        /// </summary>
        /// <param name="simpleTypeName">The typename to resolve.</param>
        /// <returns>A resolved <see cref="Type" /> or null when not found.</returns>
        /// <inheritdoc cref="IDynamicLinkCustomTypeProvider.ResolveTypeBySimpleName" />
        public Type ResolveTypeBySimpleName(string simpleTypeName)
        {
            Argument.IsNotNullOrEmpty(nameof(simpleTypeName), simpleTypeName);

            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return ResolveTypeBySimpleName(assemblies, simpleTypeName);
        }

        /// <summary>
        /// Gets the custom types internal.
        /// </summary>
        /// <returns>HashSet&lt;Type&gt;.</returns>
        private HashSet<Type> GetCustomTypesInternal()
        {
            IEnumerable<Assembly> assemblies = _assemblyHelper.GetAssemblies();
            return new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(assemblies));
        }
    }
}
