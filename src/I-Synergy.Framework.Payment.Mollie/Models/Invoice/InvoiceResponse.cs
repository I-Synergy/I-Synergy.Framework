using System.Collections.Generic;
using ISynergy.Framework.Payment.Mollie.Abstractions.Models;
using ISynergy.Framework.Payment.Mollie.Enumerations;
using ISynergy.Framework.Payment.Mollie.Models.Settlement;

namespace ISynergy.Framework.Payment.Mollie.Models.Invoice
{
    /// <summary>
    /// Class InvoiceResponse.
    /// Implements the <see cref="IResponseObject" />
    /// </summary>
    /// <seealso cref="IResponseObject" />
    public class InvoiceResponse : IResponseObject
    {
        /// <summary>
        /// Indicates the response contains an invoice object. Will always contain invoice for this endpoint.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }

        /// <summary>
        /// The invoice's unique identifier, for example inv_FrvewDA3Pr.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// The reference number of the invoice. An example value would be: 2016.10000.
        /// </summary>
        /// <value>The reference.</value>
        public string Reference { get; set; }

        /// <summary>
        /// Optional – The VAT number to which the invoice was issued to (if applicable).
        /// </summary>
        /// <value>The vat number.</value>
        public string VatNumber { get; set; }

        /// <summary>
        /// Status of the invoices
        /// </summary>
        /// <value>The status.</value>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// The invoice date (in YYYY-MM-DD format).
        /// </summary>
        /// <value>The issued at.</value>
        public string IssuedAt { get; set; }

        /// <summary>
        /// Optional – The date on which the invoice was paid (in YYYY-MM-DD format). Only for paid invoices.
        /// </summary>
        /// <value>The paid at.</value>
        public string PaidAt { get; set; }

        /// <summary>
        /// Optional – The date on which the invoice is due (in YYYY-MM-DD format). Only for due invoices.
        /// </summary>
        /// <value>The due at.</value>
        public string DueAt { get; set; }

        /// <summary>
        /// Total amount of the invoice excluding VAT, e.g. {"currency":Currency.EUR, "value":"100.00"}.
        /// </summary>
        /// <value>The net amount.</value>
        public Amount NetAmount { get; set; }

        /// <summary>
        /// VAT amount of the invoice. Only for merchants registered in the Netherlands. For EU merchants, VAT
        /// will be shifted to recipient; article 44 and 196 EU VAT Directive 2006/112. For merchants outside the
        /// EU, no VAT will be charged.
        /// </summary>
        /// <value>The vat amount.</value>
        public Amount VatAmount { get; set; }

        /// <summary>
        /// Total amount of the invoice including VAT.
        /// </summary>
        /// <value>The gross amount.</value>
        public Amount GrossAmount { get; set; }

        /// <summary>
        /// Only available if you require this field to be included – The collection of products which make up the invoice.
        /// </summary>
        /// <value>The lines.</value>
        public List<InvoiceLine> Lines { get; set; }

        /// <summary>
        /// Only available if you require this field to be included – A array of settlements that were invoiced on this invoice.
        /// You need the settlements.read permission for this field.
        /// </summary>
        /// <value>The settlements.</value>
        public List<SettlementResponse> Settlements { get; set; }

        /// <summary>
        /// Useful URLs to related resources.
        /// </summary>
        /// <value>The links.</value>
        [JsonProperty("_links")]
        public InvoiceResponseLinks Links { get; set; }
    }
}
