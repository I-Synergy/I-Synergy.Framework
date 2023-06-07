using System.Globalization;

namespace ISynergy.Framework.UI.Converters
{
    public class RadioButtonValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value.Equals(parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = "en";

            if (value is bool isChecked && isChecked && parameter is string selectedLanguage)
            {
                result = selectedLanguage;
            }

            return result;
        }
    }
}
