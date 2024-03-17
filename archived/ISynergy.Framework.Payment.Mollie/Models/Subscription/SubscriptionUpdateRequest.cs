using System;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Subscription
{
    /// <summary>
    /// Class SubscriptionUpdateRequest.
    /// </summary>
    public class SubscriptionUpdateRequest
    {
        /// <summary>
        /// The constant amount in EURO that you want to charge with each subscription payment, e.g. 100.00 if you would want
        /// to charge € 100,00.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// Optional – Total number of charges for the subscription to complete. Leave empty for an on-going subscription.
        /// </summary>
        /// <value>The times.</value>
        public int? Times { get; set; }

        /// <summary>
        /// Optional – The start date of the subscription in yyyy-mm-dd format. This is the first day on which your customer
        /// will be charged. When
        /// this parameter is not provided, the current date will be used instead.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// A description unique per customer. This will be included in the payment description along with the charge date in
        /// Y-m-d format.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Optional – Use this parameter to set a webhook URL for all subscription payments.
        /// </summary>
        /// <value>The webhook URL.</value>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// Provide any data you like, and we will save the data alongside the subscription. Whenever you fetch the subscription
        /// with our API, we’ll also include the metadata. You can use up to 1kB of JSON.
        /// </summary>
        /// <value>The metadata.</value>
        public string Metadata { get; set; }

        /// <summary>
        /// Sets the metadata.
        /// </summary>
        /// <param name="metadataObj">The metadata object.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        public void SetMetadata(object metadataObj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            Metadata = JsonConvert.SerializeObject(metadataObj, jsonSerializerSettings);
        }
    }
}
