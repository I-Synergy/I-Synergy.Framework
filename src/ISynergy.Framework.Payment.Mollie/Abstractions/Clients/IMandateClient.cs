using ISynergy.Framework.Payment.Mollie.Models.List;
using ISynergy.Framework.Payment.Mollie.Models.Mandate;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Abstractions.Clients
{
    /// <summary>
    /// Interface IMandateClient
    /// </summary>
    public interface IMandateClient
    {
        /// <summary>
        /// Gets the mandate asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="mandateId">The mandate identifier.</param>
        /// <returns>Task&lt;MandateResponse&gt;.</returns>
        Task<MandateResponse> GetMandateAsync(string customerId, string mandateId);
        /// <summary>
        /// Gets the mandate list asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>Task&lt;ListResponse&lt;MandateResponse&gt;&gt;.</returns>
        Task<ListResponse<MandateResponse>> GetMandateListAsync(string customerId, string from = null, int? limit = null);
        /// <summary>
        /// Creates the mandate asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns>Task&lt;MandateResponse&gt;.</returns>
        Task<MandateResponse> CreateMandateAsync(string customerId, MandateRequest request);
        /// <summary>
        /// Gets the mandate list asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;ListResponse&lt;MandateResponse&gt;&gt;.</returns>
        Task<ListResponse<MandateResponse>> GetMandateListAsync(UrlObjectLink<ListResponse<MandateResponse>> url);
        /// <summary>
        /// Gets the mandate asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;MandateResponse&gt;.</returns>
        Task<MandateResponse> GetMandateAsync(UrlObjectLink<MandateResponse> url);
        /// <summary>
        /// Revokes the mandate asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="mandateId">The mandate identifier.</param>
        /// <returns>Task.</returns>
        Task RevokeMandateAsync(string customerId, string mandateId);
    }
}
