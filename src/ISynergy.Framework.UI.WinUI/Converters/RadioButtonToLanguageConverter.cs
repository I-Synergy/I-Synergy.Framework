using ISynergy.Framework.Core.Enumerations;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Converters;

public class RadioButtonToLanguageConverter : IValueConverter
{
    private Languages _value;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Languages lang)
            _value = lang;

        if (Enum.TryParse(typeof(Languages), parameter.ToString(), true, out var parsedLanguage) && value.Equals(parsedLanguage))
        {
            return true;
        }

        return false;
    }

    public object ConvertBack(object isChecked, Type targetType, object parameter, string language)
    {
        if ((bool)isChecked && Enum.TryParse(typeof(Languages), parameter.ToString(), true, out var parsedLanguage))
        {
            return parsedLanguage;
        }

        return _value;
    }
}
