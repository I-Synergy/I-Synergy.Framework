using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class DecimalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            decimal.TryParse(parameter.ToString(), out decimal limit);
            decimal.TryParse(value.ToString(), out decimal test);

            if (test == limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalLesserThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            decimal.TryParse(parameter.ToString(), out decimal limit);
            decimal.TryParse(value.ToString(), out decimal test);

            if (test < limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalGreaterThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            decimal.TryParse(parameter.ToString(), out decimal limit);
            decimal.TryParse(value.ToString(), out decimal test);

            if (test > limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalEqualsOrLesserThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            decimal.TryParse(parameter.ToString(), out decimal limit);
            decimal.TryParse(value.ToString(), out decimal test);

            if (test <= limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalEqualsOrGreaterThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            decimal.TryParse(parameter.ToString(), out decimal limit);
            decimal.TryParse(value.ToString(), out decimal test);

            if (test >= limit)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            if (decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}
