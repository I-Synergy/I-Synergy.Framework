using System.Collections.Generic;

namespace ISynergy.Framework.Payment.Mollie.Models.Order
{
    /// <summary>
    /// Class OrderLineCancellationRequest.
    /// </summary>
    public class OrderLineCancellationRequest
    {
        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<OrderLineDetails> Lines { get; set; }
    }
}
