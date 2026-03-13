using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI.Extensions;
using System.Diagnostics.CodeAnalysis;
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
    /// <exception cref="ArgumentException">Thrown when value is not an enum value of the configured type.</exception>
    /// <exception cref="ArgumentException">Thrown when the parameter is not an enum name string.</exception>
    /// <remarks>
    /// Uses <see cref="Type.GetType(string)"/> to resolve the enum type from the <see cref="EnumType"/> string property.
    /// This pattern is not compatible with UWP .NET Native or IL trimming because the type name is opaque to the linker.
    /// Declare the enum type in <c>Default.rd.xml</c> for UWP .NET Native, or migrate to pass the type via
    /// a strongly-typed binding. See the UWP AOT migration guide for details.
    /// </remarks>
    [RequiresUnreferencedCode(
        "Type.GetType(string) is used to resolve the enum type at runtime. Under UWP .NET Native, " +
        "declare the enum type in Default.rd.xml. For AOT-safe usage, migrate EnumType from string to Type.")]
    [RequiresDynamicCode(
        "Enum.IsDefined and Enum.Parse require dynamic code for runtime type resolution.")]
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
    /// <exception cref="ArgumentException">Thrown when the parameter is not an enum name string.</exception>
    /// <remarks>
    /// Uses <see cref="Type.GetType(string)"/> to resolve the enum type at runtime.
    /// Not trim-safe — see <see cref="Convert"/> remarks.
    /// </remarks>
    [RequiresUnreferencedCode(
        "Type.GetType(string) is used to resolve the enum type at runtime. This is not trim-safe.")]
    [RequiresDynamicCode(
        "Enum.Parse requires dynamic code for runtime type resolution.")]
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
    /// <returns>A list of <see cref="KeyValuePair{TKey,TValue}"/> with the enum values and their descriptions.</returns>
    /// <remarks>
    /// Uses <see cref="Enum.GetValues(Type)"/> with a runtime type argument. Under UWP .NET Native,
    /// the enum type must be declared in <c>Default.rd.xml</c> with <c>Browse="Required All"</c>.
    /// For AOT-safe scenarios, prefer <c>Enum.GetValues&lt;TEnum&gt;()</c> in ViewModel code.
    /// </remarks>
    [RequiresUnreferencedCode("Enum.GetValues(Type) requires the enum type members to be preserved by the trimmer / declared in Default.rd.xml.")]
    [RequiresDynamicCode("Enum.GetValues requires dynamic code generation for runtime type resolution.")]
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
    /// <exception cref="NotImplementedException">Always thrown — back-conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <param name="value">The enum value to describe.</param>
    /// <returns>A localized description string for the enum value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
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
    /// <param name="parameter">
    /// An optional fully-qualified type name string used to identify the enum type.
    /// Using a string type name as the parameter is not AOT-safe; under UWP .NET Native,
    /// declare the enum type in <c>Default.rd.xml</c> or use a strongly-typed binding.
    /// </param>
    /// <param name="language">The language.</param>
    /// <returns>A localized description string for the enum value.</returns>
    /// <remarks>
    /// When <paramref name="parameter"/> is a non-empty string, this method calls
    /// <see cref="Type.GetType(string)"/> which is not compatible with UWP .NET Native or IL trimming.
    /// The string-type-name pattern is preserved for backwards compatibility but annotated as trim-unsafe.
    /// </remarks>
    [RequiresUnreferencedCode(
        "When 'parameter' is a string type name, Type.GetType(string) is used which is not trim-safe. " +
        "Under UWP .NET Native, declare the type in Default.rd.xml. Pass the enum value directly for the AOT-safe path.")]
    [RequiresDynamicCode("Enum.Parse requires dynamic code for runtime type resolution.")]
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
