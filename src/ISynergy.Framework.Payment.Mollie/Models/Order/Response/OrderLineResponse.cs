using ISynergy.Framework.Payment.Mollie.Enumerations;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderLineResponse.
    /// Implements the <see cref="OrderLineRequest" />
    /// </summary>
    /// <seealso cref="OrderLineRequest" />
    public class OrderLineResponse : OrderLineRequest
    {
        /// <summary>
        /// The order line’s unique identifier, for example odl_dgtxyl.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Status of the order line
        /// </summary>
        /// <value>The status.</value>
        public OrderLineStatus Status { get; set; }

        /// <summary>
        /// Whether or not the order line can be (partially) canceled.
        /// </summary>
        /// <value><c>true</c> if this instance is cancelable; otherwise, <c>false</c>.</value>
        public bool IsCancelable { get; set; }

        /// <summary>
        /// The number of items that are shipped for this order line.
        /// </summary>
        /// <value>The quantity shipped.</value>
        public int QuantityShipped { get; set; }

        /// <summary>
        /// The total amount that is shipped for this order line.
        /// </summary>
        /// <value>The amount shipped.</value>
        public Amount AmountShipped { get; set; }

        /// <summary>
        /// The number of items that are refunded for this order line.
        /// </summary>
        /// <value>The quantity refunded.</value>
        public int QuantityRefunded { get; set; }

        /// <summary>
        /// The total amount that is refunded for this order line.
        /// </summary>
        /// <value>The amount refunded.</value>
        public Amount AmountRefunded { get; set; }

        /// <summary>
        /// The number of items that are canceled in this order line.
        /// </summary>
        /// <value>The quantity canceled.</value>
        public int QuantityCanceled { get; set; }

        /// <summary>
        /// The total amount that is canceled in this order line.
        /// </summary>
        /// <value>The amount canceled.</value>
        public Amount AmountCanceled { get; set; }

        /// <summary>
        /// The number of items that can still be shipped for this order line.
        /// </summary>
        /// <value>The shippable quantity.</value>
        public int ShippableQuantity { get; set; }

        /// <summary>
        /// The number of items that can still be refunded for this order line.
        /// </summary>
        /// <value>The refundable quantity.</value>
        public int RefundableQuantity { get; set; }

        /// <summary>
        /// The number of items that can still be canceled for this order line.
        /// </summary>
        /// <value>The cancelable quantity.</value>
        public int CancelableQuantity { get; set; }

        /// <summary>
        /// The order line’s date and time of creation
        /// </summary>
        /// <value>The created at.</value>
        public DateTime CreatedAt { get; set; }
    }
}
