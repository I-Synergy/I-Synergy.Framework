using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class DecimalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((decimal)value == 0 || value is null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal.TryParse(parameter.ToString(), out var limit);
            decimal.TryParse(value.ToString(), out var test);

            if (test == limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalLesserThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal.TryParse(parameter.ToString(), out var limit);
            decimal.TryParse(value.ToString(), out var test);

            if (test < limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalGreaterThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal.TryParse(parameter.ToString(), out var limit);
            decimal.TryParse(value.ToString(), out var test);

            if (test > limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalEqualsOrLesserThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal.TryParse(parameter.ToString(), out var limit);
            decimal.TryParse(value.ToString(), out var test);

            if (test <= limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalEqualsOrGreaterThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal.TryParse(parameter.ToString(), out var limit);
            decimal.TryParse(value.ToString(), out var test);

            if (test >= limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal)
            {
                if (parameter != null)
                {
                    return string.Format((string)parameter, value);
                }

                return ((decimal)value).ToString();
            }

            if (parameter != null)
            {
                return string.Format((string)parameter, (decimal)0);
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (decimal.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }

    public class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is decimal decimalValue)
            {
                return System.Convert.ToDouble(decimalValue);
            }

            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is double doubleValue)
            {
                return System.Convert.ToDecimal(doubleValue);
            }

            return 0m;
        }
    }
}
