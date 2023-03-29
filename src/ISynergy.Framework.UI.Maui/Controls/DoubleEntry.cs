namespace ISynergy.Framework.UI.Controls
{
    public sealed class DoubleEntry : NumericEntry<double>
    {
        public DoubleEntry() : base()
        {
            Text = $"{0d}";
        }

        public override void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            this.TextChanged -= NumericEntry_TextChanged;

            if (e.NewTextValue.EndsWith(separator))
                Text = $"{e.NewTextValue.Replace(separator, "")}{separator}";

            else if (string.IsNullOrEmpty(e.NewTextValue))
            {
                Text = (0d).ToString();
                Value = 0d;
            }

            else if (!string.IsNullOrEmpty(e.NewTextValue) && double.TryParse(e.NewTextValue.Trim(), out double newResult))
            {
                Text = newResult.ToString();
                Value = newResult;
            }

            else if (!string.IsNullOrEmpty(e.OldTextValue) && double.TryParse(e.OldTextValue.Trim(), out double oldResult))
            {
                Text = oldResult.ToString();
                Value = oldResult;
            }

            this.TextChanged += NumericEntry_TextChanged;
        }
    }
}