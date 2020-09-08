using System;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Subscription
{
    /// <summary>
    /// Class SubscriptionResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class SubscriptionResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains a subscription object.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The subscription's unique identifier, for example sub_rVKGtNd6s3.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The mode used to create this subscription. Mode determines whether the payments are real or test payments.
        /// </summary>
        /// <value>The mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public Mode Mode { get; set; }

        /// <summary>
        /// The subscription's date and time of creation, in ISO 8601 format.
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The subscription's current status, depends on whether the customer has a pending, valid or invalid mandate.
        /// </summary>
        /// <value>The status.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// The constant amount that is charged with each subscription payment.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// Total number of charges for the subscription to complete.
        /// </summary>
        /// <value>The times.</value>
        public int? Times { get; set; }

        /// <summary>
        /// Interval to wait between charges like 1 month(s) or 14 days.
        /// </summary>
        /// <value>The interval.</value>
        public string Interval { get; set; }

        /// <summary>
        /// The start date of the subscription in yyyy-mm-dd format.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date of the next scheduled payment in YYYY-MM-DD format. When there will be no next payment, for example
        /// when the subscription has ended, this parameter will not be returned.
        /// </summary>
        /// <value>The next payment date.</value>
        public DateTime? NextPaymentDate { get; set; }

        /// <summary>
        /// A description unique per customer. This will be included in the payment description along with the charge date in
        /// Y-m-d format.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// The payment method used for this subscription, either forced on creation by specifying the method parameter, or
        /// null if any of the customer's valid mandates may be used.
        /// </summary>
        /// <value>The method.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentMethods? Method { get; set; }

        /// <summary>
        /// The subscription's date of cancellation, in ISO 8601 format.
        /// </summary>
        /// <value>The canceled at.</value>
        public DateTime? CanceledAt { get; set; }

        /// <summary>
        /// The URL ISynergy.Framework.Payment.Mollie will call as soon a payment status change takes place.
        /// </summary>
        /// <value>The webhook URL.</value>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// The optional metadata you provided upon subscription creation. Metadata can for example be used to link a plan to a
        /// subscription.
        /// </summary>
        /// <value>The metadata.</value>
        [JsonConverter(typeof(RawJsonConverter))]
        public string Metadata { get; set; }

        /// <summary>
        /// An object with several URL objects relevant to the subscription. Every URL object will contain an href and a type field.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public SubscriptionResponseLinks Links { get; set; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <returns>T.</returns>
        public T GetMetadata<T>(JsonSerializerSettings jsonSerializerSettings = null)
        {
            return JsonSerializer.Deserialize<T>(Metadata, jsonSerializerSettings);
        }
    }
}
