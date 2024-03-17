namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response
{
    /// <summary>
    /// Class PaySafeCardPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class PaySafeCardPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public PaySafeCardPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class PaySafeCardPaymentResponseDetails.
    /// </summary>
    public class PaySafeCardPaymentResponseDetails
    {
        /// <summary>
        /// The consumer identification supplied when the payment was created.
        /// </summary>
        /// <value>The customer reference.</value>
        public string CustomerReference { get; set; }
    }
}
