using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderResponseLinks.
    /// </summary>
    public class OrderResponseLinks
    {
        /// <summary>
        /// The API resource URL of the order itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<OrderResponse> Self { get; set; }

        /// <summary>
        /// The URL your customer should visit to make the payment for the order.
        /// This is where you should redirect the customer to after creating the order.
        /// </summary>
        /// <value>The checkout.</value>
        public UrlLink Checkout { get; set; }

        /// <summary>
        /// The URL to the order retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
        public UrlLink Documentation { get; set; }
    }
}
