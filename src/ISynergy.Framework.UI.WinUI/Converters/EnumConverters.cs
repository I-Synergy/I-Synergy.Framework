using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.UI.Xaml.Data;
using System.Diagnostics.CodeAnalysis;

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
    /// This method uses <see cref="Type.GetType(string)"/> to resolve the enum type from the
    /// <see cref="EnumType"/> string property. This pattern is not compatible with NativeAOT or
    /// IL trimming because the type name is opaque to the linker. To make this converter AOT-safe,
    /// change XAML usages from <c>EnumType="Namespace.MyEnum"</c> to pass the type via a
    /// strongly-typed binding or use a generic alternative.
    /// </remarks>
    [UnconditionalSuppressMessage("Trimming", "IL2046",
        Justification = "IValueConverter is an external WinUI SDK interface without RequiresUnreferencedCode; suppressing annotation mismatch.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "Type.GetType(string) is intentionally used for XAML-driven enum resolution; callers must ensure enum types are preserved.")]
    [UnconditionalSuppressMessage("Trimming", "IL2057",
        Justification = "Type.GetType(string) with a runtime string is intentional for XAML enum resolution; the caller must ensure the type is preserved.")]
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Enum.IsDefined and Enum.Parse require dynamic code; not AOT-safe by design.")]
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string enumString)
        {
            var type = Type.GetType(EnumType);

            if (type is not null)
            {
                if (!Enum.IsDefined(type, value))
                {
                    throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum".GetLocalized());
                }

                var enumValue = Enum.Parse(type, enumString);

                return enumValue.Equals(value);
            }
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
    /// This method uses <see cref="Type.GetType(string)"/> to resolve the enum type at runtime.
    /// Not trim-safe — see <see cref="Convert"/> remarks.
    /// </remarks>
    [UnconditionalSuppressMessage("Trimming", "IL2046",
        Justification = "IValueConverter is an external WinUI SDK interface without RequiresUnreferencedCode; suppressing annotation mismatch.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "Type.GetType(string) is intentionally used for XAML-driven enum resolution; callers must ensure enum types are preserved.")]
    [UnconditionalSuppressMessage("Trimming", "IL2057",
        Justification = "Type.GetType(string) with a runtime string is intentional for XAML enum resolution; the caller must ensure the type is preserved.")]
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Enum.Parse requires dynamic code; not AOT-safe by design.")]
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string enumString)
        {
            return Enum.Parse(Type.GetType(EnumType)!, enumString);
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
    /// <returns>A list of <see cref="KeyValuePair{TKey,TValue}"/> with the enum values and their descriptions.</returns>
    /// <remarks>
    /// Uses <see cref="Enum.GetValues(Type)"/> with a runtime type argument, which requires enum type
    /// metadata to be preserved by the trimmer. For NativeAOT compatibility, prefer
    /// <c>Enum.GetValues&lt;TEnum&gt;()</c> in ViewModel code instead of this converter.
    /// </remarks>
    [UnconditionalSuppressMessage("Trimming", "IL2046",
        Justification = "IValueConverter is an external WinUI SDK interface without RequiresUnreferencedCode; suppressing annotation mismatch.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "Enum.GetValues(Type) is intentionally used for XAML-driven enum resolution; callers must ensure enum types are preserved.")]
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Enum.GetValues requires dynamic code; not AOT-safe by design.")]
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var list = new List<KeyValuePair<int, string>>();

        if (value is Enum)
        {
            list.AddRange(from Enum item in Enum.GetValues(value.GetType()) select new KeyValuePair<int, string>(System.Convert.ToInt32(item), GetDescription(item)));
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
public class EnumToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">
    /// An optional fully-qualified type name string used to identify the enum type.
    /// Using a string type name as the parameter is not AOT-safe; prefer passing the enum value
    /// directly without a parameter, or supply the type via a strongly-typed binding.
    /// </param>
    /// <param name="language">The culture.</param>
    /// <returns>A localized description string for the enum value.</returns>
    /// <remarks>
    /// When <paramref name="parameter"/> is a non-empty string, this method calls
    /// <see cref="Type.GetType(string)"/> which is not compatible with NativeAOT or IL trimming.
    /// The string-type-name pattern is preserved for backwards compatibility but annotated as
    /// trim-unsafe. Prefer passing the enum value directly (without a string parameter) to avoid
    /// the trim-unsafe code path.
    /// </remarks>
    [UnconditionalSuppressMessage("Trimming", "IL2046",
        Justification = "IValueConverter is an external WinUI SDK interface without RequiresUnreferencedCode; suppressing annotation mismatch.")]
    [UnconditionalSuppressMessage("Trimming", "IL2026",
        Justification = "Type.GetType(string) is intentionally used for XAML-driven enum resolution; callers must ensure enum types are preserved.")]
    [UnconditionalSuppressMessage("Trimming", "IL2057",
        Justification = "Type.GetType(string) with a runtime string is intentional for XAML enum resolution; the caller must ensure the type is preserved.")]
    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Enum.Parse requires dynamic code; not AOT-safe by design.")]
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (!string.IsNullOrEmpty(parameter.ToString()) && Type.GetType(parameter.ToString()!) is { } type && type.IsEnum)
            return (Enum.Parse(type, value.ToString()!) as Enum)!.GetLocalizedDescription();

        return (Enum.Parse(value.GetType(), value.ToString()!) as Enum)!.GetLocalizedDescription();
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="language">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
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
    /// <param name="language">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
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
    /// <param name="language">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class EnumToIntegerConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="language">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Enum enumeration)
            return System.Convert.ToInt32(enumeration);

        return 0;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="language">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is int id && Enum.TryParse(targetType, id.ToString(), out var result))
            return result;

        return default;
    }
}
