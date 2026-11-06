using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Globalization.Abstractions.Services;
using System.Text;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Shared change-tracking display formatting used by the platform value converters.
/// </summary>
public static class ChangeTrackingConverter
{
    /// <summary>
    /// Formats the created/changed audit fields of <paramref name="value"/> using localized labels.
    /// </summary>
    /// <param name="value">The value to format; only <see cref="IModel"/> instances produce output.</param>
    /// <param name="languageService">The language service used for the labels.</param>
    /// <returns>The formatted audit text, or an empty string.</returns>
    public static string ToDisplayText(object? value, ILanguageService languageService)
    {
        if (value is not IModel model)
            return string.Empty;

        var result = new StringBuilder();

        var userCreated = languageService.GetString("Unknown");
        var userChanged = languageService.GetString("Unknown");

        if (!string.IsNullOrEmpty(model.CreatedBy))
            userCreated = model.CreatedBy;

        result.AppendLine($"{languageService.GetString("InputFirst")} " +
            $"{model.CreatedDate.ToLocalTime():f} {languageService.GetString("By")} {userCreated}");

        if (model.ChangedDate.HasValue)
        {
            if (!string.IsNullOrEmpty(model.ChangedBy))
                userChanged = model.ChangedBy;

            result.AppendLine($"{languageService.GetString("InputLast")} " +
                $"{model.ChangedDate.Value.ToLocalTime():f} {languageService.GetString("By")} {userChanged}");
        }

        return result.ToString();
    }
}
