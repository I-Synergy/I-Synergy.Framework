namespace ISynergy.Framework.Payment.Mollie.Models.Invoice
{
    /// <summary>
    /// Class InvoiceAmount.
    /// </summary>
    public class InvoiceAmount
    {
        /// <summary>
        /// Gets or sets the net.
        /// </summary>
        /// <value>The net.</value>
        public decimal Net { get; set; }
        /// <summary>
        /// Gets or sets the vat.
        /// </summary>
        /// <value>The vat.</value>
        public decimal Vat { get; set; }
        /// <summary>
        /// Gets or sets the gross.
        /// </summary>
        /// <value>The gross.</value>
        public decimal Gross { get; set; }
    }
}
