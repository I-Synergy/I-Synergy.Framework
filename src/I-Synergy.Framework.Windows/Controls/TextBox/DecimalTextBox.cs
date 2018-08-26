using System.Text.RegularExpressions;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    public class DecimalTextBox : TextBox
    {
        public DecimalTextBox() : base()
        {
            this.Loaded += DecimalTextBox_Loaded;
            this.Unloaded += DecimalTextBox_Unloaded;
        }

        private void DecimalTextBox_Loaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.KeyDown += DecimalTextBox_KeyDown;
            this.TextChanged += DecimalTextBox_TextChanged;
        }

        private void DecimalTextBox_Unloaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.KeyDown -= DecimalTextBox_KeyDown;
            this.TextChanged += DecimalTextBox_TextChanged;
        }

        private void DecimalTextBox_KeyDown(object sender, global::Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == global::Windows.System.VirtualKey.Decimal)
            {
                string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                e.Handled = true;

                if(this.Text.Contains(separator))
                {
                    this.Text = this.Text.Replace(separator, "");
                }

                this.Text = this.Text + separator;
                this.SelectionStart = this.Text.Length;
            }
        }

        private void DecimalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!decimal.TryParse(this.Text, out decimal result))
            {
                if(!string.IsNullOrEmpty(this.Text) && this.Text.Length != 0)
                {
                    this.Text = this.Text.Remove(this.Text.Length - 1);
                    this.SelectionStart = this.Text.Length;
                }
            }
        }
    }
}
