using System.Globalization;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    public class IntegerTextBox : TextBox
    {
        public IntegerTextBox() : base()
        {
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                TextAlignment = Windows.UI.Xaml.TextAlignment.Left;
            }
            else
            {
                TextAlignment = Windows.UI.Xaml.TextAlignment.Right;
            }

            TextBoxAttached.SetAutoSelectable(this, true);

            Loaded += IntegerTextBox_Loaded;
            Unloaded += IntegerTextBox_Unloaded;
        }

        private void IntegerTextBox_Loaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            KeyDown += IntegerTextBox_KeyDown;
            TextChanged += IntegerTextBox_TextChanged;
        }
        
        private void IntegerTextBox_Unloaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            KeyDown -= IntegerTextBox_KeyDown;
            TextChanged -= IntegerTextBox_TextChanged;
        }

        private void IntegerTextBox_KeyDown(object sender, global::Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == global::Windows.System.VirtualKey.Decimal)
            {
                e.Handled = true;
                SelectionStart = Text.Length;
            }
        }

        private void IntegerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(Text, out int result))
            {
                if (!string.IsNullOrEmpty(Text) && Text.Length != 0)
                {
                    Text = Text.Remove(Text.Length - 1);
                    SelectionStart = Text.Length;
                }
            }
        }
    }
}
