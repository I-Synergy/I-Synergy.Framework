using System.Globalization;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Providers;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// Class ParsingConfig.
    /// </summary>
    public class ParsingConfig
    {
        /// <summary>
        /// Default ParsingConfig
        /// </summary>
        /// <value>The default.</value>
        public static ParsingConfig Default { get; } = new ParsingConfig();

        /// <summary>
        /// The custom type provider
        /// </summary>
        private IDynamicLinkCustomTypeProvider _customTypeProvider;
        /// <summary>
        /// The expression promoter
        /// </summary>
        private IExpressionPromoter _expressionPromoter;
        /// <summary>
        /// The queryable analyzer
        /// </summary>
        private IQueryableAnalyzer _queryableAnalyzer;

        /// <summary>
        /// Gets or sets the <see cref="IDynamicLinkCustomTypeProvider" />.
        /// </summary>
        /// <value>The custom type provider.</value>
        public IDynamicLinkCustomTypeProvider CustomTypeProvider
        {
            get
            {
                return _customTypeProvider ??= new DefaultDynamicLinqCustomTypeProvider();
            }
            set
            {
                if (_customTypeProvider != value)
                {
                    _customTypeProvider = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IExpressionPromoter" />.
        /// </summary>
        /// <value>The expression promoter.</value>
        public IExpressionPromoter ExpressionPromoter
        {
            get => _expressionPromoter ??= new ExpressionPromoter(this);
            set
            {
                if (_expressionPromoter != value)
                {
                    _expressionPromoter = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IQueryableAnalyzer" />.
        /// </summary>
        /// <value>The queryable analyzer.</value>
        public IQueryableAnalyzer QueryableAnalyzer
        {
            get
            {
                return _queryableAnalyzer ??= new DefaultQueryableAnalyzer();
            }
            set
            {
                if (_queryableAnalyzer != value)
                {
                    _queryableAnalyzer = value;
                }
            }
        }

        /// <summary>
        /// Determines if the context keywords (it, parent, and root) are valid and usable inside a Dynamic Linq string expression.
        /// Does not affect the usability of the equivalent context symbols ($, ^ and ~).
        /// Default value is true.
        /// </summary>
        /// <value><c>true</c> if [are context keywords enabled]; otherwise, <c>false</c>.</value>
        public bool AreContextKeywordsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use dynamic object class for anonymous types.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [use dynamic object class for anonymous types]; otherwise, <c>false</c>.</value>
        public bool UseDynamicObjectClassForAnonymousTypes { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the EntityFramework version supports evaluating GroupBy at database level.
        /// See https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-2.1#linq-groupby-translation
        /// Remark: when this setting is set to 'true', make sure to supply this ParsingConfig as first parameter on the extension methods.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [evaluate group by at database]; otherwise, <c>false</c>.</value>
        public bool EvaluateGroupByAtDatabase { get; set; } = false;

        /// <summary>
        /// Use Parameterized Names in generated dynamic SQL query.
        /// See https://github.com/graeme-hill/gblog/blob/master/source_content/articles/2014.139_entity-framework-dynamic-queries-and-parameterization.mkd
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [use parameterized names in dynamic query]; otherwise, <c>false</c>.</value>
        public bool UseParameterizedNamesInDynamicQuery { get; set; } = false;

        /// <summary>
        /// Allows the New() keyword to evaluate any available Type.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [allow new to evaluate any type]; otherwise, <c>false</c>.</value>
        public bool AllowNewToEvaluateAnyType { get; set; } = false;

        /// <summary>
        /// Renames the (Typed)ParameterExpression empty Name to a the correct supplied name from `it`.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [rename parameter expression]; otherwise, <c>false</c>.</value>
        public bool RenameParameterExpression { get; set; } = false;

        /// <summary>
        /// By default when a member is not found in a type and the type has a string based index accessor it will be parsed as an index accessor. Use
        /// this flag to disable this behaviour and have parsing fail when parsing an expression
        /// where a member access on a non existing member happens.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [disable member access to index accessor fallback]; otherwise, <c>false</c>.</value>
        public bool DisableMemberAccessToIndexAccessorFallback { get; set; } = false;

        /// <summary>
        /// By default finding types by a simple name is not supported.
        /// Use this flag to use the CustomTypeProvider to resolve types by a simple name like "Employee" instead of "MyDatabase.Entities.Employee".
        /// Note that a first matching type is returned and this functionality needs to scan all types from all assemblies, so use with caution.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [resolve types by simple name]; otherwise, <c>false</c>.</value>
        public bool ResolveTypesBySimpleName { get; set; } = false;

        /// <summary>
        /// Support enumeration-types from the System namespace in mscorlib. An example could be "StringComparison".
        /// Default value is true.
        /// </summary>
        /// <value><c>true</c> if [support enumerations from system namespace]; otherwise, <c>false</c>.</value>
        public bool SupportEnumerationsFromSystemNamespace { get; set; } = true;

        /// <summary>
        /// By default DateTime (like 'Fri, 10 May 2019 11:03:17 GMT') is parsed as local time.
        /// Use this flag to parse all DateTime strings as UTC.
        /// Default value is false.
        /// </summary>
        /// <value><c>true</c> if [date time is parsed as UTC]; otherwise, <c>false</c>.</value>
        public bool DateTimeIsParsedAsUTC { get; set; } = false;

        /// <summary>
        /// The number parsing culture.
        /// Default value is CultureInfo.InvariantCulture
        /// </summary>
        /// <value>The number parse culture.</value>
        public CultureInfo NumberParseCulture { get; set; } = CultureInfo.InvariantCulture;
    }
}
