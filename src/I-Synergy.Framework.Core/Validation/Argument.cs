using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ISynergy.Framework.Core.Validation
{
    public static class Argument
    {
        /// <summary>
        /// Determines whether the specified argument is not <c>null</c>.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException">If <paramref name="paramValue" /> is <c>null</c>.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNull(string paramName, object paramValue)
        {
            if (paramName != null)
            {
                if (paramValue is null)
                {
                    var error = $"Argument '{paramName.ToString()}' cannot be null";
                    throw new ArgumentNullException(paramName, error);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException">If <paramref name="paramValue" /> is <c>null</c> or empty.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmpty(string paramName, string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue))
            {
                var error = string.Format("Argument '{0}' cannot be null or empty", paramName.ToString());
                throw new ArgumentNullException(error, paramName);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not empty.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentException">If <paramref name="paramValue" /> is <c>null</c> or empty.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotEmpty(string paramName, Guid paramValue)
        {
            if (paramValue == Guid.Empty)
            {
                var error = string.Format("Argument '{0}' cannot be Guid.Empty", paramName.ToString());
                throw new ArgumentException(error, paramName);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or empty.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="paramValue" /> is <c>null</c> or empty.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmpty(string paramName, Guid? paramValue)
        {
            if (!paramValue.HasValue || paramValue.Value == Guid.Empty)
            {
                var error = string.Format("Argument '{0}' cannot be null or Guid.Empty", paramName.ToString());
                throw new ArgumentException(error, paramName);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or a whitespace.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="paramValue" /> is <c>null</c> or a whitespace.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrWhitespace(string paramName, string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue) || (string.CompareOrdinal(paramValue.Trim(), string.Empty) == 0))
            {
                var error = string.Format("Argument '{0}' cannot be null or whitespace", paramName.ToString());
                throw new ArgumentException(error, paramName);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="paramValue" /> is <c>null</c> or an empty array.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmptyArray(string paramName, Array paramValue)
        {
            if ((paramValue is null) || (paramValue.Length == 0))
            {
                var error = string.Format("Argument '{0}' cannot be null or an empty array", paramName.ToString());
                throw new ArgumentException(error, paramName);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not <c>null</c> or an empty array (.Length == 0).
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException">If <paramref name="paramValue" /> is <c>null</c> or an empty array.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotNullOrEmptyList<T>(string paramName, IList<T> paramValue)
        {
            if ((paramValue is null) || (paramValue.Count == 0))
            {
                var error = string.Format("Argument '{0}' cannot be null or an empty list", paramName.ToString());
                throw new ArgumentException(error, paramName);
            }
        }

        public static IList<T> HasNoNulls<T>(string parameterName, IList<T> value)
            where T : class
        {
            IsNotNull(parameterName, value);

            if (value.Any(e => e == null))
            {
                IsNotNullOrEmpty(nameof(parameterName), parameterName);

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="validation">The validation function to call for validation.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validation" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="paramValue" /> is out of range.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(string paramName, T paramValue, T minimumValue, T maximumValue, Func<T, T, T, bool> validation)
        {
            IsNotNull(nameof(validation), validation);

            if (!validation(paramValue, minimumValue, maximumValue))
            {
                var error = string.Format("Argument '{0}' should be between {1} and {2}", paramName.ToString(), minimumValue, maximumValue);
                throw new ArgumentOutOfRangeException(paramName, error);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is not out of range.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="paramValue" /> is out of range.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsNotOutOfRange<T>(string paramName, T paramValue, T minimumValue, T maximumValue)
            where T : IComparable
        {
            IsNotOutOfRange(paramName, paramValue, minimumValue, maximumValue,
                (innerParamValue, innerMinimumValue, innerMaximumValue) => innerParamValue.CompareTo(innerMinimumValue) >= 0 && innerParamValue.CompareTo(innerMaximumValue) <= 0);
        }

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <param name="validation">The validation function to call for validation.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validation" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="paramValue" /> is out of range.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMinimal<T>(string paramName, T paramValue, T minimumValue, Func<T, T, bool> validation)
        {
            IsNotNull(nameof(validation), validation);

            if (!validation(paramValue, minimumValue))
            {
                var error = string.Format("Argument '{0}' should be minimal {1}", paramName.ToString(), minimumValue);
                throw new ArgumentOutOfRangeException(paramName, error);
            }
        }

        /// <summary>
        /// Determines whether the specified argument has a minimum value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="paramValue" /> is out of range.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMinimal<T>(string paramName, T paramValue, T minimumValue)
            where T : IComparable
        {
            IsMinimal(paramName, paramValue, minimumValue,
                (innerParamValue, innerMinimumValue) => innerParamValue.CompareTo(innerMinimumValue) >= 0);
        }

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <param name="validation">The validation function to call for validation.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validation" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="paramValue" /> is out of range.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMaximum<T>(string paramName, T paramValue, T maximumValue, Func<T, T, bool> validation)
        {
            if (!validation(paramValue, maximumValue))
            {
                var error = string.Format("Argument '{0}' should be at maximum {1}", paramName.ToString(), maximumValue);
                throw new ArgumentOutOfRangeException(paramName, error);
            }
        }

        /// <summary>
        /// Determines whether the specified argument has a maximum value.
        /// </summary>
        /// <typeparam name="T">Type of the argument.</typeparam>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the parameter.</param>
        /// <param name="maximumValue">The maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="paramValue" /> is out of range.</exception>
        [DebuggerNonUserCode, DebuggerStepThrough]
        public static void IsMaximum<T>(string paramName, T paramValue, T maximumValue)
            where T : IComparable
        {
            IsMaximum(paramName, paramValue, maximumValue,
                (innerParamValue, innerMaximumValue) => innerParamValue.CompareTo(innerMaximumValue) <= 0);
        }

        public static T Condition<T>(string parameterName, T value, Predicate<T> condition)
        {
            IsNotNull(nameof(condition), condition);
            IsNotNull(nameof(value), value);

            if (!condition(value))
            {
                IsNotNullOrEmpty(nameof(parameterName), parameterName);

                throw new ArgumentOutOfRangeException(parameterName);
            }

            return value;
        }
    }
}
