namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderRefundRequest.
    /// </summary>
    public class OrderRefundRequest
    {
        /// <summary>
        /// An array of objects containing the order line details you want to create a refund for. If you send
        /// an empty array, the entire order will be refunded.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<OrderLineRequest> Lines { get; set; }

        /// <summary>
        /// The description of the refund you are creating. This will be shown to the consumer on their card or
        /// bank statement when possible. Max. 140 characters.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
    }
}
