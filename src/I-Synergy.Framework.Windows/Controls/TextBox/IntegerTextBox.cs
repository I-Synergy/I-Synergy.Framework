using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    public class IntegerTextBox : TextBox
    {
        public IntegerTextBox() : base()
        {
            this.Loaded += IntegerTextBox_Loaded;
            this.Unloaded += IntegerTextBox_Unloaded;
        }

        private void IntegerTextBox_Loaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.KeyDown += IntegerTextBox_KeyDown;
            this.TextChanged += IntegerTextBox_TextChanged;
        }
        
        private void IntegerTextBox_Unloaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.KeyDown -= IntegerTextBox_KeyDown;
            this.TextChanged -= IntegerTextBox_TextChanged;
        }

        private void IntegerTextBox_KeyDown(object sender, global::Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == global::Windows.System.VirtualKey.Decimal)
            {
                e.Handled = true;
                this.SelectionStart = this.Text.Length;
            }
        }

        private void IntegerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(this.Text, out int result))
            {
                if (!string.IsNullOrEmpty(this.Text) && this.Text.Length != 0)
                {
                    this.Text = this.Text.Remove(this.Text.Length - 1);
                    this.SelectionStart = this.Text.Length;
                }
            }
        }
    }
}
