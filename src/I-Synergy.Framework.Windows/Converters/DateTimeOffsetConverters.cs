using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class DateOffsetToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value, targetType, parameter, new CultureInfo(language));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = DateTime.Now;

            if (value != null && value.GetType() == typeof(DateTimeOffset))
            {
                result = ((DateTimeOffset)value).ToLocalTime().DateTime;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertBack(value, targetType, parameter, new CultureInfo(language));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(DateTime))
                {
                    return new DateTimeOffset(DateTime.SpecifyKind((DateTime)value, DateTimeKind.Local));
                }

                if (value.GetType() == typeof(string))
                {
                    return new DateTimeOffset(DateTime.SpecifyKind(DateTime.Parse(value.ToString()), DateTimeKind.Local));
                }
            }

            return new DateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local));
        }

        public class DateOffsetToDateConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                return Convert(value, targetType, parameter, new CultureInfo(language));
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null && value.GetType() == typeof(DateTimeOffset))
                {
                    DateTimeOffset date = (DateTimeOffset)value;
                    return date.ToLocalTime().Date;
                }

                return DateTime.Now.Date;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                return ConvertBack(value, targetType, parameter, new CultureInfo(language));
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null)
                {
                    if (value.GetType() == typeof(DateTime))
                    {
                        return new DateTimeOffset((DateTime)value, DateTimeOffset.Now.Offset);
                    }

                    if (value.GetType() == typeof(string))
                    {
                        return new DateTimeOffset(DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture));
                    }
                }

                return DateTimeOffset.Now;
            }
        }
    }
}
