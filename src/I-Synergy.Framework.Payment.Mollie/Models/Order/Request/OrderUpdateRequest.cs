namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderUpdateRequest.
    /// </summary>
    public class OrderUpdateRequest
    {
        /// <summary>
        /// The billing person and address for the order. See Order address details for the
        /// exact fields needed.
        /// </summary>
        /// <value>The billing address.</value>
        public OrderAddressDetails BillingAddress { get; set; }

        /// <summary>
        /// The shipping address for the order. See Order address details for the exact
        /// fields needed.
        /// </summary>
        /// <value>The shipping address.</value>
        public OrderAddressDetails ShippingAddress { get; set; }

        /// <summary>
        /// The order number. For example, 16738. We recommend that each order should have a unique order number.
        /// </summary>
        /// <value>The order number.</value>
        public string OrderNumber { get; set; }
    }
}
