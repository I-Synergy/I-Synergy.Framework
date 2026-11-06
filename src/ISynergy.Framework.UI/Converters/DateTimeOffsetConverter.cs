namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Shared date/time-offset conversion helpers used by the platform value converters.
/// </summary>
public static class DateTimeOffsetConverter
{
    /// <summary>
    /// Converts from DateTimeOffset to TimeSpan.
    /// </summary>
    /// <param name="dt">The source DateTimeOffset value.</param>
    /// <returns>The time portion of <paramref name="dt"/>, or zero for the DateTimeOffset bounds.</returns>
    public static TimeSpan? DateTimeOffsetToTimeSpan(DateTimeOffset dt)
    {
        if (dt == DateTimeOffset.MinValue || dt == DateTimeOffset.MaxValue)
        {
            return new TimeSpan(0);
        }

        return dt.TimeOfDay;
    }

    /// <summary>
    /// Converts from TimeSpan to DateTimeOffset.
    /// </summary>
    /// <param name="dt">The date and offset to keep.</param>
    /// <param name="ts">The time of day to apply.</param>
    /// <returns>A DateTimeOffset with the date from <paramref name="dt"/> and the time from <paramref name="ts"/>.</returns>
    public static DateTimeOffset? TimeSpanToDateTimeOffset(DateTimeOffset dt, TimeSpan ts)
    {
        return new DateTimeOffset(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, dt.Offset);
    }
}
