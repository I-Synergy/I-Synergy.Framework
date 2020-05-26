namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class IngHomePayPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class IngHomePayPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// An object with payment details.
        /// </summary>
        /// <value>The details.</value>
        public IngHomePayPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class IngHomePayPaymentResponseDetails.
    /// </summary>
    public class IngHomePayPaymentResponseDetails
    {
        /// <summary>
        /// Only available one banking day after the payment has been completed – The consumer’s name.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Only available one banking day after the payment has been completed – The consumer’s IBAN.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// Only available one banking day after the payment has been completed – BBRUBEBB.
        /// </summary>
        /// <value>The consumer bic.</value>
        public string ConsumerBic { get; set; }
    }
}
