﻿namespace ISynergy.Framework.Payment.Mollie.Models.Invoice
{
    /// <summary>
    /// Class InvoiceLine.
    /// </summary>
    public class InvoiceLine
    {
        /// <summary>
        /// The administrative period (YYYY) on which the line should be booked.
        /// </summary>
        /// <value>The period.</value>
        public string Period { get; set; }

        /// <summary>
        /// Description of the product.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Number of products invoiced (usually number of payments).
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Optional – VAT percentage rate that applies to this product.
        /// </summary>
        /// <value>The vat percentage.</value>
        public decimal VatPercentage { get; set; }

        /// <summary>
        /// Amount excluding VAT.
        /// </summary>
        /// <value>The amount.</value>
        public Amount Amount { get; set; }
    }
}
