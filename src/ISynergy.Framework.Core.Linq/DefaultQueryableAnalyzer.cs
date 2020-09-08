using System.Linq;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq
{
    /// <summary>
    /// Default implementation.
    /// </summary>
    /// <seealso cref="IQueryableAnalyzer" />
    public class DefaultQueryableAnalyzer : IQueryableAnalyzer
    {
        /// <summary>
        /// Determines whether the specified query (and provider) supports LinqToObjects.
        /// </summary>
        /// <param name="query">The query to check.</param>
        /// <param name="provider">The provider to check (can be null).</param>
        /// <returns>true/false</returns>
        /// <inheritdoc cref="IQueryableAnalyzer.SupportsLinqToObjects" />
        public bool SupportsLinqToObjects(IQueryable query, IQueryProvider provider = null)
        {
            Argument.IsNotNull(nameof(query), query);

            provider = provider ?? query.Provider;

            var providerType = provider.GetType();
            var baseType = providerType.GetTypeInfo().BaseType;
            var isLinqToObjects = baseType == typeof(EnumerableQuery);

            if (!isLinqToObjects)
            {
                // Support for https://github.com/StefH/QueryInterceptor.Core, version 1.0.1 and up
                if (providerType.Name.StartsWith("QueryTranslatorProvider"))
                {
                    try
                    {
                        var property = providerType.GetProperty("OriginalProvider");

                        if (property != null)
                        {
                            return property.GetValue(provider, null) is IQueryProvider originalProvider && SupportsLinqToObjects(query, originalProvider);
                        }

                        return SupportsLinqToObjects(query);
                    }
                    catch
                    {
                        return false;
                    }
                }

                // Support for https://github.com/scottksmith95/LINQKit ExpandableQuery
                if (providerType.Name.StartsWith("ExpandableQuery"))
                {
                    try
                    {
                        var property = query.GetType().GetProperty("InnerQuery", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (property != null)
                        {
                            return property.GetValue(query, null) is IQueryable innerQuery && SupportsLinqToObjects(innerQuery, provider);
                        }

                        return SupportsLinqToObjects(query);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return isLinqToObjects;
        }
    }
}
