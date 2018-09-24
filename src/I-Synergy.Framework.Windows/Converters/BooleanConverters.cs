using ISynergy.Extensions;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class BoolToStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return Constants.InactiveStateKey.GetLocalized();
            }

            if ((bool)value)
            {
                return Constants.ActivateStateKey.GetLocalized();
            }

            return Constants.InactiveStateKey.GetLocalized();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Constants.ActivateStateKey.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class BoolToCommandBarDisplayModeConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToStyleConverter : BooleanConverter<Style>
    {
    }

    public class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter()
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }

    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is bool)
                return !(bool)value;

            throw new InvalidOperationException("The target must be a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            if (value is bool)
                return !(bool)value;

            throw new InvalidOperationException("The target must be a boolean");
        }
    }

    public class BooleanToStringConverter : BooleanConverter<string>
    {
    }

    public class BooleanToIntegerConverter : BooleanConverter<int>
    {
    }

    public class BooleanToDoubleConverter : BooleanConverter<double>
    {
    }

    public class BooleanToBooleanConverter : BooleanConverter<bool>
    {
    }

    public abstract class BooleanConverter<T> : DependencyObject, IValueConverter
    {
        public BooleanConverter()
        {
        }

        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T False
        {
            get { return (T)GetValue(FalseProperty); }
            set { SetValue(FalseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for False.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(nameof(False), typeof(T), typeof(BooleanConverter<T>), new PropertyMetadata(default(T)));

        public T True
        {
            get { return (T)GetValue(TrueProperty); }
            set { SetValue(TrueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for True.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(nameof(True), typeof(T), typeof(BooleanConverter<T>), new PropertyMetadata(default(T)));

        public virtual object Convert(object value, Type targetType, object parameter, string culture)
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

        public virtual object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return value != null ? value.Equals(True) : false;
        }
    }
}
