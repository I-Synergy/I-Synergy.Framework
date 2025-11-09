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
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var separator = parameter?.ToString() ?? string.Empty;

        if (value is ICollection collection)
        {
            var values = collection.OfType<object?>().Select(s => s?.ToString() ?? string.Empty);
            return string.Join(separator, values);
        }

        if (value != null)
        {
            throw new ArgumentException($"Value is of type {value.GetType}. Expected ICollection.");
        }

        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
