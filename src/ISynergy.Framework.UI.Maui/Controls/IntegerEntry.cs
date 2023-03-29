namespace ISynergy.Framework.UI.Controls
{
    public sealed class IntegerEntry : NumericEntry<int>
    {
        public IntegerEntry() : base()
        {
            Text = $"{0}";
        }

        public override void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.TextChanged -= NumericEntry_TextChanged;

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                Text = (0).ToString();
                Value = 0;
            }


            else if (!string.IsNullOrEmpty(e.NewTextValue) && int.TryParse(e.NewTextValue.Trim(), out int newResult))
            {
                Text = newResult.ToString();
                Value = newResult;
            }


            else if (!string.IsNullOrEmpty(e.OldTextValue) && int.TryParse(e.OldTextValue.Trim(), out int oldResult))
            {
                Text = oldResult.ToString();
                Value = oldResult;
            }

            this.TextChanged += NumericEntry_TextChanged;
        }
    }
}
