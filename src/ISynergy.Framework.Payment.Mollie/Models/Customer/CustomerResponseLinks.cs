using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Payment.Response;
using ISynergy.Framework.Payment.Mollie.Models.Subscription;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Customer
{
    /// <summary>
    /// Class CustomerResponseLinks.
    /// </summary>
    public class CustomerResponseLinks
    {
        /// <summary>
        /// The API resource URL of the customer itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<CustomerResponse> Self { get; set; }

        /// <summary>
        /// The API resource URL of the subscriptions belonging to the Customer, if there are no subscriptions this parameter is omitted.
        /// </summary>
        /// <value>The subscriptions.</value>
        public UrlObjectLink<ListResponse<SubscriptionResponse>> Subscriptions { get; set; }

        /// <summary>
        /// The API resource URL of the payments belonging to the Customer, if there are no payments this parameter is omitted.
        /// </summary>
        /// <value>The payments.</value>
        public UrlObjectLink<ListResponse<PaymentResponse>> Payments { get; set; }

        /// <summary>
        /// The URL to the customer retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
