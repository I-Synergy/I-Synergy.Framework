using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System.Text;
using Windows.UI.Xaml.Data;

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
            // Cache service lookup for performance - converters are XAML-bound and cannot use constructor injection
            var languageService = ServiceLocator.Default.GetRequiredService<ILanguageService>();

            var userCreated = languageService.GetString("Unknown");
            var userChanged = languageService.GetString("Unknown");

            if (!string.IsNullOrEmpty(model.CreatedBy)) userCreated = model.CreatedBy;

            result.AppendLine($"{languageService.GetString("InputFirst")} " +
                $"{model.CreatedDate.ToLocalTime():f} {languageService.GetString("By")} {userCreated}");

            if (model.ChangedDate.HasValue)
            {
                if (!string.IsNullOrEmpty(model.ChangedBy)) userChanged = model.ChangedBy;

                result.AppendLine($"{languageService.GetString("InputLast")} " +
                    $"{model.ChangedDate.Value.ToLocalTime():f} {languageService.GetString("By")} {userChanged}");
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
