using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using System;
using System.Collections.Generic;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml.Data;
#elif (NET5_0 && WINDOWS)
using Microsoft.UI.Xaml.Data;
#endif

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class EnumToBooleanConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the type of the enum.
        /// </summary>
        /// <value>The type of the enum.</value>
        public Type EnumType { get; set; }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized()</exception>
        /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized()</exception>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                if (!Enum.IsDefined(EnumType, value))
                {
                    throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
                }

                var enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized()</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
            {
                return Enum.Parse(EnumType, enumString);
            }

            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
        }
    }

    /// <summary>
    /// Class EnumToArrayConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class EnumToArrayConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = new List<KeyValuePair<int, string>>();

            if (value is Enum)
            {
                foreach (Enum item in Enum.GetValues(value.GetType()))
                {
                    list.Add(new KeyValuePair<int, string>(System.Convert.ToInt32(item), GetDescription(item)));
                }
            }

            return list;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentNullException">value</exception>
        public static string GetDescription(Enum value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return ServiceLocator.Default.GetInstance<ILanguageService>().GetString(value.ToString());
        }
    }

    /// <summary>
    /// Class EnumToStringConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!string.IsNullOrEmpty(parameter.ToString()) && Type.GetType(parameter.ToString()) is Type type && type.IsEnum)
            {
                return GetDescription(Enum.Parse(type, value.ToString()) as Enum);
            }

            return GetDescription(Enum.Parse(value.GetType(), value.ToString()) as Enum);
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentNullException">value</exception>
        public static string GetDescription(Enum value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var description = value.ToString();
            var fieldInfo = value.GetType().GetField(description);
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = ServiceLocator.Default.GetInstance<ILanguageService>().GetString(attributes[0].Description);
            }

            return description;
        }
    }
}
