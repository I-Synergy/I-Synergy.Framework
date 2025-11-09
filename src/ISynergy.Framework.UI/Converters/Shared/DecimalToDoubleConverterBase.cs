namespace ISynergy.Framework.UI.Converters.Shared;

/// <summary>
/// Base implementation for DecimalToDoubleConverter shared across platforms.
/// Contains the core conversion logic that doesn't depend on platform-specific IValueConverter interfaces.
/// </summary>
public static class DecimalToDoubleConverterBase
{
    /// <summary>
    /// Converts a decimal value to double.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted double value, or 0d if conversion fails.</returns>
    public static double Convert(object? value)
    {
        if (value is decimal decimalValue)
        {
            return System.Convert.ToDouble(decimalValue);
        }

        return 0d;
    }

    /// <summary>
    /// Converts a double value back to decimal.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <returns>The converted decimal value, or 0m if conversion fails.</returns>
    public static decimal ConvertBack(object? value)
    {
        if (value is double doubleValue)
        {
            // Handle NaN case (WinUI has this check)
            if (double.IsNaN(doubleValue))
                return 0m;
            return System.Convert.ToDecimal(doubleValue);
        }

        return 0m;
    }
}

