namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response.Specific
{
    /// <summary>
    /// Class BancontactPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class BancontactPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public BancontactPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class BancontactPaymentResponseDetails.
    /// </summary>
    public class BancontactPaymentResponseDetails
    {
        /// <summary>
        /// Only available if the payment is completed - The last four digits of the card number.
        /// </summary>
        /// <value>The card number.</value>
        public string CardNumber { get; set; }

        /// <summary>
        /// Only available if the payment is completed - Unique alphanumeric representation of card, usable for
        /// identifying returning customers.
        /// </summary>
        /// <value>The card fingerprint.</value>
        public string CardFingerprint { get; set; }

        /// <summary>
        /// Only available if requested during payment creation - The QR code that can be scanned by the mobile
        /// Bancontact application. This enables the desktop to mobile feature.
        /// </summary>
        /// <value>The qr code.</value>
        public QrCode QrCode { get; set; }
    }
}
