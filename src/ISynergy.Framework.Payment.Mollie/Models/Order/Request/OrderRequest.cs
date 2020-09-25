using System;
using System.Collections.Generic;
using ISynergy.Framework.Payment.Converters;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using Newtonsoft.Json;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderRequest.
    /// </summary>
    public class OrderRequest
    {
        /// <summary>
        /// The total amount of the order, including VAT and discounts. This is the amount that will be charged
        /// to your customer.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }

        /// <summary>
        /// The order number. For example, 16738. We recommend that each order should have a unique order number.
        /// </summary>
        /// <value>The order number.</value>
        public string OrderNumber { get; set; }

        /// <summary>
        /// The lines in the order. Each line contains details such as a description of the item ordered, its
        /// price et cetera.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<OrderLineRequest> Lines { get; set; }

        /// <summary>
        /// The billing person and address for the order.
        /// </summary>
        /// <value>The billing address.</value>
        public OrderAddressDetails BillingAddress { get; set; }

        /// <summary>
        /// The shipping address for the order. See Order address details for the exact fields needed. If omitted,
        /// it is assumed to be identical to the billingAddress.
        /// </summary>
        /// <value>The shipping address.</value>
        public OrderAddressDetails ShippingAddress { get; set; }

        /// <summary>
        /// The date of birth of your customer. Some payment methods need this value and if you have it, you should
        /// send it so that your customer does not have to enter it again later in the checkout process.
        /// </summary>
        /// <value>The consumer date of birth.</value>
        public DateTime? ConsumerDateOfBirth { get; set; }

        /// <summary>
        /// The URL your customer will be redirected to after the payment process.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Set the webhook URL, where we will send order status changes to.
        /// </summary>
        /// <value>The webhook URL.</value>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// Allows you to preset the language to be used in the hosted payment pages shown to the consumer.
        /// </summary>
        /// <value>The locale.</value>
        public string Locale { get; set; }

        /// <summary>
        /// Normally, a payment method selection screen is shown. However, when using this parameter, your customer
        /// will skip the selection screen and will be sent directly to the chosen payment method. The parameter enables
        /// you to fully integrate the payment method selection into your website.
        /// </summary>
        /// <value>The method.</value>
        public PaymentMethods? Method { get; set; }

        /// <summary>
        /// Provide any data you like, and we will save the data alongside the subscription. Whenever you fetch the subscription
        /// with our API, we’ll also include the metadata. You can use up to 1kB of JSON.
        /// </summary>
        /// <value>The metadata.</value>
        [JsonConverter(typeof(RawJsonConverter))]
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
