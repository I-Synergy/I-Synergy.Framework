using ISynergy.Framework.Payment.Extensions;
using ISynergy.Framework.Payment.Mollie.Abstractions.Clients;
using ISynergy.Framework.Payment.Mollie.Base;
using ISynergy.Framework.Payment.Mollie.Models.Invoice;
using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Url;
using ISynergy.Framework.Payment.Mollie.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Payment.Mollie.Clients
{
    /// <summary>
    /// Class InvoicesClient.
    /// Implements the <see cref="MollieClientBase" />
    /// Implements the <see cref="IInvoicesClient" />
    /// </summary>
    /// <seealso cref="MollieClientBase" />
    /// <seealso cref="IInvoicesClient" />
    public class InvoicesClient : MollieClientBase, IInvoicesClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesClient" /> class.
        /// </summary>
        /// <param name="clientService">The client service.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public InvoicesClient(IMollieClientService clientService, IOptions<MollieApiOptions> options, ILogger<InvoicesClient> logger) : base(clientService, options, logger)
        {
        }

        /// <summary>
        /// Gets the invoice asynchronous.
        /// </summary>
        /// <param name="invoiceId">The invoice identifier.</param>
        /// <param name="includeLines">if set to <c>true</c> [include lines].</param>
        /// <param name="includeSettlements">if set to <c>true</c> [include settlements].</param>
        /// <returns>Task&lt;InvoiceResponse&gt;.</returns>
        public Task<InvoiceResponse> GetInvoiceAsync(string invoiceId, bool includeLines = false,
            bool includeSettlements = false)
        {
            var includes = BuildIncludeParameter(includeLines, includeSettlements);
            return _clientService.GetAsync<InvoiceResponse>($"invoices/{invoiceId}{includes.ToQueryString()}");
        }

        /// <summary>
        /// Gets the invoice asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;InvoiceResponse&gt;.</returns>
        public Task<InvoiceResponse> GetInvoiceAsync(UrlObjectLink<InvoiceResponse> url) =>
            _clientService.GetAsync(url);

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
        public Task<ListResponse<InvoiceResponse>> GetInvoiceListAsync(string reference = null, int? year = null, string from = null, int? limit = null, bool includeLines = false, bool includeSettlements = false)
        {
            var parameters = BuildIncludeParameter(includeLines, includeSettlements);
            parameters.AddValueIfNotNullOrEmpty(nameof(reference), reference);
            parameters.AddValueIfNotNullOrEmpty(nameof(year), Convert.ToString(year));

            return _clientService.GetListAsync<ListResponse<InvoiceResponse>>($"invoices", from, limit, parameters);
        }

        /// <summary>
        /// Builds the include parameter.
        /// </summary>
        /// <param name="includeLines">if set to <c>true</c> [include lines].</param>
        /// <param name="includeSettlements">if set to <c>true</c> [include settlements].</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        private Dictionary<string, string> BuildIncludeParameter(bool includeLines = false, bool includeSettlements = false)
        {
            var result = new Dictionary<string, string>();

            var includeList = new List<string>();
            if (includeLines)
            {
                includeList.Add("lines");
            }

            if (includeSettlements)
            {
                includeList.Add("settlements");
            }

            if (includeList.Any())
            {
                result.Add("include", string.Join(",", includeList));
            }

            return result;
        }
    }
}
