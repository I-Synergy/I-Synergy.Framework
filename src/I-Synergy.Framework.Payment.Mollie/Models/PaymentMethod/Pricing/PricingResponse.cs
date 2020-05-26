using ISynergy.Framework.Payment.Mollie.Abstractions.Models;

namespace ISynergy.Framework.Payment.Mollie.Models.PaymentMethod.Pricing
{
    /// <summary>
    /// Class PricingResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class PricingResponse : IResponseObject
    {
        /// <summary>
        /// The area or product-type where the pricing is applied for, translated in the optional locale passed.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// The fixed price per transaction
        /// </summary>
        /// <value>The fixed.</value>
        public FixedPricingResponse Fixed { get; set; }

        /// <summary>
        /// A string containing the percentage what will be charged over the payment amount besides the fixed price.
        /// </summary>
        /// <value>The variable.</value>
        public decimal Variable { get; set; }
    }
}
