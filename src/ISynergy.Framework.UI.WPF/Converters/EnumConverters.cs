using System;
using System.Collections.Generic;
using System.Globalization;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Abstractions.Services;
using System.Windows.Data;
using System.Windows;

namespace ISynergy.Framework.UI.Converters
{
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
        /// <param name="culture">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
        /// Converts back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
            Argument.IsNotNull(nameof(value));
            return ServiceLocator.Default.GetInstance<ILanguageService>().GetString(value.ToString());
        }
    }
}
