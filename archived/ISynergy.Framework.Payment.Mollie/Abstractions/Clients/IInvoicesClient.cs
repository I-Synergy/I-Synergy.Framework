using System.Threading.Tasks;
using ISynergy.Framework.Payment.Mollie.Models.Invoice;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IInvoicesClient
    /// </summary>
    public interface IInvoicesClient
    {
        /// <summary>
        /// Gets the invoice asynchronous.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="includeLines">if set to <c>true</c> [include lines].</param>
        /// <param name="includeSettlements">if set to <c>true</c> [include settlements].</param>
        /// <returns>Task&lt;InvoiceResponse&gt;.</returns>
        Task<InvoiceResponse> GetInvoiceAsync(string invoiceId, bool includeLines = false, bool includeSettlements = false);
        /// <summary>
        /// Gets the invoice asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;InvoiceResponse&gt;.</returns>
        Task<InvoiceResponse> GetInvoiceAsync(UrlObjectLink<InvoiceResponse> url);
        /// <summary>
        /// Gets the invoice list asynchronous.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="year">The year.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="includeLines">if set to <c>true</c> [include lines].</param>
        /// <param name="includeSettlements">if set to <c>true</c> [include settlements].</param>
        /// <returns>Task&lt;ListResponse&lt;InvoiceResponse&gt;&gt;.</returns>
        Task<ListResponse<InvoiceResponse>> GetInvoiceListAsync(string reference = null, int? year = null, string from = null, int? limit = null, bool includeLines = false, bool includeSettlements = false);
    }
}
