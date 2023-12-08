using ISynergy.Framework.Mvvm.Enumerations;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Converters;

public class SelectionModeConverters : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SelectionModes selectionMode && selectionMode == SelectionModes.Multiple)
            return SelectionMode.Multiple;

        return SelectionMode.Single;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
