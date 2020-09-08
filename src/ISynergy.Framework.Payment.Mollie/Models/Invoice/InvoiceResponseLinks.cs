using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Models.Invoice
{
    /// <summary>
    /// Class InvoiceResponseLinks.
    /// </summary>
    public class InvoiceResponseLinks
    {
        /// <summary>
        /// The API resource URL of the invoice itself.
        /// </summary>
        /// <value>The self.</value>
        public UrlObjectLink<InvoiceResponse> Self { get; set; }

        /// <summary>
        /// The URL to the PDF version of the invoice. The URL will expire after 60 minutes.
        /// </summary>
        /// <value>The PDF.</value>
		public UrlLink Pdf { get; set; }

        /// <summary>
        /// The URL to the invoice retrieval endpoint documentation.
        /// </summary>
        /// <value>The documentation.</value>
		public UrlLink Documentation { get; set; }
    }
}
