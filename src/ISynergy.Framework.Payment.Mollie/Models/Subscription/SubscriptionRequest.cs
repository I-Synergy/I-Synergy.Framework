using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Subscription
{
    /// <summary>
    /// Class SubscriptionRequest.
    /// Implements the <see cref="SubscriptionUpdateRequest" />
    /// </summary>
    /// <seealso cref="SubscriptionUpdateRequest" />
    public class SubscriptionRequest : SubscriptionUpdateRequest
    {
        /// <summary>
        /// Interval to wait between charges like 1 month(s) or 14 days.
        /// </summary>
        /// <value>The interval.</value>
        public string Interval { get; set; }

        /// <summary>
        /// Optional – The payment method used for this subscription, either forced on creation or null if any of the
        /// customer's valid mandates may be used.
        /// </summary>
        /// <value>The method.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethods? Method { get; set; }
    }
}
