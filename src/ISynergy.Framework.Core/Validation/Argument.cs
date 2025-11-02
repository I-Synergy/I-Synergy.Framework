using ISynergy.Framework.Core.Services;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Validation;

#pragma warning disable CS8603

/// <summary>
/// Class Argument.
/// </summary>
[DebuggerNonUserCode, DebuggerStepThrough]
public static class Argument
{
    /// <summary>
    /// Determines whether the specified argument is not <c>null</c>.
    /// </summary>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    [return: NotNull]
    public static T IsNotNull<T>(T value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (value is null)
        {
            var error = LanguageService.Default.GetString("WarningNull");
            throw new ArgumentNullException(name, error);
        }

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not <c>null</c> or empty.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <exception cref="ArgumentNullException"></exception>
    [return: NotNull]
    public static string IsNotNullOrEmpty(string? value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (string.IsNullOrEmpty(value))
        {
            var error = LanguageService.Default.GetString("WarningNullOrEmpty");
            throw new ArgumentNullException(name, error);
        }

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not empty.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static Guid IsNotEmpty(Guid value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (value == Guid.Empty)
        {
            var error = LanguageService.Default.GetString("WarningGuidEmpty");
            throw new ArgumentException(error, name);
        }

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not <c>null</c> or empty.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static Guid IsNotNullOrEmpty(Guid? value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (!value.HasValue || value.Value == Guid.Empty)
        {
            var error = LanguageService.Default.GetString("WarningNullGuidEmpty");
            throw new ArgumentNullException(name, error);
        }

        return value.Value;
    }

    /// <summary>
    /// Determines whether the specified argument is not <c>null</c> or a whitespace.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static string IsNotNullOrWhitespace(string value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (string.IsNullOrEmpty(value) || (string.CompareOrdinal(value.Trim(), string.Empty) == 0))
        {
            var error = LanguageService.Default.GetString("WarningNullWhitespace");
            throw new ArgumentNullException(name, error);
        }

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static Array IsNotNullOrEmptyArray(Array value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if ((value is null) || (value.Length == 0))
        {
            var error = LanguageService.Default.GetString("WarningNullEmptyArray");
            throw new ArgumentNullException(name, error);
        }

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNull]
    public static IList<T> IsNotNullOrEmptyList<T>(IList<T> value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if ((value is null) || (value.Count == 0))
        {
            var error = LanguageService.Default.GetString("WarningNullEmptyList");
            throw new ArgumentNullException(name, error);
        }

        return value;
    }

    /// <summary>
    /// Determines whether [is not enum] [the specified parameter name].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">The parameter value.</param>
    /// <exception cref="System.ArgumentException"></exception>
    [return: NotNull]
    public static T IsNotNullEnum<T>(T value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (value is null || !value.GetType().IsEnum)
        {
            var error = LanguageService.Default.GetString("WarningEnum");
            throw new ArgumentException(error, name);
        }

        return value;
    }

    public static bool IsEnumType([NotNullWhen(true)] Type? value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (value is null || !value.IsEnum)
        {
            var error = LanguageService.Default.GetString("WarningEnum");
            throw new ArgumentException(error, name);
        }

        return true;
    }

    /// <summary>
    /// Determines whether [has no nulls] [the specified parameter name].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">The value.</param>
    /// <returns>IList&lt;T&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IList<T> HasNoNulls<T>([NotNull] IList<T> value, [CallerArgumentExpression(nameof(value))] string? name = "")
        where T : class
    {
        IsNotNull(value);

        if (value.Any(e => e is null))
            throw new ArgumentNullException(name);

        return value;
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <param name="validation">The validation function to call for validation.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="validation" /> is <c>null</c>.</exception>
    public static void IsNotOutOfRange<T>(T value, T minimumValue, T maximumValue, Func<T, T, T, bool> validation, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        IsNotNull(validation);

        if (!validation(value, minimumValue, maximumValue))
        {
            var error = string.Format(LanguageService.Default.GetString("WarningBetween"), minimumValue, maximumValue);
            throw new ArgumentOutOfRangeException(name, error);
        }
    }

    /// <summary>
    /// Determines whether the specified argument is not out of range.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    public static void IsNotOutOfRange<T>(T value, T minimumValue, T maximumValue, [CallerArgumentExpression(nameof(value))] string? name = "")
        where T : IComparable
    {
        IsNotOutOfRange(value, minimumValue, maximumValue,
            (innerParamValue, innerMinimumValue, innerMaximumValue) => innerParamValue.CompareTo(innerMinimumValue) >= 0 && innerParamValue.CompareTo(innerMaximumValue) <= 0,
            name);
    }

    /// <summary>
    /// Determines whether the specified argument has a minimum value.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <param name="validation">The validation function to call for validation.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="validation" /> is <c>null</c>.</exception>
    public static void IsMinimal<T>(T value, T minimumValue, Func<T, T, bool> validation, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        IsNotNull(validation);

        if (!validation(value, minimumValue))
        {
            var error = string.Format(LanguageService.Default.GetString("WarningMinimum"), minimumValue);
            throw new ArgumentOutOfRangeException(name, error);
        }
    }

    /// <summary>
    /// Determines whether the specified argument has a minimum value.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="minimumValue">The minimum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    public static void IsMinimal<T>(T value, T minimumValue, [CallerArgumentExpression(nameof(value))] string? name = "")
        where T : IComparable
    {
        IsMinimal(value, minimumValue,
            (innerParamValue, innerMinimumValue) => innerParamValue.CompareTo(innerMinimumValue) >= 0,
            name);
    }

    /// <summary>
    /// Determines whether the specified argument has a maximum value.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <param name="validation">The validation function to call for validation.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="validation" /> is <c>null</c>.</exception>
    public static void IsMaximum<T>(T value, T maximumValue, Func<T, T, bool> validation, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (!validation(value, maximumValue))
        {
            var error = string.Format(LanguageService.Default.GetString("WarningMaximum"), maximumValue);
            throw new ArgumentOutOfRangeException(name, error);
        }
    }

    /// <summary>
    /// Determines whether the specified argument has a maximum value.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    /// <param name="maximumValue">The maximum value.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value" /> is out of range.</exception>
    public static void IsMaximum<T>(T value, T maximumValue, [CallerArgumentExpression(nameof(value))] string? name = "")
        where T : IComparable
    {
        IsMaximum(value, maximumValue,
            (innerParamValue, innerMaximumValue) => innerParamValue.CompareTo(innerMaximumValue) <= 0,
            name);
    }

    /// <summary>
    /// Conditions the specified parameter name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">The value.</param>
    /// <param name="condition">The condition.</param>
    /// <returns>T.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static T Condition<T>(T value, Predicate<T> condition, [CallerArgumentExpression(nameof(value))] string? name = "")
        where T : IComparable
    {
        IsNotNull(value);
        IsNotNull(condition);

        if (!condition(value))
            throw new ArgumentOutOfRangeException(name);

        return value;
    }

    /// <summary>
    /// Conditions the specified parameter name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">The value.</param>
    /// <param name="compareValue"></param>
    /// <returns>T.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static T Equals<T>(T value, T compareValue, [CallerArgumentExpression(nameof(value))] string? name = "")
        where T : IComparable
    {
        IsNotNull(value);
        IsNotNull(compareValue);

        if (value.Equals(compareValue))
            throw new ArgumentException($"Property '{name}' cannot have the same value as the comparer value.");

        return value;
    }
}
