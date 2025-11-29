using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.UI.Extensions;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

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
    public required Type EnumType { get; set; }

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized()</exception>
    /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized()</exception>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is string enumString)
        {
            if (!Enum.IsDefined(EnumType, value!))
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());

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
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="ArgumentException">ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized()</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Only update the bound value when this RadioButton is checked
        if (value is bool isChecked && isChecked && parameter is string enumString)
            return Enum.Parse(EnumType, enumString);

        // Return DoNothing to prevent unchecked RadioButtons from updating the binding
        return Binding.DoNothing;
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
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var list = new List<KeyValuePair<int, string>>();

        if (value is Enum) list.AddRange(from Enum item in Enum.GetValues(value.GetType()) select new KeyValuePair<int, string>(System.Convert.ToInt32(item), item.GetLocalizedDescription()));

        return list;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
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
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var paramString = parameter?.ToString();
        if (!string.IsNullOrEmpty(paramString) && Type.GetType(paramString) is { } type && type.IsEnum && value is not null)
            return (Enum.Parse(type, value.ToString()!) as Enum)?.GetLocalizedDescription();

        if (value is not null)
            return (Enum.Parse(value.GetType(), value.ToString()!) as Enum)?.GetLocalizedDescription();

        return null;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Class EnumToStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class EnumToDescriptionConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Enum enumeration)
            return enumeration.GetLocalizedDescription();

        return value;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

