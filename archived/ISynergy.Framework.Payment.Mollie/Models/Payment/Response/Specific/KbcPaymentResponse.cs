namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class KbcPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class KbcPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public KbcPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class KbcPaymentResponseDetails.
    /// </summary>
    public class KbcPaymentResponseDetails
    {
        /// <summary>
        /// Only available if the payment has been completed – The consumer's name.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Only available if the payment has been completed – The consumer's IBAN.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// Only available if the payment has been completed – The consumer's bank's BIC.
        /// </summary>
        /// <value>The consumer bic.</value>
        public string ConsumerBic { get; set; }
    }
}
