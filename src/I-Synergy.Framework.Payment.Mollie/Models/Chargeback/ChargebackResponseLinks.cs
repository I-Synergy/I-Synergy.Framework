using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Settlement;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Chargeback
{
    /// <summary>
    /// Class ChargebackResponseLinks.
    /// </summary>
    public class ChargebackResponseLinks
    {
        /// <summary>
        /// The API resource URL of the chargeback itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<ChargebackResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the payment this chargeback belongs to.
        /// </summary>
        /// <value>The payment.</value>
        public UrlObjectLink<PaymentResponse> Payment { get; set; }

        /// <summary>
        /// The API resource URL of the settlement this payment has been settled with. Not present if not yet settled.
        /// </summary>
        /// <value>The settlement.</value>
        public UrlObjectLink<SettlementResponse> Settlement { get; set; }

        /// <summary>
        /// The URL to the chargeback retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
