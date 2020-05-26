using System;
using System.ComponentModel;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Converters;
using ISynergy.Framework.Core.Linq.Parsers;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Factories
{
    /// <summary>
    /// Class TypeConverterFactory.
    /// Implements the <see cref="ITypeConverterFactory" />
    /// </summary>
    /// <seealso cref="ITypeConverterFactory" />
    internal class TypeConverterFactory : ITypeConverterFactory
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly ParsingConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterFactory"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public TypeConverterFactory(ParsingConfig config)
        {
            Argument.IsNotNull(nameof(config), config);

            _config = config;
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeConverter.</returns>
        /// <see cref="ITypeConverterFactory.GetConverter" />
        public TypeConverter GetConverter(Type type)
        {
            Argument.IsNotNull(nameof(type), type);

            if (_config.DateTimeIsParsedAsUTC && (type == typeof(DateTime) || type == typeof(DateTime?)))
            {
                return new CustomDateTimeConverter();
            }

            return TypeDescriptor.GetConverter(type);
        }
    }
}
