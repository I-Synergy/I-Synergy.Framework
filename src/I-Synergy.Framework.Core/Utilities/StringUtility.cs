using System.Globalization;

namespace ISynergy.Utilities
{
    public class StringUtility
    {
        public bool AddDecimalSeperator(bool seperatoradded)
        {
            return true;
        }

        public decimal ConvertStringToDecimal(decimal value, string input, bool seperatoradded)
        {
            string placeholder;

            if (seperatoradded)
            {
                var culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
                placeholder = value.ToString() + culture.NumberFormat.CurrencyDecimalSeparator + input;
            }
            else
            {
                placeholder = value.ToString() + input;
            }

            if (placeholder.StartsWith("0") && placeholder.Length > 1)
            {
                placeholder = placeholder.Remove(0, 1);
            }


            if (decimal.TryParse(placeholder, out decimal result))
            {
                return result;
            }

            return 0;
        }
    }
}
