using System;
using System.Linq;
using System.Linq.Expressions;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    internal class TypeFinder : ITypeFinder
    {
        private readonly IKeywordsHelper _keywordsHelper;
        private readonly ParsingConfig _parsingConfig;

        public TypeFinder(ParsingConfig parsingConfig, IKeywordsHelper keywordsHelper)
        {
            Argument.IsNotNull(nameof(parsingConfig), parsingConfig);
            Argument.IsNotNull(nameof(keywordsHelper), keywordsHelper);

            _keywordsHelper = keywordsHelper;
            _parsingConfig = parsingConfig;
        }

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
