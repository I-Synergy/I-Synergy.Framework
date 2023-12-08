using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.UI.Converters;

public class ListToStringConverter : IValueConverter
{
    /// <summary>
    /// This method is used to convert list to string Converts list value to an string
    /// </summary>
    /// <param name="value">The value must be the type of list</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">separator</param>
    /// <param name="culture"></param>
    /// <returns>Returns the string value of the list</returns>
    /// <exception cref="ArgumentException"></exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var separator = string.Empty;

        if (!string.IsNullOrEmpty(parameter.ToString()))
            separator = parameter.ToString();

        if (value != null)
        {
            var collection = value as ICollection;

            if (collection != null)
            {
                var values = collection.OfType<object>().Select(s => s.ToString());
                return string.Join(separator, values);
            }

            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
            defaultInterpolatedStringHandler.AppendLiteral("Value is of ");
            defaultInterpolatedStringHandler.AppendFormatted<Func<Type>>(value.GetType);
            throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
