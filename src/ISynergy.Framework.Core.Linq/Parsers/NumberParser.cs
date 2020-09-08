using System;
using ISynergy.Framework.Core.Linq.Helpers;

namespace ISynergy.Framework.Core.Linq.Parsers
{
    /// <summary>
    /// NumberParser
    /// </summary>
    public class NumberParser
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly ParsingConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberParser" /> class.
        /// </summary>
        /// <param name="config">The ParsingConfig.</param>
        public NumberParser(ParsingConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Parses the number (text) into the specified type.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object ParseNumber(string text, Type type)
        {
            try
            {
                var tp = TypeHelper.GetNonNullableType(type);
                
                if(tp.IsEnum)
                {
                    return int.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(sbyte))
                {
                    return sbyte.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(byte))
                {
                    return byte.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(short))
                {
                    return short.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(ushort))
                {
                    return ushort.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(int))
                {
                    return int.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(uint))
                {
                    return uint.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(long))
                {
                    return long.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(ulong))
                {
                    return ulong.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(float))
                {
                    return float.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(double))
                {
                    return double.Parse(text, _config.NumberParseCulture);
                }
                if (tp == typeof(decimal))
                {
                    return decimal.Parse(text, _config.NumberParseCulture);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
