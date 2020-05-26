using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderPaymentRequest.
    /// </summary>
    public class OrderPaymentRequest
    {
        /// <summary>
        /// Normally, a payment method screen is shown. However, when using this parameter, you can choose a
        /// specific payment method and your customer will skip the selection screen and is sent directly to
        /// the chosen payment method. The parameter enables you to fully integrate the payment method
        /// selection into your website.
        /// </summary>
        /// <value>The method.</value>
        public PaymentMethods? Method { get; set; }

        /// <summary>
        /// The ID of the Customer for whom the payment is being created. This is used for recurring payments
        /// and single click payments.
        /// </summary>
        /// <value>The customer identifier.</value>
        public string CustomerId { get; set; }

        /// <summary>
        /// When creating recurring payments, the ID of a specific Mandate may be supplied to indicate which
        /// of the consumer’s accounts should be credited.
        /// </summary>
        /// <value>The mandate identifier.</value>
        public string MandateId { get; set; }
    }
}
