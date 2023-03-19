using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Converters
{
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
        /// <param name="culture">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="InvalidOperationException">The target must be a boolean</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
        /// <param name="culture">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="InvalidOperationException">The target must be a boolean</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool x)
                return !x;

            throw new InvalidOperationException("The target must be a boolean");
        }
    }

    /// <summary>
    /// Boolean to Visibility converter.
    /// </summary>
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
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo language)
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
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return value != null && value.Equals(True);
        }
    }
}
