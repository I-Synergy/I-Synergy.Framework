namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Interface IConverterService
/// </summary>
public interface IConverterService
{
    /// <summary>
    /// Converts the media color2 integer.
    /// </summary>
    /// <param name="mediacolor">The mediacolor.</param>
    /// <returns>System.Int32.</returns>
    int ConvertMediaColor2Integer(object mediacolor);
    /// <summary>
    /// Converts the decimal to currency.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    string ConvertDecimalToCurrency(decimal value);
}
