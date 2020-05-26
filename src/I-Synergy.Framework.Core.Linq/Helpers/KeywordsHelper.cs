using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;

namespace ISynergy.Framework.Core.Linq.Helpers
{
    /// <summary>
    /// Class KeywordsHelper.
    /// Implements the <see cref="IKeywordsHelper" />
    /// </summary>
    /// <seealso cref="IKeywordsHelper" />
    internal class KeywordsHelper : IKeywordsHelper
    {
        /// <summary>
        /// The symbol it
        /// </summary>
        public const string SYMBOL_IT = "$";
        /// <summary>
        /// The symbol parent
        /// </summary>
        public const string SYMBOL_PARENT = "^";
        /// <summary>
        /// The symbol root
        /// </summary>
        public const string SYMBOL_ROOT = "~";

        /// <summary>
        /// The keyword it
        /// </summary>
        public const string KEYWORD_IT = "it";
        /// <summary>
        /// The keyword parent
        /// </summary>
        public const string KEYWORD_PARENT = "parent";
        /// <summary>
        /// The keyword root
        /// </summary>
        public const string KEYWORD_ROOT = "root";

        /// <summary>
        /// The function iif
        /// </summary>
        public const string FUNCTION_IIF = "iif";
        /// <summary>
        /// The function isnull
        /// </summary>
        public const string FUNCTION_ISNULL = "isnull";
        /// <summary>
        /// The function new
        /// </summary>
        public const string FUNCTION_NEW = "new";
        /// <summary>
        /// The function nullpropagation
        /// </summary>
        public const string FUNCTION_NULLPROPAGATION = "np";
        /// <summary>
        /// The function is
        /// </summary>
        public const string FUNCTION_IS = "is";
        /// <summary>
        /// The function as
        /// </summary>
        public const string FUNCTION_AS = "as";
        /// <summary>
        /// The function cast
        /// </summary>
        public const string FUNCTION_CAST = "cast";

        /// <summary>
        /// The keywords
        /// </summary>
        private readonly IDictionary<string, object> _keywords = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { "true", Expression.Constant(true) },
            { "false", Expression.Constant(false) },
            { "null", Expression.Constant(null) }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordsHelper"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public KeywordsHelper(ParsingConfig config)
        {
            if (config.AreContextKeywordsEnabled)
            {
                _keywords.Add(KEYWORD_IT, KEYWORD_IT);
                _keywords.Add(KEYWORD_PARENT, KEYWORD_PARENT);
                _keywords.Add(KEYWORD_ROOT, KEYWORD_ROOT);
            }

            _keywords.Add(SYMBOL_IT, SYMBOL_IT);
            _keywords.Add(SYMBOL_PARENT, SYMBOL_PARENT);
            _keywords.Add(SYMBOL_ROOT, SYMBOL_ROOT);

            _keywords.Add(FUNCTION_IIF, FUNCTION_IIF);
            _keywords.Add(FUNCTION_ISNULL, FUNCTION_ISNULL);
            _keywords.Add(FUNCTION_NEW, FUNCTION_NEW);
            _keywords.Add(FUNCTION_NULLPROPAGATION, FUNCTION_NULLPROPAGATION);
            _keywords.Add(FUNCTION_IS, FUNCTION_IS);
            _keywords.Add(FUNCTION_AS, FUNCTION_AS);
            _keywords.Add(FUNCTION_CAST, FUNCTION_CAST);

            foreach (var type in PredefinedTypesHelper.PredefinedTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
            {
                if (!string.IsNullOrEmpty(type.FullName))
                {
                    _keywords[type.FullName] = type;
                }
                _keywords[type.Name] = type;
            }

            foreach (var pair in PredefinedTypesHelper.PredefinedTypesShorthands)
            {
                _keywords.Add(pair.Key, pair.Value);
            }

            if (config.SupportEnumerationsFromSystemNamespace)
            {
                foreach (var type in EnumerationsFromMscorlib.PredefinedEnumerationTypes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
                {
                    if (!string.IsNullOrEmpty(type.FullName))
                    {
                        _keywords[type.FullName] = type;
                    }
                    _keywords[type.Name] = type;
                }
            }

            if (config.CustomTypeProvider != null)
            {
                foreach (var type in config.CustomTypeProvider.GetCustomTypes())
                {
                    _keywords[type.FullName] = type;
                    _keywords[type.Name] = type;
                }
            }
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryGetValue(string name, out object type)
        {
            return _keywords.TryGetValue(name, out type);
        }
    }
}
