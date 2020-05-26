using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ISynergy.Framework.Payment.Mollie.Models.Payment.Response
{
    /// <summary>
    /// Class CreditCardPaymentResponse.
    /// Implements the <see cref="PaymentResponse" />
    /// </summary>
    /// <seealso cref="PaymentResponse" />
    public class CreditCardPaymentResponse : PaymentResponse
    {
        /// <summary>
        /// An object with credit card details.
        /// </summary>
        /// <value>The details.</value>
        public CreditCardPaymentResponseDetails Details { get; set; }
    }

    /// <summary>
    /// Class CreditCardPaymentResponseDetails.
    /// </summary>
    public class CreditCardPaymentResponseDetails
    {
        /// <summary>
        /// The card holder's name.
        /// </summary>
        /// <value>The card holder.</value>
        public string CardHolder { get; set; }

        /// <summary>
        /// The last four digits of the card number.
        /// </summary>
        /// <value>The card number.</value>
        public string CardNumber { get; set; }

        /// <summary>
        /// Only available if the payment has been completed - Unique alphanumeric representation of card, usable for identifying
        /// returning customers.
        /// </summary>
        /// <value>The card fingerprint.</value>
        public string CardFingerprint { get; set; }

        /// <summary>
        /// Not always available. – The card's target audience.
        /// </summary>
        /// <value>The card audience.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CreditCardAudience? CardAudience { get; set; }

        /// <summary>
        /// The card's label. Note that not all labels can be acquired through ISynergy.Framework.Payment.Mollie.
        /// </summary>
        /// <value>The card label.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CreditCardLabel? CardLabel { get; set; }

        /// <summary>
        /// The ISO 3166-1 alpha-2 country code of the country the card was issued in. For example: BE.
        /// </summary>
        /// <value>The card country code.</value>
        public string CardCountryCode { get; set; }

        /// <summary>
        /// Only available if the payment succeeded. – The payment's security type.
        /// </summary>
        /// <value>The card security.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CreditCardSecurity? CardSecurity { get; set; }

        /// <summary>
        /// Only available if the payment succeeded. – The fee region for the payment. See your credit card addendum for
        /// details. intra-eu for consumer cards from the EU, and other for all other cards.
        /// </summary>
        /// <value>The fee region.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CreditCardFeeRegion? FeeRegion { get; set; }

        /// <summary>
        /// Only available for failed payments. Contains a failure reason code.
        /// </summary>
        /// <value>The failure reason.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CreditCardFailureReason? FailureReason { get; set; }
    }
}
