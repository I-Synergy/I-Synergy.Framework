using ISynergy.Framework.Payment.Mollie.Abstractions.Models;

namespace ISynergy.Framework.Payment.Mollie.Models.PaymentMethod.Pricing
{
    /// <summary>
    /// Class FixedPricingResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class FixedPricingResponse : IResponseObject
    {
        /// <summary>
        /// The ISO 4217 currency code.
        /// </summary>
        /// <value>The currency.</value>
        public string Currency { get; set; }

        /// <summary>
        /// A string containing the exact amount in the given currency.
        /// </summary>
        /// <value>The value.</value>
        public decimal Value { get; set; }
    }
}
