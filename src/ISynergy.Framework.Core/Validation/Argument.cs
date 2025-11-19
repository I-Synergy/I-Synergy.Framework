using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
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
    /// Gets an error message from the language service, with fallback to default message.
    /// </summary>
    private static string GetErrorMessage(string key, string defaultMessage)
    {
        try
        {
            return ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString(key);
        }
        catch
        {
            // ServiceLocator may not be initialized (e.g., during early initialization or unit tests)
            return defaultMessage;
        }
    }

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
            var error = GetErrorMessage("WarningNull", $"Parameter '{name}' cannot be null.");
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
            var error = GetErrorMessage("WarningNullOrEmpty", $"Parameter '{name}' cannot be null or empty.");
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
            var error = GetErrorMessage("WarningGuidEmpty", $"Parameter '{name}' cannot be an empty GUID.");
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
            var error = GetErrorMessage("WarningNullGuidEmpty", $"Parameter '{name}' cannot be null or an empty GUID.");
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
            var error = GetErrorMessage("WarningNullWhitespace", $"Parameter '{name}' cannot be null, empty, or whitespace.");
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
            var error = GetErrorMessage("WarningNullEmptyArray", $"Parameter '{name}' cannot be null or an empty array.");
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
            var error = GetErrorMessage("WarningNullEmptyList", $"Parameter '{name}' cannot be null or an empty list.");
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
            var error = GetErrorMessage("WarningEnum", $"Parameter '{name}' must be a valid enum value.");
            throw new ArgumentException(error, name);
        }

        return value;
    }

    public static bool IsEnumType([NotNullWhen(true)] Type? value, [CallerArgumentExpression(nameof(value))] string? name = "")
    {
        if (value is null || !value.IsEnum)
        {
            var error = GetErrorMessage("WarningEnum", $"Parameter '{name}' must be a valid enum type.");
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
            var errorMessage = GetErrorMessage("WarningBetween", "Value must be between {0} and {1}.");
            var error = string.Format(errorMessage, minimumValue, maximumValue);
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
            var errorMessage = GetErrorMessage("WarningMinimum", "Value must be at least {0}.");
            var error = string.Format(errorMessage, minimumValue);
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
            var errorMessage = GetErrorMessage("WarningMaximum", "Value must be at most {0}.");
            var error = string.Format(errorMessage, maximumValue);
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
