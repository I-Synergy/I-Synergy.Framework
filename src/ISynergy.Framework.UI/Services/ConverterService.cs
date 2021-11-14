namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class ConverterService.
    /// Implements the <see cref="IConverterService" />
    /// </summary>
    /// <seealso cref="IConverterService" />
    public class ConverterService : IConverterService
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ConverterService(IContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Converts the media color2 integer.
        /// </summary>
        /// <param name="mediacolor">The mediacolor.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int ConvertMediaColor2Integer(object mediacolor)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the decimal to currency.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
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
