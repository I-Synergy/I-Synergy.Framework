using System;
using System.Linq;
using System.Linq.Expressions;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// Class TypeFinder.
    /// Implements the <see cref="ITypeFinder" />
    /// </summary>
    /// <seealso cref="ITypeFinder" />
    internal class TypeFinder : ITypeFinder
    {
        /// <summary>
        /// The keywords helper
        /// </summary>
        private readonly IKeywordsHelper _keywordsHelper;
        /// <summary>
        /// The parsing configuration
        /// </summary>
        private readonly ParsingConfig _parsingConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFinder"/> class.
        /// </summary>
        /// <param name="parsingConfig">The parsing configuration.</param>
        /// <param name="keywordsHelper">The keywords helper.</param>
        public TypeFinder(ParsingConfig parsingConfig, IKeywordsHelper keywordsHelper)
        {
            Argument.IsNotNull(nameof(parsingConfig), parsingConfig);
            Argument.IsNotNull(nameof(keywordsHelper), keywordsHelper);

            _keywordsHelper = keywordsHelper;
            _parsingConfig = parsingConfig;
        }

        /// <summary>
        /// Finds the name of the type by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expressions">The expressions.</param>
        /// <param name="forceUseCustomTypeProvider">if set to <c>true</c> [force use custom type provider].</param>
        /// <returns>Type.</returns>
        public Type FindTypeByName(string name, ParameterExpression[] expressions, bool forceUseCustomTypeProvider)
        {
            Argument.IsNotNullOrEmpty(nameof(name), name);

            _keywordsHelper.TryGetValue(name, out var type);

            var result = type as Type;
            if (result != null)
            {
                return result;
            }

            if (expressions != null && TryResolveTypeUsingExpressions(name, expressions, out result))
            {
                return result;
            }

            return ResolveTypeByUsingCustomTypeProvider(name, forceUseCustomTypeProvider);
        }

        /// <summary>
        /// Resolves the type by using custom type provider.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="forceUseCustomTypeProvider">if set to <c>true</c> [force use custom type provider].</param>
        /// <returns>Type.</returns>
        private Type ResolveTypeByUsingCustomTypeProvider(string name, bool forceUseCustomTypeProvider)
        {
            if ((forceUseCustomTypeProvider || _parsingConfig.AllowNewToEvaluateAnyType) && _parsingConfig.CustomTypeProvider != null)
            {
                var resolvedType = _parsingConfig.CustomTypeProvider.ResolveType(name);
                if (resolvedType != null)
                {
                    return resolvedType;
                }

                // In case the type is not found based on fullname, try to get the type on simplename if allowed
                if (_parsingConfig.ResolveTypesBySimpleName)
                {
                    return _parsingConfig.CustomTypeProvider.ResolveTypeBySimpleName(name);
                }
            }

            return null;
        }

        /// <summary>
        /// Tries the resolve type using expressions.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expressions">The expressions.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool TryResolveTypeUsingExpressions(string name, ParameterExpression[] expressions, out Type result)
        {
            foreach (var expression in expressions.Where(e => e != null))
            {
                if (name == expression.Type.Name)
                {
                    result = expression.Type;
                    return true;
                }

                if (name == $"{expression.Type.Namespace}.{expression.Type.Name}")
                {
                    result = expression.Type;
                    return true;
                }

                if (_parsingConfig.ResolveTypesBySimpleName && _parsingConfig.CustomTypeProvider != null)
                {
                    var possibleFullName = $"{expression.Type.Namespace}.{name}";
                    var resolvedType = _parsingConfig.CustomTypeProvider.ResolveType(possibleFullName);
                    if (resolvedType != null)
                    {
                        result = resolvedType;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }
    }
}
