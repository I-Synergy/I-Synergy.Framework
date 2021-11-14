#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endif


namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Value converter that applies NOT operator to a <see cref="bool" /> value.
    /// </summary>
    public sealed class BoolNegationConverter : IValueConverter
    {
        /// <summary>
        /// Convert a boolean value to its negation.
        /// </summary>
        /// <param name="value">The <see cref="bool" /> value to negate.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool x && x);
        }

        /// <summary>
        /// Convert back a boolean value to its negation.
        /// </summary>
        /// <param name="value">The <see cref="bool" /> value to negate.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool x && x);
        }
    }

    /// <summary>
    /// Class BoolToStateConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public sealed class BoolToStateConverter : IValueConverter
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
            if (!(value is bool))
            {
                return GenericConstants.InactiveStateKey.GetLocalized();
            }

            if ((bool)value)
            {
                return GenericConstants.ActivateStateKey.GetLocalized();
            }

            return GenericConstants.InactiveStateKey.GetLocalized();
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return GenericConstants.ActivateStateKey.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Class BoolToCommandBarDisplayModeConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public sealed class BoolToCommandBarDisplayModeConverter : IValueConverter
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
            if (!(value is bool))
            {
                return AppBarClosedDisplayMode.Hidden;
            }

            if ((bool)value)
            {
                return AppBarClosedDisplayMode.Compact;
            }

            return AppBarClosedDisplayMode.Hidden;
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
    }

    /// <summary>
    /// Class BooleanToStyleConverter.
    /// Implements the <see cref="BooleanConverter{Style}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{Style}" />
    public sealed class BooleanToStyleConverter : BooleanConverter<Style>
    {
    }

    /// <summary>
    /// Class BooleanToVisibilityConverter.
    /// Implements the <see cref="BooleanConverter{Visibility}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{Visibility}" />
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class.
        /// </summary>
        public BooleanToVisibilityConverter()
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }

    /// <summary>
    /// Class NullToBooleanConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public sealed class NullToBooleanConverter : IValueConverter
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
            if (value != null)
            {
                return true;
            }
            else
            {
                return false;
            }
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
    }

    /// <summary>
    /// Class InverseBooleanConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public sealed class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="InvalidOperationException">The target must be a boolean</exception>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool x)
                return !x;

            throw new InvalidOperationException("The target must be a boolean");
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="InvalidOperationException">The target must be a boolean</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool x)
                return !x;

            throw new InvalidOperationException("The target must be a boolean");
        }
    }

    /// <summary>
    /// Class BooleanToStringConverter.
    /// Implements the <see cref="BooleanToStringConverter" />
    /// </summary>
    /// <seealso cref="BooleanToStringConverter" />
    public sealed class BooleanToStringConverter : BooleanConverter<string>
    {
    }

    /// <summary>
    /// Class BooleanToIntegerConverter.
    /// Implements the <see cref="BooleanToIntegerConverter" />
    /// </summary>
    /// <seealso cref="BooleanToIntegerConverter" />
    public sealed class BooleanToIntegerConverter : BooleanConverter<int>
    {
    }

    /// <summary>
    /// Class BooleanToDoubleConverter.
    /// Implements the <see cref="BooleanToDoubleConverter" />
    /// </summary>
    /// <seealso cref="BooleanToDoubleConverter" />
    public sealed class BooleanToDoubleConverter : BooleanConverter<double>
    {
    }

    /// <summary>
    /// Class BooleanToBooleanConverter.
    /// Implements the <see cref="BooleanToBooleanConverter" />
    /// </summary>
    /// <seealso cref="BooleanToBooleanConverter" />
    public sealed class BooleanToBooleanConverter : BooleanConverter<bool>
    {
    }

    /// <summary>
    /// Class BooleanConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IValueConverter" />
    public abstract class BooleanConverter<T> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter{T}"/> class.
        /// </summary>
        protected BooleanConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter{T}"/> class.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        protected BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        /// <summary>
        /// Gets or sets the false.
        /// </summary>
        /// <value>The false.</value>
        public T False { get; set; }

        /// <summary>
        /// Gets or sets the true.
        /// </summary>
        /// <value>The true.</value>
        public T True { get; set; }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return False;
            }
            else
            {
                return System.Convert.ToBoolean(value) ? True : False;
            }
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null && value.Equals(True);
        }
    }
}
