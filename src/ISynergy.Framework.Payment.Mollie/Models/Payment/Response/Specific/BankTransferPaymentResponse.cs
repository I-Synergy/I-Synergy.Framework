using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class BankTransferPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class BankTransferPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public BankTransferPaymentResponseDetails Details { get; set; }

        /// <summary>
        /// For bank transfer payments, the _links object will contain some additional URL objects relevant to the payment.
        /// </summary>
        /// <value>The links.</value>
        public new BankTransferPaymentResponseLinks Links { get; set; }
    }

    /// <summary>
    /// Class BankTransferPaymentResponseDetails.
    /// </summary>
    public class BankTransferPaymentResponseDetails
    {
        /// <summary>
        /// The name of the bank the consumer should wire the amount to.
        /// </summary>
        /// <value>The name of the bank.</value>
        public string BankName { get; set; }

        /// <summary>
        /// The IBAN the consumer should wire the amount to.
        /// </summary>
        /// <value>The bank account.</value>
        public string BankAccount { get; set; }

        /// <summary>
        /// The BIC of the bank the consumer should wire the amount to.
        /// </summary>
        /// <value>The bank bic.</value>
        public string BankBic { get; set; }

        /// <summary>
        /// The reference the consumer should use when wiring the amount. Note you should not apply any formatting here; show
        /// it to the consumer as-is.
        /// </summary>
        /// <value>The transfer reference.</value>
        public string TransferReference { get; set; }

        /// <summary>
        /// Only available if the payment has been completed – The consumer's name.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Only available if the payment has been completed – The consumer's bank account. This may be an IBAN, or it may be a
        /// domestic account number.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// Only available if the payment has been completed – The consumer's bank's BIC / SWIFT code.
        /// </summary>
        /// <value>The consumer bic.</value>
        public string ConsumerBic { get; set; }

        /// <summary>
        /// Only available if filled out in the API or by the consumer – The email address which the consumer asked the payment
        /// instructions to be sent to.
        /// </summary>
        /// <value>The billing email.</value>
        public string BillingEmail { get; set; }
    }

    /// <summary>
    /// Class BankTransferPaymentResponseLinks.
    /// Implements the <see cref="PaymentResponseLinks" />
    /// </summary>
    /// <seealso cref="PaymentResponseLinks" />
    public class BankTransferPaymentResponseLinks : PaymentResponseLinks
    {
        /// <summary>
        /// A link to a hosted payment page where your customer can check the status of their payment.
        /// </summary>
        /// <value>The status.</value>
        public UrlLink Status { get; set; }

        /// <summary>
        /// A link to a hosted payment page where your customer can finish the payment using an alternative payment method also
        /// activated on your website profile.
        /// </summary>
        /// <value>The pay online.</value>
        public UrlLink PayOnline { get; set; }
    }
}
