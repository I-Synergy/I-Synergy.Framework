namespace ISynergy.Framework.UI.Converters.Shared;

/// <summary>
/// Base implementation for IntegerToDoubleConverter shared across platforms.
/// Contains the core conversion logic that doesn't depend on platform-specific IValueConverter interfaces.
/// </summary>
public static class IntegerToDoubleConverterBase
{
    /// <summary>
    /// Converts an integer value to double.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted double value, or 0d if conversion fails.</returns>
    public static double Convert(object? value)
    {
        if (value is int intValue)
        {
            return System.Convert.ToDouble(intValue);
        }

        return 0d;
    }

    /// <summary>
    /// Converts a double value back to integer.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <returns>The converted integer value, or 0 if conversion fails.</returns>
    public static int ConvertBack(object? value)
    {
        if (value is double doubleValue)
        {
            // Handle NaN case (WinUI has this check)
            if (double.IsNaN(doubleValue))
                return 0;
            return System.Convert.ToInt32(doubleValue);
        }

        return 0;
    }
}

