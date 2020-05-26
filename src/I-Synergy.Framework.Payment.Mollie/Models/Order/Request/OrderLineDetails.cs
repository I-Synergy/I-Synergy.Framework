namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderLineDetails.
    /// </summary>
    public class OrderLineDetails
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public int? Quantity { get; set; }
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }
    }
}
