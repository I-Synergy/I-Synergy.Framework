using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using Microsoft.UI.Xaml.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class DateTimeOffsetConverter.
/// </summary>
public static class DateTimeOffsetConverter
{
    /// <summary>
    /// Converts from DateTime to TimaSpan.
    /// </summary>
    /// <param name="dt">The source DateTime value.</param>
    /// <returns>Returns the time portion of DateTime in the form of TimeSpan if succeeded, null otherwise.</returns>
    public static TimeSpan? DateTimeOffsetToTimeSpan(DateTimeOffset dt)
    {
        if (dt == DateTimeOffset.MinValue || dt == DateTimeOffset.MaxValue)
        {
            return new TimeSpan(0);
        }

        return dt - dt.Date;
    }

    /// <summary>
    /// Converts from Timespan to DateTime.
    /// </summary>
    /// <param name="dt">The dt.</param>
    /// <param name="ts">The source TimeSpan value.</param>
    /// <returns>Returns a DateTime filled with date equals to mindate and time equals to time in timespan if succeeded, null otherwise.</returns>
    public static DateTimeOffset? TimeSpanToDateTimeOffset(DateTimeOffset dt, TimeSpan ts)
    {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, dt.Offset);
    }
}

/// <summary>
/// Class DateTimeOffsetToTimeSpanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DateTimeOffsetToTimeSpanConverter : IValueConverter
{
    /// <summary>
    /// The original
    /// </summary>
    private DateTimeOffset original;

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
        if (value is DateTimeOffset dt)
        {
            original = dt;
            var ts = DateTimeOffsetConverter.DateTimeOffsetToTimeSpan(dt);
            return ts.GetValueOrDefault(TimeSpan.MinValue);
        }

        return TimeSpan.MinValue;
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
        if (original is DateTimeOffset odt && value is TimeSpan ts)
        {
            var dt = DateTimeOffsetConverter.TimeSpanToDateTimeOffset(odt, ts);
            return dt.GetValueOrDefault(DateTimeOffset.MinValue);
        }

        return DateTimeOffset.MinValue;
    }
}

/// <summary>
/// Class DateOffsetCollectionToDateTimeCollectionConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DateOffsetCollectionToDateTimeCollectionConverter : IValueConverter
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
        var result = new ObservableCollection<DateTime>();

        if (value is not null && value.GetType() == typeof(ObservableCollection<DateTimeOffset>))
        {
            var collection = value as ObservableCollection<DateTimeOffset>;

            foreach (var item in collection.EnsureNotNull())
            {
                result.Add(item.ToLocalTime().DateTime);
            }
        }

        return result;
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
        var result = new ObservableCollection<DateTimeOffset>();

        if (value is not null)
        {
            if (value.GetType() == typeof(ObservableCollection<DateTime>))
            {
                var collection = value as ObservableCollection<DateTime>;

                foreach (var item in collection.EnsureNotNull())
                {
                    result.Add(new DateTimeOffset(DateTime.SpecifyKind(item, DateTimeKind.Local)));
                }
            }

            if (value.GetType() == typeof(ObservableCollection<string>))
            {
                var collection = value as ObservableCollection<string>;

                foreach (var item in collection.EnsureNotNull())
                {
                    result.Add(new DateTimeOffset(DateTime.SpecifyKind(DateTime.Parse(item), DateTimeKind.Local)));
                }
            }
        }

        return result;
    }
}

/// <summary>
/// Class DateTimeOffsetToLocalDateTimeOffsetConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DateTimeOffsetToLocalDateTimeOffsetConverter : IValueConverter
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
        if (value is DateTimeOffset datetime)
        {
            return datetime.ToLocalTime();
        }

        return DateTimeOffset.Now.ToLocalTime();
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
        if (value is DateTimeOffset datetime)
        {
            return datetime.ToUniversalTime();
        }

        return DateTimeOffset.Now.ToUniversalTime();
    }
}

/// <summary>
/// Class DateTimeOffsetToLocalDateStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DateTimeOffsetToLocalDateStringConverter : IValueConverter
{
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=d} 2009-06-15T13:45:30 -> 6/15/2009 (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=D} 2009-06-15T13:45:30 -> Monday, June 15, 2009 (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=f} 2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45 PM (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=F} 2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45:30 PM (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=g} 2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=G} 2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=t} 2009-06-15T13:45:30 -> 1:45 PM (en-US)
    // Converter={StaticResource DateTimeOffsetToLocalDateStringConverter}, ConverterParameter=T} 2009-06-15T13:45:30 -> 1:45:30 PM (en-US)

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
        if (value is DateTimeOffset datetime)
        {
            var culture = CultureInfo.CurrentCulture;

            if (!string.IsNullOrEmpty(language))
                culture = new CultureInfo(language);

            var offset = TimeZoneInfo.Local.BaseUtcOffset;

            var scopedContextService = ServiceLocator.Default.GetRequiredService<IScopedContextService>();

            offset = scopedContextService.GetRequiredService<IContext>().TimeZone!.BaseUtcOffset;

            if (parameter is not null)
                return datetime.ToLocalDateString(parameter.ToString()!, offset, culture);

            return datetime.ToLocalDateString("f", offset, culture);
        }

        return DateTimeOffset.Now.ToString("f");
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
/// Class IsoDateTimeOffsetConverter.
/// Implements the <see cref="IsoDateTimeOffsetJsonConverter" />
/// </summary>
/// <seealso cref="IsoDateTimeOffsetJsonConverter" />
public class IsoDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTimeOffset.ParseExact(reader.GetString()!, "o", null);

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        if (value.Offset == TimeSpan.Zero)
        {
            // If there is no offset, serialize as a DateTime
            writer.WriteStringValue(value.UtcDateTime.ToString("o"));
        }
        else
        {
            writer.WriteStringValue(value.ToString("o"));
        }
    }
}
