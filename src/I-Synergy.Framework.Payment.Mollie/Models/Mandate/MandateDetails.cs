namespace ISynergy.Framework.Payment.Mollie.Models.Mandate
{
    /// <summary>
    /// Class MandateDetails.
    /// </summary>
    public class MandateDetails
    {
        /// <summary>
        /// The direct debit account holder's name.
        /// </summary>
        /// <value>The name of the consumer.</value>
        public string ConsumerName { get; set; }

        /// <summary>
        /// The direct debit account IBAN.
        /// </summary>
        /// <value>The consumer account.</value>
        public string ConsumerAccount { get; set; }

        /// <summary>
        /// The direct debit account BIC.
        /// </summary>
        /// <value>The consumer bic.</value>
        public string ConsumerBic { get; set; }

        /// <summary>
        /// The credit card holder's name.
        /// </summary>
        /// <value>The card holder.</value>
        public string CardHolder { get; set; }

        /// <summary>
        /// The last four digits of the credit card number.
        /// </summary>
        /// <value>The card number.</value>
        public string CardNumber { get; set; }

        /// <summary>
        /// The credit card's label. Note that not all labels can be acquired through ISynergy.Framework.Payment.Mollie.
        /// </summary>
        /// <value>The card label.</value>
        public string CardLabel { get; set; }

        /// <summary>
        /// Unique alphanumeric representation of credit card, usable for identifying returning customers.
        /// </summary>
        /// <value>The card fingerprint.</value>
        public string CardFingerprint { get; set; }

        /// <summary>
        /// Expiry date of the credit card card in YYYY-MM-DD format.
        /// </summary>
        /// <value>The card expiry date.</value>
        public string CardExpiryDate { get; set; }
    }
}
