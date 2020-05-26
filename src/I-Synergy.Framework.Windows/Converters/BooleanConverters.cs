using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Windows.Extensions;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    /// <summary>
    /// Value converter that applies NOT operator to a <see cref="bool" /> value.
    /// </summary>
    public class BoolNegationConverter : IValueConverter
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
    /// Implements the <see cref="Windows.UI.Xaml.Data.IValueConverter" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class BoolToStateConverter : IValueConverter
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
    /// Implements the <see cref="Windows.UI.Xaml.Data.IValueConverter" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class BoolToCommandBarDisplayModeConverter : IValueConverter
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
    /// Implements the <see cref="BooleanConverter{Windows.UI.Xaml.Style}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{Windows.UI.Xaml.Style}" />
    public class BooleanToStyleConverter : BooleanConverter<Style>
    {
    }

    /// <summary>
    /// Class BooleanToVisibilityConverter.
    /// Implements the <see cref="BooleanConverter{Windows.UI.Xaml.Visibility}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{Windows.UI.Xaml.Visibility}" />
    public class BooleanToVisibilityConverter : BooleanConverter<Visibility>
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
    /// Implements the <see cref="Windows.UI.Xaml.Data.IValueConverter" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class NullToBooleanConverter : IValueConverter
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
    /// Implements the <see cref="Windows.UI.Xaml.Data.IValueConverter" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class InverseBooleanConverter : IValueConverter
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
    /// Implements the <see cref="BooleanConverter{string}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{string}" />
    public class BooleanToStringConverter : BooleanConverter<string>
    {
    }

    /// <summary>
    /// Class BooleanToIntegerConverter.
    /// Implements the <see cref="BooleanConverter{int}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{int}" />
    public class BooleanToIntegerConverter : BooleanConverter<int>
    {
    }

    /// <summary>
    /// Class BooleanToDoubleConverter.
    /// Implements the <see cref="BooleanConverter{double}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{double}" />
    public class BooleanToDoubleConverter : BooleanConverter<double>
    {
    }

    /// <summary>
    /// Class BooleanToBooleanConverter.
    /// Implements the <see cref="BooleanConverter{bool}" />
    /// </summary>
    /// <seealso cref="BooleanConverter{bool}" />
    public class BooleanToBooleanConverter : BooleanConverter<bool>
    {
    }

    /// <summary>
    /// Class BooleanConverter.
    /// Implements the <see cref="Windows.UI.Xaml.DependencyObject" />
    /// Implements the <see cref="Windows.UI.Xaml.Data.IValueConverter" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Windows.UI.Xaml.DependencyObject" />
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public abstract class BooleanConverter<T> : DependencyObject, IValueConverter
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
        public T False
        {
            get { return (T)GetValue(FalseProperty); }
            set { SetValue(FalseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for False.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The false property
        /// </summary>
        public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(nameof(False), typeof(T), typeof(BooleanConverter<T>), new PropertyMetadata(default(T)));

        /// <summary>
        /// Gets or sets the true.
        /// </summary>
        /// <value>The true.</value>
        public T True
        {
            get { return (T)GetValue(TrueProperty); }
            set { SetValue(TrueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for True.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The true property
        /// </summary>
        public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(nameof(True), typeof(T), typeof(BooleanConverter<T>), new PropertyMetadata(default(T)));

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
