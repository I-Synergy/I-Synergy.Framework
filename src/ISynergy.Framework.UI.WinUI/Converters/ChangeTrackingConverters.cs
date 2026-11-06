using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Globalization.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Data;
using System.Text;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class ChangeTrackingConverters.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public partial class ChangeTrackingConverters : IValueConverter
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
        // Cache service lookup for performance - converters are XAML-bound and cannot use constructor injection
        var languageService = Application.Services.GetRequiredService<ILanguageService>();
        return ChangeTrackingConverter.ToDisplayText(value, languageService);
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
