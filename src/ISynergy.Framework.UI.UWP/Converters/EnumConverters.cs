using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI.Extensions;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class EnumToBooleanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public partial class EnumToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets the type of the enum.
    /// </summary>
    /// <value>The type of the enum.</value>
    public string EnumType { get; set; } = string.Empty;

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
            var type = Type.GetType(EnumType);
            if (type is null)
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
            }

            if (!Enum.IsDefined(type, value))
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
            }

            var enumValue = Enum.Parse(type, enumString);

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
            var type = Type.GetType(EnumType);
            if (type is null)
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
            }
            return Enum.Parse(type, enumString);
        }

        throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName".GetLocalized());
    }
}

/// <summary>
/// Class EnumToArrayConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public partial class EnumToArrayConverter : IValueConverter
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

        return ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(value.ToString());
    }
}

/// <summary>
/// Class EnumToStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public partial class EnumToStringConverter : IValueConverter
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
        if (parameter is not null && !string.IsNullOrEmpty(parameter.ToString()) && Type.GetType(parameter.ToString()!) is Type type && type.IsEnum)
        {
            if (value is not null && value.ToString() is string valueString)
            {
                return GetDescription(Enum.Parse(type, valueString) as Enum ?? throw new ArgumentException("Invalid enum value"));
            }
        }

        if (value is not null && value.ToString() is string valueStr)
        {
            return GetDescription(Enum.Parse(value.GetType(), valueStr) as Enum ?? throw new ArgumentException("Invalid enum value"));
        }

        return string.Empty;
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
        if (fieldInfo is null)
        {
            return description ?? string.Empty;
        }
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes is not null && attributes.Length > 0)
        {
            description = ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(attributes[0].Description);
        }

        return description;
    }
}
