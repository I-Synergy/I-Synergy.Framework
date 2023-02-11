namespace ISynergy.Framework.UI.Controls
{
    public sealed class DecimalEntry : NumericEntry<decimal>
    {
        public DecimalEntry() : base()
        {
        }

        public override void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            this.TextChanged -= NumericEntry_TextChanged;

            if (e.NewTextValue.EndsWith(separator))
                Text = $"{e.NewTextValue.Replace(separator, "")}{separator}";

            else if (string.IsNullOrEmpty(e.NewTextValue))
            {
                Text = (0m).ToString();
                Value = 0m;
            }

            else if (decimal.TryParse(e.NewTextValue.Trim(), out decimal newResult))
            {
                Text = newResult.ToString();
                Value = newResult;
            }

            else if (decimal.TryParse(e.OldTextValue.Trim(), out decimal oldResult))
            {
                Text = oldResult.ToString();
                Value = oldResult;
            }

            this.TextChanged += NumericEntry_TextChanged;
        }
    }
}