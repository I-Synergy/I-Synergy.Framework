using System.Globalization;
using System.Threading;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    public class DecimalTextBox : TextBox
    {
        public DecimalTextBox() : base()
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

            Loaded += DecimalTextBox_Loaded;
            Unloaded += DecimalTextBox_Unloaded;
        }

        private void DecimalTextBox_Loaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            KeyDown += DecimalTextBox_KeyDown;
            TextChanged += DecimalTextBox_TextChanged;
        }

        private void DecimalTextBox_Unloaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            KeyDown -= DecimalTextBox_KeyDown;
            TextChanged -= DecimalTextBox_TextChanged;
        }

        private void DecimalTextBox_KeyDown(object sender, global::Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == global::Windows.System.VirtualKey.Decimal)
            {
                string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                e.Handled = true;

                if(Text.Contains(separator))
                {
                    Text = Text.Replace(separator, "");
                }

                Text = Text + separator;
                SelectionStart = Text.Length;
            }
        }

        private void DecimalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!decimal.TryParse(Text, out decimal result))
            {
                if(!string.IsNullOrEmpty(Text) && Text.Length != 0)
                {
                    Text = Text.Remove(Text.Length - 1);
                    SelectionStart = Text.Length;
                }
            }
        }
    }
}
