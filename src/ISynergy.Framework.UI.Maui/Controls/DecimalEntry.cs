using Microsoft.Maui.Controls;

namespace ISynergy.Framework.UI.Controls
{
    public class DecimalEntry : Entry
    {
        public DecimalEntry() : base()
        {
            this.Keyboard = Keyboard.Numeric;
            this.IsPassword = false;    
            this.IsTextPredictionEnabled = false;
            this.IsSpellCheckEnabled = false;

            this.Loaded += DecimalEntry_Loaded;
            this.Unloaded += DecimalEntry_Unloaded;
        }

        private void DecimalEntry_Unloaded(object sender, EventArgs e)
        {
            this.TextChanged -= DecimalEntry_TextChanged;
        }

        private void DecimalEntry_Loaded(object sender, EventArgs e)
        {
            this.TextChanged += DecimalEntry_TextChanged;
        }

        private void DecimalEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            this.TextChanged -= DecimalEntry_TextChanged;

            if (e.NewTextValue.EndsWith(separator))
                Text = $"{e.NewTextValue.Replace(separator, "")}{separator}";
            else if (string.IsNullOrEmpty(e.NewTextValue))
                Text = (0m).ToString();
            else if (decimal.TryParse(e.NewTextValue.Trim(), out decimal newResult))
                Text = newResult.ToString();
            else if (decimal.TryParse(e.OldTextValue.Trim(), out decimal oldResult))
                Text = oldResult.ToString();

            this.TextChanged += DecimalEntry_TextChanged;
        }
    }
}
