using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Services;
using Microsoft.UI.Xaml.Data;
using System.Text;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class ChangeTrackingConverters.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ChangeTrackingConverters : IValueConverter
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
        if (value is IModel model)
        {
            var result = new StringBuilder();

            var userCreated = LanguageService.Default.GetString("Unknown");
            var userChanged = LanguageService.Default.GetString("Unknown");

            if (!string.IsNullOrEmpty(model.CreatedBy)) userCreated = model.CreatedBy;

            result.AppendLine($"{LanguageService.Default.GetString("InputFirst")} " +
                $"{model.CreatedDate.ToLocalTime():f} {LanguageService.Default.GetString("By")} {userCreated}");

            if (model.ChangedDate.HasValue)
            {
                if (!string.IsNullOrEmpty(model.ChangedBy)) userChanged = model.ChangedBy;

                result.AppendLine($"{LanguageService.Default.GetString("InputLast")} " +
                    $"{model.ChangedDate.Value.ToLocalTime():f} {LanguageService.Default.GetString("By")} {userChanged}");
            }

            return result.ToString();
        }

        return string.Empty;
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
