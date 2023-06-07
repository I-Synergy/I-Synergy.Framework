using System.Globalization;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Converters
{
    public class RadioButtonValueConverter : IValueConverter
    {
        public object Value { get; }

        public RadioButtonValueConverter(object value)
        {
            Value = value;        
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value.Equals(Value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value            // Is RadioButton? is checked...
                ? Value               // Send 'Value' back to update the associated binding...
                : Binding.DoNothing;  // Otherwise return Binding.DoNothing, telling the binding 'ignore this change'
    }
}
