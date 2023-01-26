using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Validation
{
    /// <summary>
    /// Class Argument.
    /// </summary>
    public static class Argument
    {
        /// <summary>
        /// Determines whether the specified argument is not <c>null</c>.
        /// </summary>
        /// <param name="value">Value of the parameter.</param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNull(object value, [CallerArgumentExpression("value")] string name = null)
        {
            if (value is null)
            {
                var error = $"Argument '{name}' cannot be null";
                throw new ArgumentNullException(name, error);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmpty(string value, [CallerArgumentExpression("value")] string name = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                var error = $"Argument '{name}' cannot be null or empty";
                throw new ArgumentNullException(error, name);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not empty.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotEmpty(Guid value, [CallerArgumentExpression("value")] string name = null)
        {
            if (value == Guid.Empty)
            {
                var error = $"Argument '{name}' cannot be Guid.Empty";
                throw new ArgumentException(error, name);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmpty(Guid? value, [CallerArgumentExpression("value")] string name = null)
        {
            if (!value.HasValue || value.Value == Guid.Empty)
            {
                var error = $"Argument '{name}' cannot be null or Guid.Empty";
                throw new ArgumentNullException(error, name);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or a whitespace.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrWhitespace(string value, [CallerArgumentExpression("value")] string name = null)
        {
            if (string.IsNullOrEmpty(value) || (string.CompareOrdinal(value.Trim(), string.Empty) == 0))
            {
                var error = $"Argument '{name}' cannot be null or whitespace";
                throw new ArgumentNullException(error, name);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmptyArray(Array value, [CallerArgumentExpression("value")] string name = null)
        {
            if ((value is null) || (value.Length == 0))
            {
                var error = $"Argument '{name}' cannot be null or an empty array";
                throw new ArgumentNullException(error, name);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmptyList<T>(IList<T> value, [CallerArgumentExpression("value")] string name = null)
        {
            if ((value is null) || (value.Count == 0))
            {
                var error = $"Argument '{name}' cannot be null or an empty list";
                throw new ArgumentNullException(error, name);
            }
        }

        /// <summary>
        /// Determines whether [is not enum] [the specified parameter name].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="System.ArgumentException"></exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotEnum<T>(T value, [CallerArgumentExpression("value")] string name = null)
        {
            if (!typeof(T).IsEnum)
            {
                var error = $"Argument '{name}' can only be an Enum";
                throw new ArgumentException(error, name);
            }
        }

        /// <summary>
        /// Determines whether [has no nulls] [the specified parameter name].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<T> HasNoNulls<T>(IList<T> value, [CallerArgumentExpression("value")] string name = null)
            where T : class
        {
            IsNotNull(value);

            if (value.Any(e => e is null))
            {
                IsNotNullOrEmpty(name);

                throw new ArgumentNullException(name);
            }

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
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(T value, T minimumValue, T maximumValue, Func<T, T, T, bool> validation, [CallerArgumentExpression("value")] string name = null)
        {
            IsNotNull(validation);

            if (!validation(value, minimumValue, maximumValue))
            {
                var error = $"Argument '{name}' should be between {minimumValue} and {maximumValue}";
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
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(T value, T minimumValue, T maximumValue, [CallerArgumentExpression("value")] string name = null)
            where T : IComparable
        {
            IsNotOutOfRange(value, minimumValue, maximumValue,
                (innerParamValue, innerMinimumValue, innerMaximumValue) => innerParamValue.CompareTo(innerMinimumValue) >= 0 && innerParamValue.CompareTo(innerMaximumValue) <= 0);
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
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMinimal<T>(T value, T minimumValue, Func<T, T, bool> validation, [CallerArgumentExpression("value")] string name = null)
        {
            IsNotNull(validation);

            if (!validation(value, minimumValue))
            {
                var error = $"Argument '{name}' should be minimal {minimumValue}";
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
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMinimal<T>(T value, T minimumValue, [CallerArgumentExpression("value")] string name = null)
            where T : IComparable
        {
            IsMinimal(value, minimumValue,
                (innerParamValue, innerMinimumValue) => innerParamValue.CompareTo(innerMinimumValue) >= 0);
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
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMaximum<T>(T value, T maximumValue, Func<T, T, bool> validation, [CallerArgumentExpression("value")] string name = null)
        {
            if (!validation(value, maximumValue))
            {
                var error = $"Argument '{name}' should be at maximum {maximumValue}";
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
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMaximum<T>(T value, T maximumValue, [CallerArgumentExpression("value")] string name = null)
            where T : IComparable
        {
            IsMaximum(value, maximumValue,
                (innerParamValue, innerMaximumValue) => innerParamValue.CompareTo(innerMaximumValue) <= 0);
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
        public static T Condition<T>(T value, Predicate<T> condition, [CallerArgumentExpression("value")] string name = null)
        {
            IsNotNull(condition);
            IsNotNull(value);

            if (!condition(value))
            {
                IsNotNullOrEmpty(name);

                throw new ArgumentOutOfRangeException(name);
            }

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
        public static T Equals<T>(T value, T compareValue, [CallerArgumentExpression("value")] string name = null)
        {
            IsNotNull(compareValue);
            IsNotNull(value);

            if (value.Equals(compareValue))
            {
                IsNotNullOrEmpty(name);
                throw new ArgumentException($"Property '{name}' cannot have the same value as the comparer value.");
            }

            return value;
        }
    }
}
