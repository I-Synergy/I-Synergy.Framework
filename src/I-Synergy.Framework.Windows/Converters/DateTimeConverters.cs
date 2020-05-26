using System;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public static class DateTimeConverter
    {
        /// <summary>
        /// Converts from DateTime to TimaSpan.
        /// </summary>
        /// <param name="dt">The source DateTime value.</param>
        /// <returns>Returns the time portion of DateTime in the form of TimeSpan if succeeded, null otherwise.</returns>
        public static TimeSpan? DateTimeToTimeSpan(DateTime dt)
        {
            if (dt == DateTime.MinValue || dt == DateTime.MaxValue)
            {
                return new TimeSpan(0);
            }
            else
            {
                return dt - dt.Date;
            }
        }

        /// <summary>
        /// Converts from Timespan to DateTime.
        /// </summary>
        /// <param name="ts">The source TimeSpan value.</param>
        /// <returns>Returns a DateTime filled with date equals to mindate and time equals to time in timespan if succeeded, null otherwise.</returns>
        public static DateTime? TimeSpanToDateTime(DateTime dt, TimeSpan ts)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, dt.Kind);
        }
    }

    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        private DateTime original;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dt)
            {
                original = dt;
                var ts = DateTimeConverter.DateTimeToTimeSpan(dt);
                return ts.GetValueOrDefault(TimeSpan.MinValue);
            }

            return TimeSpan.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (original is DateTime odt && value is TimeSpan ts)
            {
                var dt = DateTimeConverter.TimeSpanToDateTime(odt, ts);
                return dt.GetValueOrDefault(DateTime.MinValue);
            }

            return DateTime.MinValue;
        }
    }

    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTime dt)
            {
                return new DateTimeOffset(dt);
            }

            return DateTimeOffset.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is DateTimeOffset dt)
            {
                if (dt.Offset.Equals(TimeSpan.Zero))
                    return dt.UtcDateTime;
                else if (dt.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dt.DateTime)))
                    return DateTime.SpecifyKind(dt.DateTime, DateTimeKind.Local);
                else
                    return dt.DateTime;
            }

            return DateTime.Now;
        }
    }

    public class DateTimeToLocalDateStringConverter : IValueConverter
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
            if (value is DateTime datetime)
            {
                if (parameter != null)
                {
                    return datetime.ToLocalTime().ToString(parameter.ToString());
                }

                return datetime.ToLocalTime().ToString("f");
            }

            return DateTime.Now.ToString("f");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
