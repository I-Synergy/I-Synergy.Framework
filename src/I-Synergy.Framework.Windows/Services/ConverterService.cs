using System;
using System.Globalization;

namespace ISynergy.Services
{
    public class ConverterService : IConverterService
    {
        private readonly IContext Context;

        public ConverterService(IContext context)
        {
            Context = context;
        }

        public int ConvertMediaColor2Integer(object mediacolor)
        {
            throw new NotImplementedException();
        }

        public string ConvertDecimalToCurrency(decimal value)
        {
            var currencySymbol = "$";

            currencySymbol = Context.CurrencySymbol;

            var info = CultureInfo.CurrentCulture.NumberFormat;
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", value).Replace("  ", " ");
        }
    }
}
