using ISynergy.Framework.Core.Enumerations;
using System.ComponentModel;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters
{
    public class RadioButtonToLanguageConverter : IMarkupExtension<RadioButtonToLanguageConverter>, IValueConverter
    {
        private Languages _value;

        public Languages Language { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            if (value is Languages languages)
                _value = languages;

            return value.Equals(Language);
        }

        public object ConvertBack(object isChecked, Type targetType, object parameter, CultureInfo culture)
            => (bool)isChecked  // Is this the checked RadioButton? If so...
            ? Language          // Send 'Language' back to update the associated binding. Otherwise...
            : _value;           // Return de converterd value, telling the binding 'ignore this change'

        public RadioButtonToLanguageConverter ProvideValue(IServiceProvider serviceProvider) => this;

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            (this as IMarkupExtension<RadioButtonToLanguageConverter>).ProvideValue(serviceProvider);
    }
}
