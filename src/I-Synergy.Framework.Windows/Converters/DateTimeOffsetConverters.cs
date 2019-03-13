using System;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class DateTimeOffsetToLocalDateTimeOffsetConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset datetime)
            {
                return datetime.ToLocalTime();
            }

            return DateTimeOffset.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeOffsetToLocalDateStringConverter : IValueConverter
    {
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:d\} 2009-06-15T13:45:30 -> 6/15/2009 (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:D\} 2009-06-15T13:45:30 -> Monday, June 15, 2009 (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:f\} 2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:F\} 2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45:30 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:g\} 2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=\{0:G\} 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=T} 2009-06-15T13:45:30 -> 1:45 PM (en-US)
        // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=T} 2009-06-15T13:45:30 -> 1:45:30 PM (en-US)

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset datetime)
            {
                if(parameter != null)
                {
                    return datetime.ToLocalTime().ToString(parameter.ToString());
                }

                return datetime.ToLocalTime().ToString("f");
            }

            return DateTimeOffset.Now.ToString("f");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
