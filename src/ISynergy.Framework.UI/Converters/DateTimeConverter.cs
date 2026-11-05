namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Shared date/time conversion helpers used by the platform value converters.
/// </summary>
public static class DateTimeConverter
{
    /// <summary>
    /// Converts from DateTime to TimeSpan.
    /// </summary>
    /// <param name="dt">The source DateTime value.</param>
    /// <returns>The time portion of <paramref name="dt"/>, or zero for the DateTime bounds.</returns>
    public static TimeSpan? DateTimeToTimeSpan(DateTime dt)
    {
        if (dt == DateTime.MinValue || dt == DateTime.MaxValue)
        {
            return new TimeSpan(0);
        }

        return dt - dt.Date;
    }

    /// <summary>
    /// Converts from TimeSpan to DateTime.
    /// </summary>
    /// <param name="dt">The date to keep.</param>
    /// <param name="ts">The time of day to apply.</param>
    /// <returns>A DateTime with the date from <paramref name="dt"/> and the time from <paramref name="ts"/>.</returns>
    public static DateTime? TimeSpanToDateTime(DateTime dt, TimeSpan ts)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds, dt.Kind);
    }
}
