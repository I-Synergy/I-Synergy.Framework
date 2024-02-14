using ISynergy.Framework.Core.Enumerations;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Converters;

public class RadioButtonToLanguageConverter : IValueConverter
{
    private Languages _value;

    public object Convert(object value, Type targetType, object parameter, string culture)
    {
        if (value is Languages languages)
            _value = languages;

        if (Enum.TryParse(typeof(Languages), parameter.ToString(), true, out var language) && value.Equals(language))
        {
            return true;
        }

        return false;
    }

    public object ConvertBack(object isChecked, Type targetType, object parameter, string culture)
    {
        if ((bool)isChecked && Enum.TryParse(typeof(Languages), parameter.ToString(), true, out var language))
        {
            return language;
        }

        return _value;
    }
}
