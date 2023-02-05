using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Controls
{
    public class NumericEntry : Entry
    {
        public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(NumericTypes), typeof(NumericEntry), NumericTypes.Integer);

        public NumericTypes Type
        {
            get => (NumericTypes)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public NumericEntry()
            : base()
        {
            Keyboard = Keyboard.Numeric;
            IsPassword = false;
            IsTextPredictionEnabled = false;
            IsSpellCheckEnabled = false;

            Loaded += NumericEntry_Loaded;
            Unloaded += NumericEntry_Unloaded;
        }

        private void NumericEntry_Unloaded(object sender, EventArgs e) => TextChanged -= NumericEntry_TextChanged;

        private void NumericEntry_Loaded(object sender, EventArgs e) => TextChanged += NumericEntry_TextChanged;

        private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            this.TextChanged -= NumericEntry_TextChanged;

            switch (Type)
            {
                case NumericTypes.Double:
                    if (e.NewTextValue.EndsWith(separator))
                        Text = $"{e.NewTextValue.Replace(separator, "")}{separator}";

                    else if (string.IsNullOrEmpty(e.NewTextValue))
                        Text = (0d).ToString();

                    else if (double.TryParse(e.NewTextValue.Trim(), out double newResult))
                        Text = newResult.ToString();

                    else if (double.TryParse(e.OldTextValue.Trim(), out double oldResult))
                        Text = oldResult.ToString();

                    break;
                case NumericTypes.Decimal:
                    if (e.NewTextValue.EndsWith(separator))
                        Text = $"{e.NewTextValue.Replace(separator, "")}{separator}";

                    else if (string.IsNullOrEmpty(e.NewTextValue))
                        Text = (0m).ToString();

                    else if (decimal.TryParse(e.NewTextValue.Trim(), out decimal newResult))
                        Text = newResult.ToString();

                    else if (decimal.TryParse(e.OldTextValue.Trim(), out decimal oldResult))
                        Text = oldResult.ToString();

                    break;
                default:
                    if (string.IsNullOrEmpty(e.NewTextValue))
                        Text = (0).ToString();

                    else if (int.TryParse(e.NewTextValue.Trim(), out int newResult))
                        Text = newResult.ToString();

                    else if (int.TryParse(e.OldTextValue.Trim(), out int oldResult))
                        Text = oldResult.ToString();

                    break;
            }

            this.TextChanged += NumericEntry_TextChanged;
        }
    }
}