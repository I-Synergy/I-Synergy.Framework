using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Locators;
using Windows.Globalization.NumberFormatting;

namespace ISynergy.Framework.UI.Formatters
{
    public class DecimalFormatter : INumberFormatter2, INumberParser
    {
        private readonly IContext _context;

        public int Decimals { get; set; }

        public DecimalFormatter()
        {
            _context = ServiceLocator.Default.GetInstance<IContext>();
            Decimals = _context.NumberFormat.CurrencyDecimalDigits;
        }

        public DecimalFormatter(int decimals)
            : this()
        {
            Decimals = decimals;
        }

        public string FormatDouble(double value)
        {
            return value.ToString($"N{Decimals}");
        }

        public string FormatInt(long value)
        {
            return value.ToString($"N{Decimals}");
        }

        public string FormatUInt(ulong value)
        {
            return value.ToString($"N{Decimals}");
        }

        public double? ParseDouble(string text)
        {
            return Convert.ToDouble(text);
        }

        public long? ParseInt(string text)
        {
            return Convert.ToInt64(text);
        }

        public ulong? ParseUInt(string text)
        {
            return Convert.ToUInt32(text);
        }
    }
}
