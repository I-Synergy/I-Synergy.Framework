using System;
using System.ComponentModel;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Converters;
using ISynergy.Framework.Core.Linq.Parsers;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Factories
{
    internal class TypeConverterFactory : ITypeConverterFactory
    {
        private readonly ParsingConfig _config;

        public TypeConverterFactory(ParsingConfig config)
        {
            Argument.IsNotNull(nameof(config), config);

            _config = config;
        }

        /// <see cref="ITypeConverterFactory.GetConverter"/>
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
