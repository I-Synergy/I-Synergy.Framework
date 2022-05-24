using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.PaymentMethod
{
    /// <summary>
    /// Class PaymentMethodResponseLinks.
    /// </summary>
    public class PaymentMethodResponseLinks
    {
        /// <summary>
        /// The API resource URL of the payment method itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<PaymentMethodResponse> Self { get; set; }

        /// <summary>
        /// The URL to the payment method retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
